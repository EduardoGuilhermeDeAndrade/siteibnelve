using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

/// <summary>Sem dado sensível — reaproveitado tanto pelo endpoint de admin quanto pelo público
/// (mesmo padrão já usado para Local/Ministério/ConteudoTexto).</summary>
public record ConfiguracaoContribuicaoDto(
    Guid Id,
    string ChavePix,
    string TipoChave,
    string Favorecido,
    DateTimeOffset? DataAtualizacao
);

public record ConfiguracaoContribuicaoUpsertRequest(
    [Required, MaxLength(200)] string ChavePix,
    [Required] string TipoChave,
    [Required, MaxLength(200)] string Favorecido
);
