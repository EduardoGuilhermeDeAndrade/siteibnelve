using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/ministerios")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class MinisteriosController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MinisterioDto>>> Listar()
    {
        var ministerios = await db.Ministerios.OrderBy(m => m.Ordem).ThenBy(m => m.Nome).ToListAsync();
        return ministerios.Select(ParaDto).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MinisterioDto>> ObterPorId(Guid id)
    {
        var ministerio = await db.Ministerios.FindAsync(id);
        return ministerio is null ? NotFound() : ParaDto(ministerio);
    }

    [HttpPost]
    public async Task<ActionResult<MinisterioDto>> Criar(MinisterioUpsertRequest request)
    {
        var ministerio = new Ministerio { Id = Guid.NewGuid() };
        AplicarRequest(ministerio, request);

        db.Ministerios.Add(ministerio);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(ObterPorId), new { id = ministerio.Id }, ParaDto(ministerio));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MinisterioDto>> Atualizar(Guid id, MinisterioUpsertRequest request)
    {
        var ministerio = await db.Ministerios.FindAsync(id);
        if (ministerio is null)
        {
            return NotFound();
        }

        AplicarRequest(ministerio, request);
        await db.SaveChangesAsync();
        return ParaDto(ministerio);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var ministerio = await db.Ministerios.FindAsync(id);
        if (ministerio is null)
        {
            return NotFound();
        }

        db.Ministerios.Remove(ministerio);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void AplicarRequest(Ministerio ministerio, MinisterioUpsertRequest request)
    {
        ministerio.Nome = request.Nome;
        ministerio.Lideres = request.Lideres;
        ministerio.Descricao = request.Descricao;
        ministerio.ImagemUrl = request.ImagemUrl;
        ministerio.Ativo = request.Ativo;
        ministerio.Ordem = request.Ordem;
    }

    private static MinisterioDto ParaDto(Ministerio ministerio) => new(
        ministerio.Id, ministerio.Nome, ministerio.Lideres, ministerio.Descricao,
        ministerio.ImagemUrl, ministerio.Ativo, ministerio.Ordem);
}
