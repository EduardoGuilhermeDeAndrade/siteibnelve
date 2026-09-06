namespace Ibnelve.Api.Data.Entities;

/// <summary>Pedido de oração enviado pelo formulário público de Contato. Nome/contato são
/// opcionais; se Anonimo=true, Nome é sempre null (imposto pelo backend, não confia só no front).</summary>
public class PedidoOracao
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public bool Anonimo { get; set; }
    public string? Contato { get; set; }
    public bool DesejaFalarComPastor { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public bool Lido { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
}
