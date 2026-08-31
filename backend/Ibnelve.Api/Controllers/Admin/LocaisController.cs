using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/locais")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class LocaisController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LocalDto>>> Listar()
    {
        var locais = await db.Locais.OrderBy(l => l.Ordem).ThenBy(l => l.Nome).ToListAsync();
        return locais.Select(ParaDto).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocalDto>> ObterPorId(Guid id)
    {
        var local = await db.Locais.FindAsync(id);
        return local is null ? NotFound() : ParaDto(local);
    }

    [HttpPost]
    public async Task<ActionResult<LocalDto>> Criar(LocalUpsertRequest request)
    {
        var local = new Local { Id = Guid.NewGuid() };
        AplicarRequest(local, request);

        db.Locais.Add(local);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(ObterPorId), new { id = local.Id }, ParaDto(local));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LocalDto>> Atualizar(Guid id, LocalUpsertRequest request)
    {
        var local = await db.Locais.FindAsync(id);
        if (local is null)
        {
            return NotFound();
        }

        AplicarRequest(local, request);
        await db.SaveChangesAsync();
        return ParaDto(local);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var local = await db.Locais.FindAsync(id);
        if (local is null)
        {
            return NotFound();
        }

        db.Locais.Remove(local);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void AplicarRequest(Local local, LocalUpsertRequest request)
    {
        local.Nome = request.Nome;
        local.Tipo = request.Tipo;
        local.Endereco = request.Endereco;
        local.Bairro = request.Bairro;
        local.Cidade = request.Cidade;
        local.Estado = request.Estado;
        local.Cep = request.Cep;
        local.GoogleMapsUrl = request.GoogleMapsUrl;
        local.Latitude = request.Latitude;
        local.Longitude = request.Longitude;
        local.Ativo = request.Ativo;
        local.Ordem = request.Ordem;
    }

    private static LocalDto ParaDto(Local local) => new(
        local.Id, local.Nome, local.Tipo, local.Endereco, local.Bairro, local.Cidade,
        local.Estado, local.Cep, local.GoogleMapsUrl, local.Latitude, local.Longitude,
        local.Ativo, local.Ordem);
}
