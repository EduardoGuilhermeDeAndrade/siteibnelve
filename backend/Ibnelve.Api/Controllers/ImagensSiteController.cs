using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) das imagens geridas pelo site.</summary>
[ApiController]
[Route("api/imagens-site")]
public class ImagensSiteController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ImagemSiteDto>>> Listar()
    {
        var imagens = await db.ImagensSite.OrderBy(i => i.Chave).ToListAsync();
        return imagens
            .Select(i => new ImagemSiteDto(i.Chave, ImagemUrlHelper.Construir(Request, i.Chave, i.DataAtualizacao), i.DataAtualizacao))
            .ToList();
    }

    /// <summary>Serve o conteúdo binário. A URL é versionada por ?v=, então cache agressivo é seguro.</summary>
    [HttpGet("{chave}/arquivo")]
    public async Task<IActionResult> Arquivo(string chave)
    {
        var imagem = await db.ImagensSite.FirstOrDefaultAsync(i => i.Chave == chave);
        if (imagem is null)
        {
            return NotFound();
        }

        Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(365),
            Extensions = { new NameValueHeaderValue("immutable") }
        };

        return File(imagem.Conteudo, imagem.ContentType);
    }
}
