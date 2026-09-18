using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) do painel de fotos.</summary>
[ApiController]
[Route("api/galeria")]
public class GaleriaController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FotoGaleriaDto>>> Listar()
    {
        var fotos = await db.FotosGaleria.OrderBy(f => f.Ordem).ToListAsync();
        return fotos.Select(ParaDto).ToList();
    }

    /// <summary>Serve o conteúdo binário. A URL é versionada por ?v=, então cache agressivo é seguro.</summary>
    [HttpGet("{id:guid}/arquivo")]
    public async Task<IActionResult> Arquivo(Guid id)
    {
        var foto = await db.FotosGaleria.FindAsync(id);
        if (foto is null)
        {
            return NotFound();
        }

        Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(365),
            Extensions = { new NameValueHeaderValue("immutable") }
        };

        return File(foto.Conteudo, foto.ContentType);
    }

    private FotoGaleriaDto ParaDto(Data.Entities.FotoGaleria foto) => new(
        foto.Id,
        ImagemUrlHelper.Construir(Request, $"galeria/{foto.Id}", foto.DataAtualizacao),
        foto.Legenda,
        foto.Ordem,
        foto.DataAtualizacao);
}
