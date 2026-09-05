using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record UsuarioAdminDto(
    Guid Id,
    string NomeCompleto,
    string Email,
    bool Ativo
);

public record UsuarioCriarRequest(
    [Required, MaxLength(200)] string NomeCompleto,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Senha
);

public record UsuarioAtualizarRequest(
    [Required, MaxLength(200)] string NomeCompleto
);

public record RedefinirSenhaRequest(
    [Required, MinLength(8)] string NovaSenha
);
