using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Domain;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/eventos")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class EventosController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventoDto>>> Listar()
    {
        var eventos = await db.Eventos
            .Include(e => e.Local)
            .OrderBy(e => e.DataHoraInicio)
            .ToListAsync();

        return eventos.Select(ParaDto).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventoDto>> ObterPorId(Guid id)
    {
        var evento = await db.Eventos.Include(e => e.Local).FirstOrDefaultAsync(e => e.Id == id);
        return evento is null ? NotFound() : ParaDto(evento);
    }

    [HttpPost]
    public async Task<ActionResult<EventoDto>> Criar(EventoUpsertRequest request)
    {
        if (!ValidarLocal(request, out var problema))
        {
            return BadRequest(new { message = problema });
        }

        var evento = new Evento
        {
            Id = Guid.NewGuid(),
            CriadoPorUsuarioId = ObterUsuarioIdAtual(),
            DataCriacao = DateTimeOffset.UtcNow
        };
        AplicarRequest(evento, request);

        db.Eventos.Add(evento);
        await db.SaveChangesAsync();

        await db.Entry(evento).Reference(e => e.Local).LoadAsync();
        return CreatedAtAction(nameof(ObterPorId), new { id = evento.Id }, ParaDto(evento));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EventoDto>> Atualizar(Guid id, EventoUpsertRequest request)
    {
        var evento = await db.Eventos.FirstOrDefaultAsync(e => e.Id == id);
        if (evento is null)
        {
            return NotFound();
        }

        if (!ValidarLocal(request, out var problema))
        {
            return BadRequest(new { message = problema });
        }

        AplicarRequest(evento, request);
        evento.AtualizadoPorUsuarioId = ObterUsuarioIdAtual();
        evento.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        await db.Entry(evento).Reference(e => e.Local).LoadAsync();
        return ParaDto(evento);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var evento = await db.Eventos.FirstOrDefaultAsync(e => e.Id == id);
        if (evento is null)
        {
            return NotFound();
        }

        db.Eventos.Remove(evento);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private bool ValidarLocal(EventoUpsertRequest request, out string problema)
    {
        if (request.LocalId is null && string.IsNullOrWhiteSpace(request.LocalTexto))
        {
            problema = "Informe um Local cadastrado ou um local textual.";
            return false;
        }

        if (request.RecorrenciaSemanal && request.DiaSemana is null)
        {
            problema = "Evento semanal precisa de um dia da semana.";
            return false;
        }

        problema = string.Empty;
        return true;
    }

    private static void AplicarRequest(Evento evento, EventoUpsertRequest request)
    {
        evento.Titulo = request.Titulo;
        evento.Descricao = request.Descricao;
        evento.Categoria = EnumMappings.ParseCategoria(request.Categoria);
        evento.Visibilidade = EnumMappings.ParseVisibilidade(request.Visibilidade);
        evento.Status = EnumMappings.ParseStatus(request.Status);
        // Npgsql só aceita DateTimeOffset com Offset=0 em colunas "timestamp with time zone".
        evento.DataHoraInicio = request.DataHoraInicio.ToUniversalTime();
        evento.DataHoraFim = request.DataHoraFim?.ToUniversalTime();
        evento.LocalId = request.LocalId;
        evento.LocalTexto = request.LocalTexto;
        evento.RecorrenciaSemanal = request.RecorrenciaSemanal;
        evento.DiaSemana = request.RecorrenciaSemanal && request.DiaSemana is not null
            ? (DayOfWeek)request.DiaSemana.Value
            : null;
    }

    private static EventoDto ParaDto(Evento evento) => new(
        evento.Id,
        evento.Titulo,
        evento.Descricao,
        evento.Categoria.ToApiString(),
        evento.Visibilidade.ToApiString(),
        evento.Status.ToApiString(),
        evento.DataHoraInicio,
        evento.DataHoraFim,
        evento.LocalId,
        evento.Local?.Nome,
        evento.LocalTexto,
        evento.RecorrenciaSemanal,
        evento.DiaSemana is null ? null : (int)evento.DiaSemana.Value,
        evento.ImagemUrl);

    private Guid ObterUsuarioIdAtual() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
