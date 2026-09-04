using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record ConteudoTextoDto(string Chave, string Titulo, string Conteudo, DateTimeOffset DataAtualizacao);

public record ConteudoTextoUpsertRequest(
    [Required, MaxLength(200)] string Titulo,
    [Required] string Conteudo
);
