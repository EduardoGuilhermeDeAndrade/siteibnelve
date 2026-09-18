namespace Ibnelve.Api.Data.Entities;

/// <summary>
/// Um ciclo de retirada/devolução de um <see cref="ItemPatrimonio"/>. Um registro cobre as duas
/// pontas: é criado na retirada (com <see cref="QuemRetirou"/>/<see cref="ObservacaoRetirada"/>) e
/// completado na devolução (<see cref="DataHoraDevolucao"/> nulo enquanto o item não volta). Para
/// itens de <see cref="TipoControlePatrimonio.Quantidade"/>, vários empréstimos podem estar ativos
/// ao mesmo tempo para o mesmo item, desde que a soma das quantidades não passe do estoque.
/// </summary>
public class EmprestimoPatrimonio
{
    public Guid Id { get; set; }
    public Guid ItemPatrimonioId { get; set; }
    public ItemPatrimonio? Item { get; set; }
    public int Quantidade { get; set; } = 1;
    public string QuemRetirou { get; set; } = string.Empty;
    public string ObservacaoRetirada { get; set; } = string.Empty;
    public DateTimeOffset DataHoraRetirada { get; set; }
    public string? QuemDevolveu { get; set; }
    public string? ObservacaoDevolucao { get; set; }
    public DateTimeOffset? DataHoraDevolucao { get; set; }
    public Guid CriadoPorUsuarioId { get; set; }
}
