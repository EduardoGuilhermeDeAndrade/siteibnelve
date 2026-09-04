using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) dos locais ativos.</summary>
[ApiController]
[Route("api/locais")]
public class LocaisController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LocalDto>>> Listar()
    {
        var locais = await db.Locais
            .Where(l => l.Ativo)
            .OrderBy(l => l.Ordem)
            .ThenBy(l => l.Nome)
            .ToListAsync();

        return locais.Select(l => new LocalDto(
            l.Id, l.Nome, l.Tipo, l.Endereco, l.Bairro, l.Cidade,
            l.Estado, l.Cep, l.GoogleMapsUrl, l.Latitude, l.Longitude, l.Ativo, l.Ordem)).ToList();
    }
}
