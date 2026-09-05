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

        if (!await TemAssinaturaValidaAsync(arquivo, arquivo.ContentType))
        {
            return BadRequest(new { message = "O conteúdo do arquivo não corresponde ao formato informado." });
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

    /// <summary>Confere a assinatura binária (magic bytes) do arquivo — o Content-Type enviado pelo
    /// cliente é apenas um header HTTP e pode ser forjado, então não é suficiente sozinho.</summary>
    private static async Task<bool> TemAssinaturaValidaAsync(IFormFile arquivo, string contentType)
    {
        var cabecalho = new byte[12];
        await using (var stream = arquivo.OpenReadStream())
        {
            var lidos = await stream.ReadAsync(cabecalho.AsMemory(0, (int)Math.Min(cabecalho.Length, arquivo.Length)));
            if (lidos < cabecalho.Length)
            {
                Array.Clear(cabecalho, lidos, cabecalho.Length - lidos);
            }
        }

        return contentType switch
        {
            "image/jpeg" => cabecalho[0] == 0xFF && cabecalho[1] == 0xD8 && cabecalho[2] == 0xFF,
            "image/png" => cabecalho[0] == 0x89 && cabecalho[1] == 0x50 && cabecalho[2] == 0x4E && cabecalho[3] == 0x47
                            && cabecalho[4] == 0x0D && cabecalho[5] == 0x0A && cabecalho[6] == 0x1A && cabecalho[7] == 0x0A,
            "image/webp" => cabecalho[0] == 0x52 && cabecalho[1] == 0x49 && cabecalho[2] == 0x46 && cabecalho[3] == 0x46
                             && cabecalho[8] == 0x57 && cabecalho[9] == 0x45 && cabecalho[10] == 0x42 && cabecalho[11] == 0x50,
            _ => false
        };
    }

    private static ImagemSiteDto ParaDto(ImagemSite imagem) => new(imagem.Chave, imagem.Url, imagem.DataAtualizacao);
}
