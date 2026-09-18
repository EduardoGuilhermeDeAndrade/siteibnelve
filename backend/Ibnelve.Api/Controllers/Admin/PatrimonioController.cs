using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Domain;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Ibnelve.Api.Controllers.Admin;

/// <summary>
/// Controle de patrimônio (bens físicos da igreja). Leitura é liberada a qualquer usuário
/// autenticado do Portal; escrita exige ADMIN ou o papel restrito PATRIMONIO_EDITOR — ver
/// CLAUDE.md, decisão de 2026-09-18.
/// </summary>
[ApiController]
[Route("api/admin/patrimonio")]
[Authorize]
public class PatrimonioController(IbnelveDbContext db) : ControllerBase
{
    private const string PapeisEscrita = $"{AdminSeeder.PapelAdmin},{AdminSeeder.PapelPatrimonioEditor}";

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ItemPatrimonioDto>>> Listar()
    {
        var itens = await db.ItensPatrimonio.Include(i => i.Local).OrderBy(i => i.Descricao).ToListAsync();
        var emprestadoPorItem = await ObterQuantidadeEmprestadaPorItemAsync();

        return itens.Select(i => ParaDto(i, emprestadoPorItem.GetValueOrDefault(i.Id))).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemPatrimonioDto>> ObterPorId(Guid id)
    {
        var item = await db.ItensPatrimonio.Include(i => i.Local).FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        var emprestado = await db.EmprestimosPatrimonio
            .Where(e => e.ItemPatrimonioId == id && e.DataHoraDevolucao == null)
            .SumAsync(e => (int?)e.Quantidade) ?? 0;

        return ParaDto(item, emprestado);
    }

    [HttpGet("{id:guid}/emprestimos")]
    public async Task<ActionResult<IReadOnlyList<EmprestimoPatrimonioDto>>> ListarEmprestimos(Guid id)
    {
        if (!await db.ItensPatrimonio.AnyAsync(i => i.Id == id))
        {
            return NotFound();
        }

        var emprestimos = await db.EmprestimosPatrimonio
            .Where(e => e.ItemPatrimonioId == id)
            .OrderByDescending(e => e.DataHoraRetirada)
            .ToListAsync();

        return emprestimos.Select(ParaDto).ToList();
    }

    // Anônimo: uma tag <img> não envia o token Bearer, então o binário em si precisa ficar
    // acessível sem sessão (mesmo padrão de GaleriaController/ImagensSiteController) — a URL só
    // é descoberta através da listagem, que continua exigindo login.
    [HttpGet("{id:guid}/foto/arquivo")]
    [AllowAnonymous]
    public async Task<IActionResult> Foto(Guid id)
    {
        var item = await db.ItensPatrimonio.FindAsync(id);
        if (item?.FotoConteudo is null || item.FotoContentType is null)
        {
            return NotFound();
        }

        DefinirCacheImutavel();
        return File(item.FotoConteudo, item.FotoContentType);
    }

    [HttpGet("{id:guid}/foto-defeito/arquivo")]
    [AllowAnonymous]
    public async Task<IActionResult> FotoDefeito(Guid id)
    {
        var item = await db.ItensPatrimonio.FindAsync(id);
        if (item?.FotoDefeitoConteudo is null || item.FotoDefeitoContentType is null)
        {
            return NotFound();
        }

        DefinirCacheImutavel();
        return File(item.FotoDefeitoConteudo, item.FotoDefeitoContentType);
    }

    [HttpPost]
    [Authorize(Roles = PapeisEscrita)]
    [RequestSizeLimit(ValidacaoImagem.TamanhoMaximoBytes)]
    public async Task<ActionResult<ItemPatrimonioDto>> Criar(
        IFormFile? arquivo,
        [FromForm] string descricao,
        [FromForm] string? numeroPatrimonio,
        [FromForm] string tipoControle,
        [FromForm] int quantidadeTotal,
        [FromForm] Guid? localId,
        [FromForm] string? observacao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            return BadRequest(new { message = "Informe a descrição do item." });
        }

        TipoControlePatrimonio tipo;
        try
        {
            tipo = EnumMappings.ParseTipoControlePatrimonio(tipoControle);
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { message = erro.Message });
        }

        byte[]? fotoConteudo = null;
        string? fotoContentType = null;
        if (arquivo is not null)
        {
            var erroValidacao = await ValidacaoImagem.ValidarAsync(arquivo);
            if (erroValidacao is not null)
            {
                return BadRequest(new { message = erroValidacao });
            }

            using var stream = new MemoryStream();
            await arquivo.CopyToAsync(stream);
            fotoConteudo = stream.ToArray();
            fotoContentType = arquivo.ContentType;
        }

        var usuarioId = ObterUsuarioIdAtual();
        var agora = DateTimeOffset.UtcNow;

        var item = new ItemPatrimonio
        {
            Id = Guid.NewGuid(),
            Descricao = descricao.Trim(),
            NumeroPatrimonio = string.IsNullOrWhiteSpace(numeroPatrimonio) ? null : numeroPatrimonio.Trim(),
            TipoControle = tipo,
            QuantidadeTotal = tipo == TipoControlePatrimonio.Unitario ? 1 : Math.Max(1, quantidadeTotal),
            FotoConteudo = fotoConteudo,
            FotoContentType = fotoContentType,
            LocalId = localId,
            Situacao = SituacaoPatrimonio.Ativo,
            Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim(),
            CriadoPorUsuarioId = usuarioId,
            DataCriacao = agora,
            AtualizadoPorUsuarioId = usuarioId,
            DataAtualizacao = agora
        };

        db.ItensPatrimonio.Add(item);
        await db.SaveChangesAsync();

        return ParaDto(item, 0);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = PapeisEscrita)]
    public async Task<ActionResult<ItemPatrimonioDto>> Atualizar(Guid id, ItemPatrimonioAtualizarRequest request)
    {
        var item = await db.ItensPatrimonio.Include(i => i.Local).FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        var quantidadeTotal = item.TipoControle == TipoControlePatrimonio.Unitario ? 1 : Math.Max(1, request.QuantidadeTotal);

        var emprestado = await db.EmprestimosPatrimonio
            .Where(e => e.ItemPatrimonioId == id && e.DataHoraDevolucao == null)
            .SumAsync(e => (int?)e.Quantidade) ?? 0;

        if (quantidadeTotal < emprestado)
        {
            return BadRequest(new { message = $"Não é possível reduzir a quantidade abaixo do que já está emprestado ({emprestado})." });
        }

        item.Descricao = request.Descricao.Trim();
        item.NumeroPatrimonio = string.IsNullOrWhiteSpace(request.NumeroPatrimonio) ? null : request.NumeroPatrimonio.Trim();
        item.QuantidadeTotal = quantidadeTotal;
        item.LocalId = request.LocalId;
        item.Observacao = string.IsNullOrWhiteSpace(request.Observacao) ? null : request.Observacao.Trim();
        item.AtualizadoPorUsuarioId = ObterUsuarioIdAtual();
        item.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();

        await db.Entry(item).Reference(i => i.Local).LoadAsync();
        return ParaDto(item, emprestado);
    }

    [HttpPost("{id:guid}/foto")]
    [Authorize(Roles = PapeisEscrita)]
    [RequestSizeLimit(ValidacaoImagem.TamanhoMaximoBytes)]
    public async Task<ActionResult<ItemPatrimonioDto>> SubstituirFoto(Guid id, IFormFile arquivo)
    {
        var item = await db.ItensPatrimonio.Include(i => i.Local).FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        var erroValidacao = await ValidacaoImagem.ValidarAsync(arquivo);
        if (erroValidacao is not null)
        {
            return BadRequest(new { message = erroValidacao });
        }

        using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream);

        item.FotoConteudo = stream.ToArray();
        item.FotoContentType = arquivo.ContentType;
        item.AtualizadoPorUsuarioId = ObterUsuarioIdAtual();
        item.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();

        var emprestado = await db.EmprestimosPatrimonio
            .Where(e => e.ItemPatrimonioId == id && e.DataHoraDevolucao == null)
            .SumAsync(e => (int?)e.Quantidade) ?? 0;
        return ParaDto(item, emprestado);
    }

    [HttpPut("{id:guid}/situacao")]
    [Authorize(Roles = PapeisEscrita)]
    public async Task<ActionResult<ItemPatrimonioDto>> AlterarSituacao(Guid id, ItemPatrimonioSituacaoRequest request)
    {
        var item = await db.ItensPatrimonio.Include(i => i.Local).FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        if (item.Situacao == SituacaoPatrimonio.Baixado)
        {
            return BadRequest(new { message = "Um item baixado não pode ser reativado ou inativado." });
        }

        if (!request.Ativo && string.IsNullOrWhiteSpace(request.Observacao))
        {
            return BadRequest(new { message = "Informe uma observação explicando o motivo da inativação." });
        }

        item.Situacao = request.Ativo ? SituacaoPatrimonio.Ativo : SituacaoPatrimonio.Inativo;
        item.Observacao = string.IsNullOrWhiteSpace(request.Observacao) ? item.Observacao : request.Observacao.Trim();
        item.AtualizadoPorUsuarioId = ObterUsuarioIdAtual();
        item.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();

        var emprestado = await db.EmprestimosPatrimonio
            .Where(e => e.ItemPatrimonioId == id && e.DataHoraDevolucao == null)
            .SumAsync(e => (int?)e.Quantidade) ?? 0;
        return ParaDto(item, emprestado);
    }

    [HttpPost("{id:guid}/baixa")]
    [Authorize(Roles = PapeisEscrita)]
    [RequestSizeLimit(ValidacaoImagem.TamanhoMaximoBytes)]
    public async Task<ActionResult<ItemPatrimonioDto>> DarBaixa(Guid id, IFormFile fotoDefeito, [FromForm] string observacaoBaixa)
    {
        var item = await db.ItensPatrimonio.Include(i => i.Local).FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        if (item.Situacao == SituacaoPatrimonio.Baixado)
        {
            return BadRequest(new { message = "Este item já foi baixado." });
        }

        if (string.IsNullOrWhiteSpace(observacaoBaixa))
        {
            return BadRequest(new { message = "Informe uma observação com as informações do descarte." });
        }

        var temEmprestimoAtivo = await db.EmprestimosPatrimonio
            .AnyAsync(e => e.ItemPatrimonioId == id && e.DataHoraDevolucao == null);
        if (temEmprestimoAtivo)
        {
            return BadRequest(new { message = "Devolva o item antes de dar baixa." });
        }

        var erroValidacao = await ValidacaoImagem.ValidarAsync(fotoDefeito);
        if (erroValidacao is not null)
        {
            return BadRequest(new { message = erroValidacao });
        }

        using var stream = new MemoryStream();
        await fotoDefeito.CopyToAsync(stream);

        item.Situacao = SituacaoPatrimonio.Baixado;
        item.FotoDefeitoConteudo = stream.ToArray();
        item.FotoDefeitoContentType = fotoDefeito.ContentType;
        item.ObservacaoBaixa = observacaoBaixa.Trim();
        item.DataBaixa = DateTimeOffset.UtcNow;
        item.AtualizadoPorUsuarioId = ObterUsuarioIdAtual();
        item.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return ParaDto(item, 0);
    }

    [HttpPost("{id:guid}/emprestimos")]
    [Authorize(Roles = PapeisEscrita)]
    public async Task<ActionResult<EmprestimoPatrimonioDto>> Emprestar(Guid id, ItemPatrimonioEmprestimoRequest request)
    {
        var item = await db.ItensPatrimonio.FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        if (item.Situacao != SituacaoPatrimonio.Ativo)
        {
            return BadRequest(new { message = "Só é possível emprestar um item ativo." });
        }

        var emprestado = await db.EmprestimosPatrimonio
            .Where(e => e.ItemPatrimonioId == id && e.DataHoraDevolucao == null)
            .SumAsync(e => (int?)e.Quantidade) ?? 0;

        var quantidade = item.TipoControle == TipoControlePatrimonio.Unitario ? 1 : Math.Max(1, request.Quantidade);

        if (item.TipoControle == TipoControlePatrimonio.Unitario && emprestado > 0)
        {
            return BadRequest(new { message = "Este item já está emprestado." });
        }

        var disponivel = item.QuantidadeTotal - emprestado;
        if (quantidade > disponivel)
        {
            return BadRequest(new { message = $"Só há {disponivel} unidade(s) disponível(is) para empréstimo." });
        }

        var emprestimo = new EmprestimoPatrimonio
        {
            Id = Guid.NewGuid(),
            ItemPatrimonioId = id,
            Quantidade = quantidade,
            QuemRetirou = request.QuemRetirou.Trim(),
            ObservacaoRetirada = request.Observacao.Trim(),
            DataHoraRetirada = DateTimeOffset.UtcNow,
            CriadoPorUsuarioId = ObterUsuarioIdAtual()
        };

        db.EmprestimosPatrimonio.Add(emprestimo);
        await db.SaveChangesAsync();

        return ParaDto(emprestimo);
    }

    [HttpPut("{id:guid}/emprestimos/{emprestimoId:guid}/devolucao")]
    [Authorize(Roles = PapeisEscrita)]
    public async Task<ActionResult<EmprestimoPatrimonioDto>> Devolver(Guid id, Guid emprestimoId, ItemPatrimonioDevolucaoRequest request)
    {
        var emprestimo = await db.EmprestimosPatrimonio
            .FirstOrDefaultAsync(e => e.Id == emprestimoId && e.ItemPatrimonioId == id);
        if (emprestimo is null)
        {
            return NotFound();
        }

        if (emprestimo.DataHoraDevolucao is not null)
        {
            return BadRequest(new { message = "Este empréstimo já foi devolvido." });
        }

        emprestimo.QuemDevolveu = request.QuemDevolveu.Trim();
        emprestimo.ObservacaoDevolucao = request.Observacao.Trim();
        emprestimo.DataHoraDevolucao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();

        return ParaDto(emprestimo);
    }

    private async Task<Dictionary<Guid, int>> ObterQuantidadeEmprestadaPorItemAsync() =>
        await db.EmprestimosPatrimonio
            .Where(e => e.DataHoraDevolucao == null)
            .GroupBy(e => e.ItemPatrimonioId)
            .Select(g => new { ItemId = g.Key, Quantidade = g.Sum(e => e.Quantidade) })
            .ToDictionaryAsync(x => x.ItemId, x => x.Quantidade);

    private void DefinirCacheImutavel()
    {
        Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(365),
            Extensions = { new NameValueHeaderValue("immutable") }
        };
    }

    private ItemPatrimonioDto ParaDto(ItemPatrimonio item, int quantidadeEmprestada) => new(
        item.Id,
        item.Descricao,
        item.NumeroPatrimonio,
        item.TipoControle.ToApiString(),
        item.QuantidadeTotal,
        quantidadeEmprestada,
        item.QuantidadeTotal - quantidadeEmprestada,
        item.FotoConteudo is not null ? ImagemUrlHelper.Construir(Request, $"admin/patrimonio/{item.Id}/foto", item.DataAtualizacao) : null,
        item.LocalId,
        item.Local?.Nome,
        item.Situacao.ToApiString(),
        item.Observacao,
        item.FotoDefeitoConteudo is not null ? ImagemUrlHelper.Construir(Request, $"admin/patrimonio/{item.Id}/foto-defeito", item.DataAtualizacao) : null,
        item.ObservacaoBaixa,
        item.DataBaixa,
        item.DataAtualizacao);

    private static EmprestimoPatrimonioDto ParaDto(EmprestimoPatrimonio emprestimo) => new(
        emprestimo.Id,
        emprestimo.Quantidade,
        emprestimo.QuemRetirou,
        emprestimo.ObservacaoRetirada,
        emprestimo.DataHoraRetirada,
        emprestimo.QuemDevolveu,
        emprestimo.ObservacaoDevolucao,
        emprestimo.DataHoraDevolucao);

    private Guid ObterUsuarioIdAtual() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
