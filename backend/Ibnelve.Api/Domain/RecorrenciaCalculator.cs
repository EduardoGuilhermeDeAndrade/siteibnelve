using Ibnelve.Api.Data.Entities;

namespace Ibnelve.Api.Domain;

/// <summary>
/// Calcula a próxima ocorrência de um Evento a partir de agora — porta em C# a mesma lógica de
/// frontend/src/app/shared/data/recorrencia.util.ts (proximoDiaSemana), agora do lado do servidor.
/// Só cobre os dois casos que a entidade Evento suporta hoje (avulso / semanal fixo); o motor
/// completo de recorrência (mensal por posição, exceções etc.) é a Fase 6.
/// </summary>
public static class RecorrenciaCalculator
{
    /// <summary>Retorna null se o evento não tem mais ocorrências futuras (avulso já passado).</summary>
    public static DateTimeOffset? ProximaOcorrencia(Evento evento, DateTimeOffset agora)
    {
        if (!evento.RecorrenciaSemanal)
        {
            return evento.DataHoraInicio >= agora ? evento.DataHoraInicio : null;
        }

        if (evento.DiaSemana is null)
        {
            return null;
        }

        var horaDoEvento = evento.DataHoraInicio.TimeOfDay;
        var candidata = new DateTimeOffset(agora.Date, agora.Offset).Add(horaDoEvento);

        var diferencaDias = ((int)evento.DiaSemana.Value - (int)candidata.DayOfWeek + 7) % 7;
        candidata = candidata.AddDays(diferencaDias);

        while (candidata < agora)
        {
            candidata = candidata.AddDays(7);
        }

        return candidata;
    }
}
