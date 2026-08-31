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
    [property: Required, MaxLength(200)] string Titulo,
    string? Descricao,
    [property: Required] string Categoria,
    [property: Required] string Visibilidade,
    [property: Required] string Status,
    [property: Required] DateTimeOffset DataHoraInicio,
    DateTimeOffset? DataHoraFim,
    Guid? LocalId,
    string? LocalTexto,
    bool RecorrenciaSemanal,
    [property: Range(0, 6)] int? DiaSemana
);
