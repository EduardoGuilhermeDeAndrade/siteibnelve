using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) da Agenda — só eventos PUBLICO + PUBLICADO.</summary>
[ApiController]
[Route("api/eventos")]
public class EventosController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventoPublicoDto>>> Listar([FromQuery] int? limite)
    {
        var agora = DateTimeOffset.UtcNow;

        var eventos = await db.Eventos
            .Include(e => e.Local)
            .Where(e => e.Visibilidade == VisibilidadeEvento.Publico && e.Status == StatusEvento.Publicado)
            .ToListAsync();

        var comOcorrencia = eventos
            .Select(e => (Evento: e, Data: RecorrenciaCalculator.ProximaOcorrencia(e, agora)))
            .Where(x => x.Data is not null)
            .OrderBy(x => x.Data)
            .Select(x => ParaDto(x.Evento, x.Data!.Value));

        if (limite is > 0)
        {
            comOcorrencia = comOcorrencia.Take(limite.Value);
        }

        return comOcorrencia.ToList();
    }

    private static EventoPublicoDto ParaDto(Evento evento, DateTimeOffset data) => new(
        evento.Id,
        evento.Titulo,
        evento.Descricao,
        evento.Categoria.ToApiString(),
        data,
        evento.Local?.Nome ?? evento.LocalTexto,
        evento.RecorrenciaSemanal);
}
