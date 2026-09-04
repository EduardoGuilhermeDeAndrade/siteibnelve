using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record MinisterioDto(
    Guid Id,
    string Nome,
    string Lideres,
    string? Descricao,
    string? ImagemUrl,
    bool Ativo,
    int Ordem
);

public record MinisterioUpsertRequest(
    [Required, MaxLength(200)] string Nome,
    [Required, MaxLength(200)] string Lideres,
    string? Descricao,
    string? ImagemUrl,
    bool Ativo,
    int Ordem
);
