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
[Route("api/admin/conteudo-texto")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class ConteudoTextoController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConteudoTextoDto>>> Listar()
    {
        var itens = await db.ConteudosTexto.OrderBy(c => c.Chave).ToListAsync();
        return itens.Select(ParaDto).ToList();
    }

    [HttpPut("{chave}")]
    public async Task<ActionResult<ConteudoTextoDto>> Upsert(string chave, ConteudoTextoUpsertRequest request)
    {
        var item = await db.ConteudosTexto.FirstOrDefaultAsync(c => c.Chave == chave);
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (item is null)
        {
            item = new ConteudoTexto { Id = Guid.NewGuid(), Chave = chave };
            db.ConteudosTexto.Add(item);
        }

        item.Titulo = request.Titulo;
        item.Conteudo = request.Conteudo;
        item.AtualizadoPorUsuarioId = usuarioId;
        item.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return ParaDto(item);
    }

    private static ConteudoTextoDto ParaDto(ConteudoTexto item) =>
        new(item.Chave, item.Titulo, item.Conteudo, item.DataAtualizacao);
}
