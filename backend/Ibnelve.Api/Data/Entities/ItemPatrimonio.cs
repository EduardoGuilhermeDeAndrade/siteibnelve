namespace Ibnelve.Api.Data.Entities;

public enum TipoControlePatrimonio
{
    Unitario,
    Quantidade
}

public enum SituacaoPatrimonio
{
    Ativo,
    Inativo,
    Baixado
}

/// <summary>
/// Um bem físico da igreja (móvel, eletrodoméstico, cadeiras etc.). <see cref="TipoControle"/> é
/// fixo após a criação: <see cref="TipoControlePatrimonio.Unitario"/> para itens com entrada/saída
/// individual (ex.: um fogão, geralmente com <see cref="NumeroPatrimonio"/> próprio), ou
/// <see cref="TipoControlePatrimonio.Quantidade"/> para itens controlados em estoque (ex.: 100
/// cadeiras, das quais parte pode estar emprestada). Foto e "foto do defeito" seguem o mesmo
/// esquema de bytea no Postgres já usado em <see cref="ImagemSite"/>/<see cref="FotoGaleria"/>.
/// </summary>
public class ItemPatrimonio
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? NumeroPatrimonio { get; set; }
    public TipoControlePatrimonio TipoControle { get; set; }
    public int QuantidadeTotal { get; set; } = 1;
    public byte[]? FotoConteudo { get; set; }
    public string? FotoContentType { get; set; }
    public Guid? LocalId { get; set; }
    public Local? Local { get; set; }
    public SituacaoPatrimonio Situacao { get; set; } = SituacaoPatrimonio.Ativo;
    public string? Observacao { get; set; }
    public byte[]? FotoDefeitoConteudo { get; set; }
    public string? FotoDefeitoContentType { get; set; }
    public string? ObservacaoBaixa { get; set; }
    public DateTimeOffset? DataBaixa { get; set; }
    public Guid CriadoPorUsuarioId { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
    public Guid AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
    public List<EmprestimoPatrimonio> Emprestimos { get; set; } = [];
}
