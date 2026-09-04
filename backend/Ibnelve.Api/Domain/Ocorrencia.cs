namespace Ibnelve.Api.Domain;

/// <summary>
/// Ocorrência de uma SerieEvento numa data específica, sempre calculada em memória por
/// OcorrenciaCalculator — nunca persistida (regra explícita do CLAUDE.md: não gerar
/// fisicamente eventos, só a regra + exceções).
/// </summary>
public record Ocorrencia(
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
