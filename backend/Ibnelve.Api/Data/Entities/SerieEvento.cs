using Ibnelve.Api.Domain;

namespace Ibnelve.Api.Data.Entities;

/// <summary>
/// Regra de recorrência da Agenda (modelo conceitual SerieEvento do CLAUDE.md). Cobre os 6 tipos
/// de recorrência (Nenhuma = evento avulso, Semanal, ACadaNSemanas, MensalPorDia, MensalPorPosicao,
/// Anual). As datas efetivas nunca são persistidas: são calculadas sob demanda por
/// Domain.OcorrenciaCalculator a partir desta regra + das ExcecaoEvento associadas.
/// </summary>
public class SerieEvento
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public CategoriaEvento Categoria { get; set; }
    public VisibilidadeEvento Visibilidade { get; set; }
    public StatusEvento Status { get; set; }

    public TipoRecorrencia TipoRecorrencia { get; set; }

    /// <summary>Só usado em ACadaNSemanas (a cada N semanas).</summary>
    public int? Intervalo { get; set; }

    /// <summary>Usado em Semanal, ACadaNSemanas e MensalPorPosicao.</summary>
    public DayOfWeek? DiaSemana { get; set; }

    /// <summary>Só usado em MensalPorDia.</summary>
    public int? DiaMes { get; set; }

    /// <summary>Só usado em MensalPorPosicao: 1-4 = primeiro..quarto, -1 = último.</summary>
    public int? PosicaoNoMes { get; set; }

    public TimeOnly HoraInicio { get; set; }
    public TimeOnly? HoraFim { get; set; }

    /// <summary>Data de referência da série: para Nenhuma é a própria data do evento avulso;
    /// para Anual, mês/dia fixam a data anual; para as demais, marca o início da janela de validade.</summary>
    public DateOnly DataInicioRecorrencia { get; set; }

    /// <summary>null = sem data de fim (recorrência indefinida).</summary>
    public DateOnly? DataFimRecorrencia { get; set; }

    public Guid? LocalId { get; set; }
    public Local? Local { get; set; }
    public string? LocalTexto { get; set; }

    public string? ImagemUrl { get; set; }
    public bool Destaque { get; set; }
    public bool Ativo { get; set; } = true;

    public Guid CriadoPorUsuarioId { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
    public Guid? AtualizadoPorUsuarioId { get; set; }
    public DateTimeOffset? DataAtualizacao { get; set; }

    public List<ExcecaoEvento> Excecoes { get; set; } = [];
}
