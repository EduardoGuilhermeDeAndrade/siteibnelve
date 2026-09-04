namespace Ibnelve.Api.Data.Entities;

public class Ministerio
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Lideres { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? ImagemUrl { get; set; }
    public bool Ativo { get; set; } = true;
    public int Ordem { get; set; }
}
