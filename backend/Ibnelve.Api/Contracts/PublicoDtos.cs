namespace Ibnelve.Api.Contracts;

/// <summary>
/// Único DTO público que precisa ser diferente do DTO de admin: Local/Ministério/ConteudoTexto/
/// ImagemSite não têm nada sensível, então os endpoints públicos reaproveitam os DTOs de admin
/// (LocalDto, MinisterioDto, ConteudoTextoDto, ImagemSiteDto). Evento é diferente porque o público
/// recebe uma OCORRÊNCIA já calculada (Data) de uma SerieEvento, não a regra de recorrência bruta,
/// e um único campo de Local (nome ou texto) em vez de LocalId/LocalNome/LocalTexto separados.
/// RecorrenciaSemanal generalizou de significado na Fase 6 (série tem 6 tipos possíveis de
/// recorrência agora) mas manteve nome/formato: true quando a série é recorrente (TipoRecorrencia
/// != NENHUMA), preservando o contrato já consumido pelo site público.
/// </summary>
public record EventoPublicoDto(
    Guid Id,
    string Titulo,
    string? Descricao,
    string Categoria,
    DateTimeOffset Data,
    string? Local,
    bool RecorrenciaSemanal
);
