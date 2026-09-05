using Ibnelve.Api.Domain;

namespace Ibnelve.Api.Data.Entities;

/// <summary>
/// Configuração de exibição da chave PIX de contribuições — singleton (só existe 1 linha),
/// semeada com os dados confirmados do CLAUDE.md. Editável pelo Portal Admin.
/// </summary>
public class ConfiguracaoContribuicao
{
    public Guid Id { get; set; }
    public string ChavePix { get; set; } = string.Empty;
    public TipoChavePix TipoChave { get; set; }
    public string Favorecido { get; set; } = string.Empty;

    public Guid? AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset? DataAtualizacao { get; set; }
}
