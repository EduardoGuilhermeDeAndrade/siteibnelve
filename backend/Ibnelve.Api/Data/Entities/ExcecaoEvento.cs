namespace Ibnelve.Api.Data.Entities;

/// <summary>
/// Exceção pontual de uma ocorrência calculada de uma SerieEvento (escopo "somente esta
/// ocorrência"). Uma exceção específica sempre prevalece sobre a regra de recorrência.
/// Cancelado=true esconde a ocorrência (ex.: da Agenda pública) sem afetar as demais.
/// </summary>
public class ExcecaoEvento
{
    public Guid Id { get; set; }
    public Guid SerieEventoId { get; set; }
    public SerieEvento? SerieEvento { get; set; }

    /// <summary>Data calculada pela regra da série que esta exceção substitui/cancela.</summary>
    public DateOnly DataOriginal { get; set; }

    public bool Cancelado { get; set; }

    public DateOnly? NovaData { get; set; }
    public TimeOnly? NovaHoraInicio { get; set; }
    public TimeOnly? NovaHoraFim { get; set; }
    public string? TituloSubstituto { get; set; }
    public string? DescricaoSubstituta { get; set; }
    public Guid? LocalSubstitutoId { get; set; }
    public Local? LocalSubstituto { get; set; }
    public string? LocalTextoSubstituto { get; set; }
    public string? Motivo { get; set; }

    public Guid CriadoPorUsuarioId { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
    public Guid? AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset? DataAtualizacao { get; set; }
}
