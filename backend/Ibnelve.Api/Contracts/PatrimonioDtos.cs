using System.ComponentModel.DataAnnotations;

namespace Ibnelve.Api.Contracts;

public record ItemPatrimonioDto(
    Guid Id,
    string Descricao,
    string? NumeroPatrimonio,
    string TipoControle,
    int QuantidadeTotal,
    int QuantidadeEmprestada,
    int QuantidadeDisponivel,
    string? FotoUrl,
    Guid? LocalId,
    string? LocalNome,
    string Situacao,
    string? Observacao,
    string? FotoDefeitoUrl,
    string? ObservacaoBaixa,
    DateTimeOffset? DataBaixa,
    DateTimeOffset DataAtualizacao
);

public record EmprestimoPatrimonioDto(
    Guid Id,
    int Quantidade,
    string QuemRetirou,
    string ObservacaoRetirada,
    DateTimeOffset DataHoraRetirada,
    string? QuemDevolveu,
    string? ObservacaoDevolucao,
    DateTimeOffset? DataHoraDevolucao
);

public record ItemPatrimonioAtualizarRequest(
    [Required, MaxLength(300)] string Descricao,
    [MaxLength(100)] string? NumeroPatrimonio,
    [Range(1, int.MaxValue)] int QuantidadeTotal,
    Guid? LocalId,
    [MaxLength(1000)] string? Observacao
);

public record ItemPatrimonioSituacaoRequest(
    bool Ativo,
    [MaxLength(1000)] string? Observacao
);

public record ItemPatrimonioEmprestimoRequest(
    [Range(1, int.MaxValue)] int Quantidade,
    [Required, MaxLength(200)] string QuemRetirou,
    [Required, MaxLength(1000)] string Observacao
);

public record ItemPatrimonioDevolucaoRequest(
    [Required, MaxLength(200)] string QuemDevolveu,
    [Required, MaxLength(1000)] string Observacao
);
