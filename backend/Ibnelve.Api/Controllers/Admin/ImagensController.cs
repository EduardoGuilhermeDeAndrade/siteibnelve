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
[Route("api/admin/imagens")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class ImagensController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ImagemSiteDto>>> Listar()
    {
        var imagens = await db.ImagensSite.OrderBy(i => i.Chave).ToListAsync();
        return imagens.Select(ParaDto).ToList();
    }

    [HttpPost("{chave}")]
    [RequestSizeLimit(ValidacaoImagem.TamanhoMaximoBytes)]
    public async Task<ActionResult<ImagemSiteDto>> Upload(string chave, IFormFile arquivo)
    {
        var erroValidacao = await ValidacaoImagem.ValidarAsync(arquivo);
        if (erroValidacao is not null)
        {
            return BadRequest(new { message = erroValidacao });
        }

        using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream);

        var imagem = await db.ImagensSite.FirstOrDefaultAsync(i => i.Chave == chave);
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (imagem is null)
        {
            imagem = new ImagemSite { Id = Guid.NewGuid(), Chave = chave };
            db.ImagensSite.Add(imagem);
        }

        imagem.Conteudo = stream.ToArray();
        imagem.ContentType = arquivo.ContentType;
        imagem.AtualizadoPorUsuarioId = usuarioId;
        imagem.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return ParaDto(imagem);
    }

    private ImagemSiteDto ParaDto(ImagemSite imagem) =>
        new(imagem.Chave, ImagemUrlHelper.Construir(Request, $"imagens-site/{imagem.Chave}", imagem.DataAtualizacao), imagem.DataAtualizacao);
}
