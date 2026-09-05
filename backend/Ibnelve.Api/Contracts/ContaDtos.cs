using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record TrocarSenhaRequest(
    [Required] string SenhaAtual,
    [Required, MinLength(8)] string NovaSenha
);
