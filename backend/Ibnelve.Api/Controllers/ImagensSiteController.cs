using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        return imagens.Select(i => new ImagemSiteDto(i.Chave, i.Url, i.DataAtualizacao)).ToList();
    }
}
