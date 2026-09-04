namespace Ibnelve.Api.Data.Entities;

/// <summary>Bloco de texto institucional editável pelo Portal (Quem Somos, Missão, Visão etc.).</summary>
public class ConteudoTexto
{
    public Guid Id { get; set; }
    public string Chave { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public Guid AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
}
