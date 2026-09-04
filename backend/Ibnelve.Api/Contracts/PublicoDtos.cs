namespace Ibnelve.Api.Contracts;

/// <summary>
/// Único DTO público que precisa ser diferente do DTO de admin: Local/Ministério/ConteudoTexto/
/// ImagemSite não têm nada sensível, então os endpoints públicos reaproveitam os DTOs de admin
/// (LocalDto, MinisterioDto, ConteudoTextoDto, ImagemSiteDto). Evento é diferente porque o público
/// recebe a PRÓXIMA OCORRÊNCIA já calculada (Data), não a data bruta armazenada, e um único campo
/// de Local (nome ou texto) em vez de LocalId/LocalNome/LocalTexto separados.
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
