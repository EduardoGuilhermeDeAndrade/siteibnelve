using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/galeria")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class GaleriaController(IbnelveDbContext db) : ControllerBase
{
    private const int LimiteFotos = 10;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FotoGaleriaDto>>> Listar()
    {
        var fotos = await db.FotosGaleria.OrderBy(f => f.Ordem).ToListAsync();
        return fotos.Select(ParaDto).ToList();
    }

    [HttpPost]
    [RequestSizeLimit(ValidacaoImagem.TamanhoMaximoBytes)]
    public async Task<ActionResult<FotoGaleriaDto>> Adicionar(IFormFile arquivo, [FromForm] string? legenda)
    {
        var erroValidacao = await ValidacaoImagem.ValidarAsync(arquivo);
        if (erroValidacao is not null)
        {
            return BadRequest(new { message = erroValidacao });
        }

        var totalAtual = await db.FotosGaleria.CountAsync();
        if (totalAtual >= LimiteFotos)
        {
            return BadRequest(new { message = $"Limite de {LimiteFotos} fotos atingido. Exclua alguma para adicionar outra." });
        }

        using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream);

        var maiorOrdem = await db.FotosGaleria.Select(f => (int?)f.Ordem).MaxAsync() ?? 0;
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var agora = DateTimeOffset.UtcNow;

        var foto = new FotoGaleria
        {
            Id = Guid.NewGuid(),
            Conteudo = stream.ToArray(),
            ContentType = arquivo.ContentType,
            Legenda = string.IsNullOrWhiteSpace(legenda) ? null : legenda.Trim(),
            Ordem = maiorOrdem + 1,
            AtualizadoPorUsuarioId = usuarioId,
            DataCriacao = agora,
            DataAtualizacao = agora
        };

        db.FotosGaleria.Add(foto);
        await db.SaveChangesAsync();
        return ParaDto(foto);
    }

    [HttpPost("{id:guid}/substituir")]
    [RequestSizeLimit(ValidacaoImagem.TamanhoMaximoBytes)]
    public async Task<ActionResult<FotoGaleriaDto>> Substituir(Guid id, IFormFile arquivo)
    {
        var foto = await db.FotosGaleria.FindAsync(id);
        if (foto is null)
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

        foto.Conteudo = stream.ToArray();
        foto.ContentType = arquivo.ContentType;
        foto.AtualizadoPorUsuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        foto.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return ParaDto(foto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FotoGaleriaDto>> AtualizarMetadados(Guid id, FotoGaleriaAtualizarRequest request)
    {
        var foto = await db.FotosGaleria.FindAsync(id);
        if (foto is null)
        {
            return NotFound();
        }

        foto.Legenda = string.IsNullOrWhiteSpace(request.Legenda) ? null : request.Legenda.Trim();
        foto.Ordem = request.Ordem;
        foto.AtualizadoPorUsuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        foto.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return ParaDto(foto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var foto = await db.FotosGaleria.FindAsync(id);
        if (foto is null)
        {
            return NotFound();
        }

        db.FotosGaleria.Remove(foto);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private FotoGaleriaDto ParaDto(FotoGaleria foto) => new(
        foto.Id,
        ImagemUrlHelper.Construir(Request, $"galeria/{foto.Id}", foto.DataAtualizacao),
        foto.Legenda,
        foto.Ordem,
        foto.DataAtualizacao);
}
