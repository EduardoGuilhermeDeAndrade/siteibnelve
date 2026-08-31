using Ibnelve.Api.Domain;

namespace Ibnelve.Api.Data.Entities;

/// <summary>
/// Evento simples do v1 do Portal Administrativo: cobre eventos avulsos (data única)
/// e eventos semanais fixos (mesmo dia da semana/horário). O motor de recorrência
/// completo do CLAUDE.md (Fase 6: a cada N semanas, mensal por posição, exceções,
/// divisão de série) substituirá/expandirá esta entidade mais adiante.
/// </summary>
public class Evento
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public CategoriaEvento Categoria { get; set; }
    public VisibilidadeEvento Visibilidade { get; set; }
    public StatusEvento Status { get; set; }

    /// <summary>Data/hora do evento avulso, ou primeira ocorrência de referência se semanal.</summary>
    public DateTimeOffset DataHoraInicio { get; set; }
    public DateTimeOffset? DataHoraFim { get; set; }

    public Guid? LocalId { get; set; }
    public Local? Local { get; set; }
    public string? LocalTexto { get; set; }

    /// <summary>false = evento avulso (data única); true = repete toda semana no mesmo dia/horário.</summary>
    public bool RecorrenciaSemanal { get; set; }
    public DayOfWeek? DiaSemana { get; set; }

    public string? ImagemUrl { get; set; }

    public Guid CriadoPorUsuarioId { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
    public Guid? AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset? DataAtualizacao { get; set; }
}
