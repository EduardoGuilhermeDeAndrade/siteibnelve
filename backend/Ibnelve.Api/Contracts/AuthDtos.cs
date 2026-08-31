using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password
);

public record UsuarioLogadoDto(Guid Id, string Email, string NomeCompleto, IReadOnlyList<string> Roles);

public record LoginResponse(string AccessToken, DateTimeOffset ExpiraEm, UsuarioLogadoDto Usuario);
