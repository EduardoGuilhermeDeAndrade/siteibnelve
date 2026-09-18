namespace Ibnelve.Api.Data.Entities;

/// <summary>
/// Uma foto do painel público de fotos (até 10, ver GaleriaController). Mesmo esquema de
/// armazenamento de <see cref="ImagemSite"/> (bytea no Postgres), mas aqui são várias linhas
/// livres — com ordem e legenda — em vez de uma por chave fixa.
/// </summary>
public class FotoGaleria
{
    public Guid Id { get; set; }
    public byte[] Conteudo { get; set; } = [];
    public string ContentType { get; set; } = string.Empty;
    public string? Legenda { get; set; }
    public int Ordem { get; set; }
    public Guid AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
}
