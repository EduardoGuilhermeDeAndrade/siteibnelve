using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Services;
using Ibnelve.Api.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/imagens")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class ImagensController(IbnelveDbContext db, IImagemStorageService storage) : ControllerBase
{
    private static readonly Dictionary<string, string> TiposPermitidos = new()
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };

    private const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5 MB

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ImagemSiteDto>>> Listar()
    {
        var imagens = await db.ImagensSite.OrderBy(i => i.Chave).ToListAsync();
        return imagens.Select(ParaDto).ToList();
    }

    [HttpPost("{chave}")]
    [RequestSizeLimit(TamanhoMaximoBytes)]
    public async Task<ActionResult<ImagemSiteDto>> Upload(string chave, IFormFile arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
        {
            return BadRequest(new { message = "Envie um arquivo de imagem." });
        }

        if (arquivo.Length > TamanhoMaximoBytes)
        {
            return BadRequest(new { message = "Arquivo maior que o limite de 5 MB." });
        }

        if (!TiposPermitidos.TryGetValue(arquivo.ContentType, out var extensao))
        {
            return BadRequest(new { message = "Formato não suportado. Envie JPEG, PNG ou WebP." });
        }

        await storage.GarantirBucketAsync();

        await using var stream = arquivo.OpenReadStream();
        var url = await storage.SalvarAsync(chave, stream, arquivo.ContentType, extensao);

        var imagem = await db.ImagensSite.FirstOrDefaultAsync(i => i.Chave == chave);
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (imagem is null)
        {
            imagem = new ImagemSite { Id = Guid.NewGuid(), Chave = chave };
            db.ImagensSite.Add(imagem);
        }

        imagem.Url = url;
        imagem.AtualizadoPorUsuarioId = usuarioId;
        imagem.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return ParaDto(imagem);
    }

    private static ImagemSiteDto ParaDto(ImagemSite imagem) => new(imagem.Chave, imagem.Url, imagem.DataAtualizacao);
}
