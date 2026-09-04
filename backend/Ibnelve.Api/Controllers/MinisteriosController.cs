using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) dos ministérios ativos.</summary>
[ApiController]
[Route("api/ministerios")]
public class MinisteriosController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MinisterioDto>>> Listar()
    {
        var ministerios = await db.Ministerios
            .Where(m => m.Ativo)
            .OrderBy(m => m.Ordem)
            .ThenBy(m => m.Nome)
            .ToListAsync();

        return ministerios.Select(m => new MinisterioDto(
            m.Id, m.Nome, m.Lideres, m.Descricao, m.ImagemUrl, m.Ativo, m.Ordem)).ToList();
    }
}
