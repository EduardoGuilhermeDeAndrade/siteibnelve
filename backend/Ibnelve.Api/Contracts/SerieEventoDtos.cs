using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record SerieEventoDto(
    Guid Id,
    string Titulo,
    string? Descricao,
    string Categoria,
    string Visibilidade,
    string Status,
    string TipoRecorrencia,
    int? Intervalo,
    int? DiaSemana,
    int? DiaMes,
    int? PosicaoNoMes,
    TimeOnly HoraInicio,
    TimeOnly? HoraFim,
    DateOnly DataInicioRecorrencia,
    DateOnly? DataFimRecorrencia,
    Guid? LocalId,
    string? LocalNome,
    string? LocalTexto,
    string? ImagemUrl,
    bool Destaque,
    bool Ativo
);

public record SerieEventoUpsertRequest(
    [Required, MaxLength(200)] string Titulo,
    string? Descricao,
    [Required] string Categoria,
    [Required] string Visibilidade,
    [Required] string Status,
    [Required] string TipoRecorrencia,
    [Range(1, 52)] int? Intervalo,
    [Range(0, 6)] int? DiaSemana,
    [Range(1, 31)] int? DiaMes,
    [Range(-1, 4)] int? PosicaoNoMes,
    [Required] TimeOnly HoraInicio,
    TimeOnly? HoraFim,
    [Required] DateOnly DataInicioRecorrencia,
    DateOnly? DataFimRecorrencia,
    Guid? LocalId,
    string? LocalTexto,
    string? ImagemUrl,
    bool Destaque,
    bool Ativo
);

/// <summary>Ocorrência calculada (nunca persistida) devolvida pela prévia do Admin.
/// DataOriginal identifica a ocorrência mesmo quando uma exceção move a data efetiva.</summary>
public record OcorrenciaDto(
    DateOnly DataOriginal,
    DateTimeOffset DataHoraInicio,
    DateTimeOffset? DataHoraFim,
    string Titulo,
    string? Descricao,
    Guid? LocalId,
    string? LocalNome,
    string? LocalTexto,
    bool Cancelada,
    bool TemExcecao,
    string? Motivo
);

/// <summary>Corpo do PUT que cria/atualiza a exceção de uma ocorrência (escopo "somente esta ocorrência").</summary>
public record ExcecaoUpsertRequest(
    bool Cancelado,
    DateOnly? NovaData,
    TimeOnly? NovaHoraInicio,
    TimeOnly? NovaHoraFim,
    string? TituloSubstituto,
    string? DescricaoSubstituta,
    Guid? LocalSubstitutoId,
    string? LocalTextoSubstituto,
    string? Motivo
);
