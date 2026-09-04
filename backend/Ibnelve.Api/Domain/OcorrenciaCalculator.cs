using Ibnelve.Api.Data.Entities;

namespace Ibnelve.Api.Domain;

/// <summary>
/// Motor de recorrência completo da Agenda (Fase 6): calcula ocorrências de uma SerieEvento por
/// janela de consulta, aplicando as ExcecaoEvento associadas. Nunca gera/persiste fisicamente
/// eventos para muitos anos — só a matemática de recorrência a partir da regra + exceções.
/// </summary>
public static class OcorrenciaCalculator
{
    private const int JanelaInicialDias = 366;
    private const int JanelaTetoDias = 366 * 5;

    /// <summary>HoraInicio/HoraFim são sempre horário local de Brasília (hora que o admin digita e
    /// que aparece na Agenda), nunca UTC. O Brasil não observa horário de verão desde 2019, então um
    /// offset fixo é suficiente — evita depender de banco de fusos horários (IANA/Windows) só para
    /// uma igreja com um único fuso.</summary>
    private static readonly TimeSpan OffsetBrasilia = TimeSpan.FromHours(-3);

    /// <summary>Só a matemática pura de recorrência por tipo, sem aplicar exceções.</summary>
    public static IReadOnlyList<DateOnly> DatasBase(SerieEvento serie, DateOnly inicio, DateOnly fim)
    {
        var datas = new List<DateOnly>();

        var limiteInicio = serie.DataInicioRecorrencia > inicio ? serie.DataInicioRecorrencia : inicio;
        var limiteFim = serie.DataFimRecorrencia is { } dataFim && dataFim < fim ? dataFim : fim;

        if (limiteInicio > limiteFim)
        {
            return datas;
        }

        switch (serie.TipoRecorrencia)
        {
            case TipoRecorrencia.Nenhuma:
                if (serie.DataInicioRecorrencia >= limiteInicio && serie.DataInicioRecorrencia <= limiteFim)
                {
                    datas.Add(serie.DataInicioRecorrencia);
                }
                break;

            case TipoRecorrencia.Semanal:
            case TipoRecorrencia.ACadaNSemanas:
                AdicionarDatasSemanais(serie, limiteInicio, limiteFim, datas);
                break;

            case TipoRecorrencia.MensalPorDia:
                AdicionarDatasMensalPorDia(serie, limiteInicio, limiteFim, datas);
                break;

            case TipoRecorrencia.MensalPorPosicao:
                AdicionarDatasMensalPorPosicao(serie, limiteInicio, limiteFim, datas);
                break;

            case TipoRecorrencia.Anual:
                AdicionarDatasAnuais(serie, limiteInicio, limiteFim, datas);
                break;
        }

        return datas;
    }

    private static void AdicionarDatasSemanais(SerieEvento serie, DateOnly limiteInicio, DateOnly limiteFim, List<DateOnly> datas)
    {
        if (serie.DiaSemana is null)
        {
            return;
        }

        var passoSemanas = serie.TipoRecorrencia == TipoRecorrencia.ACadaNSemanas
            ? Math.Max(1, serie.Intervalo ?? 1)
            : 1;

        var diferenca = ((int)serie.DiaSemana.Value - (int)serie.DataInicioRecorrencia.DayOfWeek + 7) % 7;
        var candidata = serie.DataInicioRecorrencia.AddDays(diferenca);

        while (candidata < limiteInicio)
        {
            candidata = candidata.AddDays(7 * passoSemanas);
        }

        while (candidata <= limiteFim)
        {
            datas.Add(candidata);
            candidata = candidata.AddDays(7 * passoSemanas);
        }
    }

    private static void AdicionarDatasMensalPorDia(SerieEvento serie, DateOnly limiteInicio, DateOnly limiteFim, List<DateOnly> datas)
    {
        if (serie.DiaMes is null)
        {
            return;
        }

        var cursor = new DateOnly(limiteInicio.Year, limiteInicio.Month, 1);
        var cursorFim = new DateOnly(limiteFim.Year, limiteFim.Month, 1);

        while (cursor <= cursorFim)
        {
            var diasNoMes = DateTime.DaysInMonth(cursor.Year, cursor.Month);
            if (serie.DiaMes.Value <= diasNoMes)
            {
                var data = new DateOnly(cursor.Year, cursor.Month, serie.DiaMes.Value);
                if (data >= limiteInicio && data <= limiteFim)
                {
                    datas.Add(data);
                }
            }

            cursor = cursor.AddMonths(1);
        }
    }

    private static void AdicionarDatasMensalPorPosicao(SerieEvento serie, DateOnly limiteInicio, DateOnly limiteFim, List<DateOnly> datas)
    {
        if (serie.DiaSemana is null || serie.PosicaoNoMes is null)
        {
            return;
        }

        var cursor = new DateOnly(limiteInicio.Year, limiteInicio.Month, 1);
        var cursorFim = new DateOnly(limiteFim.Year, limiteFim.Month, 1);

        while (cursor <= cursorFim)
        {
            var data = CalcularDiaPorPosicao(cursor.Year, cursor.Month, serie.DiaSemana.Value, serie.PosicaoNoMes.Value);
            if (data is not null && data.Value >= limiteInicio && data.Value <= limiteFim)
            {
                datas.Add(data.Value);
            }

            cursor = cursor.AddMonths(1);
        }
    }

    private static DateOnly? CalcularDiaPorPosicao(int ano, int mes, DayOfWeek diaSemana, int posicao)
    {
        var diasNoMes = DateTime.DaysInMonth(ano, mes);
        var candidatos = Enumerable.Range(1, diasNoMes)
            .Select(dia => new DateOnly(ano, mes, dia))
            .Where(data => data.DayOfWeek == diaSemana)
            .ToList();

        if (candidatos.Count == 0)
        {
            return null;
        }

        if (posicao == -1)
        {
            return candidatos[^1];
        }

        var indice = posicao - 1;
        return indice >= 0 && indice < candidatos.Count ? candidatos[indice] : null;
    }

    private static void AdicionarDatasAnuais(SerieEvento serie, DateOnly limiteInicio, DateOnly limiteFim, List<DateOnly> datas)
    {
        var mes = serie.DataInicioRecorrencia.Month;
        var dia = serie.DataInicioRecorrencia.Day;

        for (var ano = limiteInicio.Year; ano <= limiteFim.Year; ano++)
        {
            if (dia > DateTime.DaysInMonth(ano, mes))
            {
                continue;
            }

            var data = new DateOnly(ano, mes, dia);
            if (data >= limiteInicio && data <= limiteFim)
            {
                datas.Add(data);
            }
        }
    }

    /// <summary>Aplica as ExcecaoEvento sobre as datas base e devolve a lista final ordenada.
    /// Requer serie.Local e serie.Excecoes (com LocalSubstituto) já carregados.</summary>
    public static IReadOnlyList<Ocorrencia> CalcularOcorrencias(SerieEvento serie, DateOnly janelaInicio, DateOnly janelaFim)
    {
        var datasBase = DatasBase(serie, janelaInicio, janelaFim);
        var excecoesPorData = serie.Excecoes.ToDictionary(e => e.DataOriginal);

        var resultado = new List<Ocorrencia>();
        foreach (var data in datasBase)
        {
            excecoesPorData.TryGetValue(data, out var excecao);
            resultado.Add(MontarOcorrencia(serie, data, excecao));
        }

        return resultado.OrderBy(o => o.DataHoraInicio).ToList();
    }

    private static Ocorrencia MontarOcorrencia(SerieEvento serie, DateOnly dataOriginal, ExcecaoEvento? excecao)
    {
        if (excecao is not null && excecao.Cancelado)
        {
            return new Ocorrencia(
                dataOriginal,
                CombinarComOffsetLocal(dataOriginal, serie.HoraInicio),
                serie.HoraFim is null ? null : CombinarComOffsetLocal(dataOriginal, serie.HoraFim.Value),
                excecao.TituloSubstituto ?? serie.Titulo,
                excecao.DescricaoSubstituta ?? serie.Descricao,
                serie.LocalId,
                serie.Local?.Nome,
                serie.LocalTexto,
                Cancelada: true,
                TemExcecao: true,
                excecao.Motivo);
        }

        var data = excecao?.NovaData ?? dataOriginal;
        var horaInicio = excecao?.NovaHoraInicio ?? serie.HoraInicio;
        var horaFim = excecao?.NovaHoraFim ?? serie.HoraFim;
        var localId = excecao?.LocalSubstitutoId ?? serie.LocalId;
        var localNome = excecao?.LocalSubstitutoId is not null ? excecao.LocalSubstituto?.Nome : serie.Local?.Nome;
        var localTexto = excecao?.LocalTextoSubstituto ?? serie.LocalTexto;

        return new Ocorrencia(
            dataOriginal,
            CombinarComOffsetLocal(data, horaInicio),
            horaFim is null ? null : CombinarComOffsetLocal(data, horaFim.Value),
            excecao?.TituloSubstituto ?? serie.Titulo,
            excecao?.DescricaoSubstituta ?? serie.Descricao,
            localId,
            localNome,
            localTexto,
            Cancelada: false,
            TemExcecao: excecao is not null,
            excecao?.Motivo);
    }

    private static DateTimeOffset CombinarComOffsetLocal(DateOnly data, TimeOnly hora) =>
        new(data.ToDateTime(hora), OffsetBrasilia);

    /// <summary>Data de "hoje" no horário de Brasília — usada como início padrão das janelas de
    /// consulta, para não cortar o dia no fuso errado perto da meia-noite.</summary>
    public static DateOnly HojeLocal(DateTimeOffset agora) => DateOnly.FromDateTime(agora.ToOffset(OffsetBrasilia).DateTime);

    /// <summary>Próxima ocorrência não cancelada a partir de agora, expandindo a janela de
    /// consulta progressivamente até um teto de segurança. Usado por páginas que mostram
    /// "próximo evento" de uma categoria/série específica.</summary>
    public static Ocorrencia? ProximaOcorrencia(SerieEvento serie, DateTimeOffset agora)
    {
        var hoje = HojeLocal(agora);
        var janela = JanelaInicialDias;

        while (true)
        {
            var fim = hoje.AddDays(janela);
            var proxima = CalcularOcorrencias(serie, hoje, fim)
                .FirstOrDefault(o => !o.Cancelada && o.DataHoraInicio >= agora);

            if (proxima is not null)
            {
                return proxima;
            }

            var esgotouSerie = serie.TipoRecorrencia == TipoRecorrencia.Nenhuma
                || (serie.DataFimRecorrencia is { } dataFim && fim >= dataFim);
            if (esgotouSerie || janela >= JanelaTetoDias)
            {
                return null;
            }

            janela *= 2;
        }
    }
}
