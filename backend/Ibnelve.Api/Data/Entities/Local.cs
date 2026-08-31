namespace Ibnelve.Api.Data.Entities;

public class Local
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool Ativo { get; set; } = true;
    public int Ordem { get; set; }
}
