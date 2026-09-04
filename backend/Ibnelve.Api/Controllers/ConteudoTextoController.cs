using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) dos textos institucionais.</summary>
[ApiController]
[Route("api/conteudo-texto")]
public class ConteudoTextoController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConteudoTextoDto>>> Listar()
    {
        var itens = await db.ConteudosTexto.OrderBy(c => c.Chave).ToListAsync();
        return itens.Select(i => new ConteudoTextoDto(i.Chave, i.Titulo, i.Conteudo, i.DataAtualizacao)).ToList();
    }
}
