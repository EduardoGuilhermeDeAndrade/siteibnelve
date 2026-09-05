using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Domain;

namespace Ibnelve.Api.Tests.Domain;

public class OcorrenciaCalculatorTests
{
    private static SerieEvento CriarSerie(
        TipoRecorrencia tipo,
        DateOnly dataInicioRecorrencia,
        DayOfWeek? diaSemana = null,
        int? intervalo = null,
        int? diaMes = null,
        int? posicaoNoMes = null,
        DateOnly? dataFimRecorrencia = null,
        TimeOnly? horaInicio = null) => new()
    {
        Id = Guid.NewGuid(),
        Titulo = "Evento de teste",
        Categoria = CategoriaEvento.Culto,
        Visibilidade = VisibilidadeEvento.Publico,
        Status = StatusEvento.Publicado,
        TipoRecorrencia = tipo,
        DiaSemana = diaSemana,
        Intervalo = intervalo,
        DiaMes = diaMes,
        PosicaoNoMes = posicaoNoMes,
        HoraInicio = horaInicio ?? new TimeOnly(18, 0),
        DataInicioRecorrencia = dataInicioRecorrencia,
        DataFimRecorrencia = dataFimRecorrencia,
        Ativo = true
    };

    [Fact]
    public void DatasBase_Semanal_RetornaTodosOsDomingosNaJanela()
    {
        var serie = CriarSerie(TipoRecorrencia.Semanal, new DateOnly(2026, 9, 6), DayOfWeek.Sunday);

        var datas = OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 30));

        Assert.Equal(
            [new DateOnly(2026, 9, 6), new DateOnly(2026, 9, 13), new DateOnly(2026, 9, 20), new DateOnly(2026, 9, 27)],
            datas);
    }

    [Fact]
    public void DatasBase_ACadaNSemanas_RespeitaIntervaloAncoradoNoInicio()
    {
        // Mocidade: a cada 2 semanas, sábado, começando num sábado.
        var serie = CriarSerie(TipoRecorrencia.ACadaNSemanas, new DateOnly(2026, 9, 5), DayOfWeek.Saturday, intervalo: 2);

        var datas = OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 9, 1), new DateOnly(2026, 10, 31));

        Assert.Equal(
            [
                new DateOnly(2026, 9, 5), new DateOnly(2026, 9, 19),
                new DateOnly(2026, 10, 3), new DateOnly(2026, 10, 17), new DateOnly(2026, 10, 31)
            ],
            datas);
    }

    [Fact]
    public void DatasBase_MensalPorDia_PulaMesesSemODia31()
    {
        var serie = CriarSerie(TipoRecorrencia.MensalPorDia, new DateOnly(2026, 1, 1), diaMes: 31);

        // 2026 não é bissexto: fevereiro tem 28 dias, então não gera ocorrência em fevereiro.
        var datas = OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 1, 1), new DateOnly(2026, 3, 31));

        Assert.Equal([new DateOnly(2026, 1, 31), new DateOnly(2026, 3, 31)], datas);
    }

    [Fact]
    public void DatasBase_MensalPorPosicao_PrimeiroDomingoDoMes()
    {
        // Ceia: primeiro domingo de cada mês.
        var serie = CriarSerie(TipoRecorrencia.MensalPorPosicao, new DateOnly(2026, 1, 1), DayOfWeek.Sunday, posicaoNoMes: 1);

        var datas = OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 1, 1), new DateOnly(2026, 5, 31));

        Assert.Equal(
            [
                new DateOnly(2026, 1, 4), new DateOnly(2026, 2, 1), new DateOnly(2026, 3, 1),
                new DateOnly(2026, 4, 5), new DateOnly(2026, 5, 3)
            ],
            datas);
    }

    [Fact]
    public void DatasBase_MensalPorPosicao_UltimoDomingoDoMes()
    {
        var serie = CriarSerie(TipoRecorrencia.MensalPorPosicao, new DateOnly(2026, 1, 1), DayOfWeek.Sunday, posicaoNoMes: -1);

        var datas = OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 1, 1), new DateOnly(2026, 3, 31));

        Assert.Equal([new DateOnly(2026, 1, 25), new DateOnly(2026, 2, 22), new DateOnly(2026, 3, 29)], datas);
    }

    [Fact]
    public void DatasBase_Anual_PulaAnosNaoBissextosQuandoDataEh29DeFevereiro()
    {
        var serie = CriarSerie(TipoRecorrencia.Anual, new DateOnly(2024, 2, 29));

        var datas = OcorrenciaCalculator.DatasBase(serie, new DateOnly(2025, 1, 1), new DateOnly(2028, 12, 31));

        // Só 2028 é bissexto no intervalo (2025/2026/2027 não têm 29/fev).
        Assert.Equal([new DateOnly(2028, 2, 29)], datas);
    }

    [Fact]
    public void DatasBase_Nenhuma_SoRetornaDentroDaJanela()
    {
        var serie = CriarSerie(TipoRecorrencia.Nenhuma, new DateOnly(2026, 10, 18));

        Assert.Equal([new DateOnly(2026, 10, 18)], OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 1, 1), new DateOnly(2027, 1, 1)));
        Assert.Empty(OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 1, 1), new DateOnly(2026, 10, 1)));
        Assert.Empty(OcorrenciaCalculator.DatasBase(serie, new DateOnly(2026, 11, 1), new DateOnly(2026, 12, 1)));
    }

    [Fact]
    public void CalcularOcorrencias_SemExcecao_NaoMarcaAlteracaoOuCancelamento()
    {
        var serie = CriarSerie(TipoRecorrencia.Semanal, new DateOnly(2026, 9, 6), DayOfWeek.Sunday);

        var ocorrencias = OcorrenciaCalculator.CalcularOcorrencias(serie, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 10));

        var ocorrencia = Assert.Single(ocorrencias);
        Assert.False(ocorrencia.Cancelada);
        Assert.False(ocorrencia.TemExcecao);
        Assert.Equal("Evento de teste", ocorrencia.Titulo);
    }

    [Fact]
    public void CalcularOcorrencias_ExcecaoDeHorario_SubstituiSemAfetarOutrasOcorrencias()
    {
        var serie = CriarSerie(TipoRecorrencia.Semanal, new DateOnly(2026, 9, 6), DayOfWeek.Sunday);
        serie.Excecoes.Add(new ExcecaoEvento
        {
            SerieEventoId = serie.Id,
            DataOriginal = new DateOnly(2026, 9, 13),
            NovaHoraInicio = new TimeOnly(19, 30),
            Motivo = "Horário excepcional"
        });

        var ocorrencias = OcorrenciaCalculator.CalcularOcorrencias(serie, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 20));

        var alterada = ocorrencias.Single(o => o.DataOriginal == new DateOnly(2026, 9, 13));
        Assert.True(alterada.TemExcecao);
        Assert.False(alterada.Cancelada);
        Assert.Equal(new TimeOnly(19, 30), TimeOnly.FromDateTime(alterada.DataHoraInicio.DateTime));
        Assert.Equal("Horário excepcional", alterada.Motivo);

        var inalterada = ocorrencias.Single(o => o.DataOriginal == new DateOnly(2026, 9, 6));
        Assert.False(inalterada.TemExcecao);
        Assert.Equal(new TimeOnly(18, 0), TimeOnly.FromDateTime(inalterada.DataHoraInicio.DateTime));
    }

    [Fact]
    public void CalcularOcorrencias_ExcecaoDeCancelamento_MarcaCanceladaMasContinuaNaLista()
    {
        var serie = CriarSerie(TipoRecorrencia.Semanal, new DateOnly(2026, 9, 6), DayOfWeek.Sunday);
        serie.Excecoes.Add(new ExcecaoEvento
        {
            SerieEventoId = serie.Id,
            DataOriginal = new DateOnly(2026, 9, 13),
            Cancelado = true,
            Motivo = "Feriado"
        });

        var ocorrencias = OcorrenciaCalculator.CalcularOcorrencias(serie, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 20));

        Assert.Equal(3, ocorrencias.Count);
        var cancelada = ocorrencias.Single(o => o.DataOriginal == new DateOnly(2026, 9, 13));
        Assert.True(cancelada.Cancelada);
        Assert.True(cancelada.TemExcecao);
    }

    [Fact]
    public void CalcularOcorrencias_HorarioSempreComOffsetDeBrasilia()
    {
        // Regressão do bug de fuso da Fase 6: HoraInicio é local de Brasília, não UTC.
        var serie = CriarSerie(TipoRecorrencia.Nenhuma, new DateOnly(2026, 9, 6), horaInicio: new TimeOnly(18, 0));

        var ocorrencia = Assert.Single(OcorrenciaCalculator.CalcularOcorrencias(serie, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 30)));

        Assert.Equal(TimeSpan.FromHours(-3), ocorrencia.DataHoraInicio.Offset);
        Assert.Equal(18, ocorrencia.DataHoraInicio.Hour);
    }

    [Fact]
    public void ProximaOcorrencia_PulaOcorrenciaCanceladaERetornaASeguinte()
    {
        var serie = CriarSerie(TipoRecorrencia.Semanal, new DateOnly(2026, 9, 6), DayOfWeek.Sunday);
        serie.Excecoes.Add(new ExcecaoEvento
        {
            SerieEventoId = serie.Id,
            DataOriginal = new DateOnly(2026, 9, 6),
            Cancelado = true
        });

        var agora = new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.FromHours(-3));
        var proxima = OcorrenciaCalculator.ProximaOcorrencia(serie, agora);

        Assert.NotNull(proxima);
        Assert.Equal(new DateOnly(2026, 9, 13), proxima!.DataOriginal);
    }

    [Fact]
    public void ProximaOcorrencia_RetornaNullQuandoEventoAvulsoJaPassou()
    {
        var serie = CriarSerie(TipoRecorrencia.Nenhuma, new DateOnly(2026, 1, 1));

        var agora = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.FromHours(-3));

        Assert.Null(OcorrenciaCalculator.ProximaOcorrencia(serie, agora));
    }

    [Fact]
    public void ProximaOcorrencia_RetornaNullQuandoSerieRecorrenteJaEncerrou()
    {
        var serie = CriarSerie(
            TipoRecorrencia.Semanal,
            new DateOnly(2026, 1, 4),
            DayOfWeek.Sunday,
            dataFimRecorrencia: new DateOnly(2026, 2, 1));

        var agora = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.FromHours(-3));

        Assert.Null(OcorrenciaCalculator.ProximaOcorrencia(serie, agora));
    }
}
