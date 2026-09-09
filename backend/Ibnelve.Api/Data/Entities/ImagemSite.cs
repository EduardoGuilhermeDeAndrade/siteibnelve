namespace Ibnelve.Api.Data.Entities;

/// <summary>
/// Imagem gerida pelo admin, identificada por uma chave estável (ex.: "home-hero"). O conteúdo
/// binário fica no próprio Postgres — poucas imagens, sempre substituídas por inteiro quando
/// trocadas, não justificam um serviço de storage externo.
/// </summary>
public class ImagemSite
{
    public Guid Id { get; set; }
    public string Chave { get; set; } = string.Empty;
    public byte[] Conteudo { get; set; } = [];
    public string ContentType { get; set; } = string.Empty;
    public Guid AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
}
