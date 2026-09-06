using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record PedidoOracaoCriarRequest(
    string? Nome,
    bool Anonimo,
    string? Contato,
    bool DesejaFalarComPastor,
    [Required(AllowEmptyStrings = false)] string Mensagem
);

public record PedidoOracaoAdminDto(
    Guid Id,
    string? Nome,
    bool Anonimo,
    string? Contato,
    bool DesejaFalarComPastor,
    string Mensagem,
    bool Lido,
    DateTimeOffset DataCriacao
);
