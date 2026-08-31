using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record EventoDto(
    Guid Id,
    string Titulo,
    string? Descricao,
    string Categoria,
    string Visibilidade,
    string Status,
    DateTimeOffset DataHoraInicio,
    DateTimeOffset? DataHoraFim,
    Guid? LocalId,
    string? LocalNome,
    string? LocalTexto,
    bool RecorrenciaSemanal,
    int? DiaSemana,
    string? ImagemUrl
);

public record EventoUpsertRequest(
    [Required, MaxLength(200)] string Titulo,
    string? Descricao,
    [Required] string Categoria,
    [Required] string Visibilidade,
    [Required] string Status,
    [Required] DateTimeOffset DataHoraInicio,
    DateTimeOffset? DataHoraFim,
    Guid? LocalId,
    string? LocalTexto,
    bool RecorrenciaSemanal,
    [Range(0, 6)] int? DiaSemana
);
