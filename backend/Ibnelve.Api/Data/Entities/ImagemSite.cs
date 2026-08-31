namespace Ibnelve.Api.Data.Entities;

/// <summary>Imagem gerida pelo admin, identificada por uma chave estável (ex.: "home-hero").</summary>
public class ImagemSite
{
    public Guid Id { get; set; }
    public string Chave { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Guid AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
}
