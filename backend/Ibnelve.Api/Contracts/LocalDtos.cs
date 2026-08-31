using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record LocalDto(
    Guid Id,
    string Nome,
    string Tipo,
    string Endereco,
    string Bairro,
    string Cidade,
    string? Estado,
    string? Cep,
    string? GoogleMapsUrl,
    double? Latitude,
    double? Longitude,
    bool Ativo,
    int Ordem
);

public record LocalUpsertRequest(
    [property: Required, MaxLength(200)] string Nome,
    [property: Required, MaxLength(100)] string Tipo,
    [property: Required, MaxLength(300)] string Endereco,
    [property: Required, MaxLength(120)] string Bairro,
    [property: Required, MaxLength(120)] string Cidade,
    string? Estado,
    string? Cep,
    string? GoogleMapsUrl,
    double? Latitude,
    double? Longitude,
    bool Ativo,
    int Ordem
);
