using System.Globalization;
using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Domain;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/series-evento")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class SeriesEventoController(IbnelveDbContext db) : ControllerBase
{
    private const int JanelaPadraoOcorrenciasDias = 180;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SerieEventoDto>>> Listar()
    {
        var series = await db.SeriesEvento
            .Include(s => s.Local)
            .OrderBy(s => s.Titulo)
            .ToListAsync();

        return series.Select(ParaDto).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SerieEventoDto>> ObterPorId(Guid id)
    {
        var serie = await db.SeriesEvento.Include(s => s.Local).FirstOrDefaultAsync(s => s.Id == id);
        return serie is null ? NotFound() : ParaDto(serie);
    }

    [HttpPost]
    public async Task<ActionResult<SerieEventoDto>> Criar(SerieEventoUpsertRequest request)
    {
        if (!ValidarRequest(request, out var problema))
        {
            return BadRequest(new { message = problema });
        }

        var serie = new SerieEvento
        {
            Id = Guid.NewGuid(),
            CriadoPorUsuarioId = ObterUsuarioIdAtual(),
            DataCriacao = DateTimeOffset.UtcNow
        };
        AplicarRequest(serie, request);

        db.SeriesEvento.Add(serie);
        await db.SaveChangesAsync();

        await db.Entry(serie).Reference(s => s.Local).LoadAsync();
        return CreatedAtAction(nameof(ObterPorId), new { id = serie.Id }, ParaDto(serie));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SerieEventoDto>> Atualizar(Guid id, SerieEventoUpsertRequest request)
    {
        var serie = await db.SeriesEvento.FirstOrDefaultAsync(s => s.Id == id);
        if (serie is null)
        {
            return NotFound();
        }

        if (!ValidarRequest(request, out var problema))
        {
            return BadRequest(new { message = problema });
        }

        AplicarRequest(serie, request);
        serie.AtualizadoPorUsuarioId = ObterUsuarioIdAtual();
        serie.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        await db.Entry(serie).Reference(s => s.Local).LoadAsync();
        return ParaDto(serie);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var serie = await db.SeriesEvento.FirstOrDefaultAsync(s => s.Id == id);
        if (serie is null)
        {
            return NotFound();
        }

        db.SeriesEvento.Remove(serie);
        await db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Prévia de ocorrências calculadas — usada tanto antes de confirmar mudanças na série
    /// quanto na lista expansível "Ver próximas ocorrências" da tela de Agenda.</summary>
    [HttpGet("{id:guid}/ocorrencias")]
    public async Task<ActionResult<IReadOnlyList<OcorrenciaDto>>> ObterOcorrencias(
        Guid id, [FromQuery] string? desde, [FromQuery] string? ate)
    {
        var serie = await db.SeriesEvento
            .Include(s => s.Local)
            .Include(s => s.Excecoes).ThenInclude(e => e.LocalSubstituto)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (serie is null)
        {
            return NotFound();
        }

        if (!TentarParseData(desde, out var dataDesde))
        {
            dataDesde = OcorrenciaCalculator.HojeLocal(DateTimeOffset.UtcNow);
        }
        if (!TentarParseData(ate, out var dataAte))
        {
            dataAte = dataDesde.AddDays(JanelaPadraoOcorrenciasDias);
        }

        var ocorrencias = OcorrenciaCalculator.CalcularOcorrencias(serie, dataDesde, dataAte);
        return ocorrencias.Select(ParaDto).ToList();
    }

    /// <summary>Cria/atualiza a exceção de uma ocorrência específica (escopo "somente esta ocorrência").</summary>
    [HttpPut("{id:guid}/ocorrencias/{data}")]
    public async Task<ActionResult<OcorrenciaDto>> SalvarExcecao(Guid id, string data, ExcecaoUpsertRequest request)
    {
        if (!TentarParseData(data, out var dataOriginal))
        {
            return BadRequest(new { message = "Data inválida. Use o formato AAAA-MM-DD." });
        }

        var serie = await db.SeriesEvento
            .Include(s => s.Local)
            .Include(s => s.Excecoes).ThenInclude(e => e.LocalSubstituto)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (serie is null)
        {
            return NotFound();
        }

        if (request.LocalSubstitutoId is not null
            && !await db.Locais.AnyAsync(l => l.Id == request.LocalSubstitutoId))
        {
            return BadRequest(new { message = "Local substituto informado não existe." });
        }

        var excecao = serie.Excecoes.FirstOrDefault(e => e.DataOriginal == dataOriginal);
        var usuarioId = ObterUsuarioIdAtual();

        if (excecao is null)
        {
            excecao = new ExcecaoEvento
            {
                Id = Guid.NewGuid(),
                SerieEventoId = serie.Id,
                DataOriginal = dataOriginal,
                CriadoPorUsuarioId = usuarioId,
                DataCriacao = DateTimeOffset.UtcNow
            };
            db.ExcecoesEvento.Add(excecao);
        }
        else
        {
            excecao.AtualizadoPorUsuarioId = usuarioId;
            excecao.DataAtualizacao = DateTimeOffset.UtcNow;
        }

        excecao.Cancelado = request.Cancelado;
        excecao.NovaData = request.NovaData;
        excecao.NovaHoraInicio = request.NovaHoraInicio;
        excecao.NovaHoraFim = request.NovaHoraFim;
        excecao.TituloSubstituto = request.TituloSubstituto;
        excecao.DescricaoSubstituta = request.DescricaoSubstituta;
        excecao.LocalSubstitutoId = request.LocalSubstitutoId;
        excecao.LocalTextoSubstituto = request.LocalTextoSubstituto;
        excecao.Motivo = request.Motivo;

        await db.SaveChangesAsync();

        if (excecao.LocalSubstitutoId is not null)
        {
            await db.Entry(excecao).Reference(e => e.LocalSubstituto).LoadAsync();
        }

        return ParaDto(OcorrenciaCalculator.CalcularOcorrencias(serie, dataOriginal, dataOriginal).Single());
    }

    /// <summary>Remove a exceção de uma data (a ocorrência volta a seguir o padrão da série).</summary>
    [HttpDelete("{id:guid}/ocorrencias/{data}")]
    public async Task<IActionResult> RemoverExcecao(Guid id, string data)
    {
        if (!TentarParseData(data, out var dataOriginal))
        {
            return BadRequest(new { message = "Data inválida. Use o formato AAAA-MM-DD." });
        }

        var excecao = await db.ExcecoesEvento
            .FirstOrDefaultAsync(e => e.SerieEventoId == id && e.DataOriginal == dataOriginal);
        if (excecao is null)
        {
            return NotFound();
        }

        db.ExcecoesEvento.Remove(excecao);
        await db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Escopo "esta e as próximas": encerra a série atual na véspera de apartirDe, cria uma
    /// nova série a partir dali com os campos do corpo, e migra as exceções futuras para a nova série.</summary>
    [HttpPost("{id:guid}/dividir")]
    public async Task<ActionResult<SerieEventoDto>> Dividir(
        Guid id, [FromQuery] string apartirDe, SerieEventoUpsertRequest request)
    {
        if (!TentarParseData(apartirDe, out var dataDivisao))
        {
            return BadRequest(new { message = "Data inválida. Use o formato AAAA-MM-DD." });
        }

        if (!ValidarRequest(request, out var problema))
        {
            return BadRequest(new { message = problema });
        }

        var serieAtual = await db.SeriesEvento
            .Include(s => s.Excecoes)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (serieAtual is null)
        {
            return NotFound();
        }

        if (dataDivisao <= serieAtual.DataInicioRecorrencia)
        {
            return BadRequest(new { message = "A divisão precisa começar depois do início da série atual." });
        }

        var usuarioId = ObterUsuarioIdAtual();
        var agora = DateTimeOffset.UtcNow;

        serieAtual.DataFimRecorrencia = dataDivisao.AddDays(-1);
        serieAtual.AtualizadoPorUsuarioId = usuarioId;
        serieAtual.DataAtualizacao = agora;

        var novaSerie = new SerieEvento
        {
            Id = Guid.NewGuid(),
            CriadoPorUsuarioId = usuarioId,
            DataCriacao = agora
        };
        AplicarRequest(novaSerie, request);
        novaSerie.DataInicioRecorrencia = dataDivisao;

        var excecoesParaMigrar = serieAtual.Excecoes.Where(e => e.DataOriginal >= dataDivisao).ToList();
        foreach (var excecao in excecoesParaMigrar)
        {
            excecao.SerieEventoId = novaSerie.Id;
        }

        db.SeriesEvento.Add(novaSerie);
        await db.SaveChangesAsync();

        await db.Entry(novaSerie).Reference(s => s.Local).LoadAsync();
        return CreatedAtAction(nameof(ObterPorId), new { id = novaSerie.Id }, ParaDto(novaSerie));
    }

    private static bool TentarParseData(string? valor, out DateOnly data)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            data = default;
            return false;
        }

        return DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data);
    }

    private bool ValidarRequest(SerieEventoUpsertRequest request, out string problema)
    {
        if (request.LocalId is null && string.IsNullOrWhiteSpace(request.LocalTexto))
        {
            problema = "Informe um Local cadastrado ou um local textual.";
            return false;
        }

        var tipo = EnumMappings.ParseTipoRecorrencia(request.TipoRecorrencia);

        switch (tipo)
        {
            case TipoRecorrencia.Semanal when request.DiaSemana is null:
                problema = "Recorrência semanal precisa de um dia da semana.";
                return false;

            case TipoRecorrencia.ACadaNSemanas when request.DiaSemana is null || request.Intervalo is null or < 1:
                problema = "Recorrência a cada N semanas precisa de um dia da semana e de um intervalo válido.";
                return false;

            case TipoRecorrencia.MensalPorDia when request.DiaMes is null:
                problema = "Recorrência mensal por dia precisa do dia do mês.";
                return false;

            case TipoRecorrencia.MensalPorPosicao when request.DiaSemana is null || request.PosicaoNoMes is null:
                problema = "Recorrência mensal por posição precisa do dia da semana e da posição no mês.";
                return false;
        }

        problema = string.Empty;
        return true;
    }

    private static void AplicarRequest(SerieEvento serie, SerieEventoUpsertRequest request)
    {
        var tipo = EnumMappings.ParseTipoRecorrencia(request.TipoRecorrencia);

        serie.Titulo = request.Titulo;
        serie.Descricao = request.Descricao;
        serie.Categoria = EnumMappings.ParseCategoria(request.Categoria);
        serie.Visibilidade = EnumMappings.ParseVisibilidade(request.Visibilidade);
        serie.Status = EnumMappings.ParseStatus(request.Status);
        serie.TipoRecorrencia = tipo;
        serie.Intervalo = tipo == TipoRecorrencia.ACadaNSemanas ? request.Intervalo : null;
        serie.DiaSemana = tipo is TipoRecorrencia.Semanal or TipoRecorrencia.ACadaNSemanas or TipoRecorrencia.MensalPorPosicao
            ? (DayOfWeek?)request.DiaSemana
            : null;
        serie.DiaMes = tipo == TipoRecorrencia.MensalPorDia ? request.DiaMes : null;
        serie.PosicaoNoMes = tipo == TipoRecorrencia.MensalPorPosicao ? request.PosicaoNoMes : null;
        serie.HoraInicio = request.HoraInicio;
        serie.HoraFim = request.HoraFim;
        serie.DataInicioRecorrencia = request.DataInicioRecorrencia;
        serie.DataFimRecorrencia = request.DataFimRecorrencia;
        serie.LocalId = request.LocalId;
        serie.LocalTexto = request.LocalTexto;
        serie.ImagemUrl = request.ImagemUrl;
        serie.Destaque = request.Destaque;
        serie.Ativo = request.Ativo;
    }

    private static SerieEventoDto ParaDto(SerieEvento serie) => new(
        serie.Id,
        serie.Titulo,
        serie.Descricao,
        serie.Categoria.ToApiString(),
        serie.Visibilidade.ToApiString(),
        serie.Status.ToApiString(),
        serie.TipoRecorrencia.ToApiString(),
        serie.Intervalo,
        serie.DiaSemana is null ? null : (int)serie.DiaSemana.Value,
        serie.DiaMes,
        serie.PosicaoNoMes,
        serie.HoraInicio,
        serie.HoraFim,
        serie.DataInicioRecorrencia,
        serie.DataFimRecorrencia,
        serie.LocalId,
        serie.Local?.Nome,
        serie.LocalTexto,
        serie.ImagemUrl,
        serie.Destaque,
        serie.Ativo);

    private static OcorrenciaDto ParaDto(Ocorrencia ocorrencia) => new(
        ocorrencia.DataOriginal,
        ocorrencia.DataHoraInicio,
        ocorrencia.DataHoraFim,
        ocorrencia.Titulo,
        ocorrencia.Descricao,
        ocorrencia.LocalId,
        ocorrencia.LocalNome,
        ocorrencia.LocalTexto,
        ocorrencia.Cancelada,
        ocorrencia.TemExcecao,
        ocorrencia.Motivo);

    private Guid ObterUsuarioIdAtual() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
