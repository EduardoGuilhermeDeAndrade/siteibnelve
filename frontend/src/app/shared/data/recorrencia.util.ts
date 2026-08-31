/**
 * Utilitários de recorrência simplificados para a Agenda mock (Fase 2).
 * Calculam apenas a PRÓXIMA ocorrência a partir de agora — não é o motor de
 * recorrência completo do CLAUDE.md (séries, exceções, janela de consulta),
 * que fica para a Fase 6.
 */

function primeiraOcorrenciaDoDiaNoMes(ano: number, mes: number, diaSemana: number): Date {
  const data = new Date(ano, mes, 1);
  const diferenca = (diaSemana - data.getDay() + 7) % 7;
  data.setDate(1 + diferenca);
  return data;
}

function ehPrimeiraOcorrenciaDoMes(data: Date): boolean {
  return data.getDate() <= 7;
}

/** Próxima ocorrência de um dia da semana em um horário fixo (ex.: todo domingo às 18h). */
export function proximoDiaSemana(
  diaSemana: number,
  hora: number,
  minuto: number,
  opcoes?: { excetoPrimeiraOcorrenciaDoMes?: boolean }
): Date {
  const agora = new Date();
  const candidata = new Date(agora);
  const diferenca = (diaSemana - agora.getDay() + 7) % 7;
  candidata.setDate(agora.getDate() + diferenca);
  candidata.setHours(hora, minuto, 0, 0);

  while (
    candidata < agora ||
    (opcoes?.excetoPrimeiraOcorrenciaDoMes && ehPrimeiraOcorrenciaDoMes(candidata))
  ) {
    candidata.setDate(candidata.getDate() + 7);
  }

  return candidata;
}

/** Próxima ocorrência no primeiro dia-da-semana X de um mês (ex.: 1º domingo às 08h30). */
export function proximoPrimeiroDiaSemanaDoMes(diaSemana: number, hora: number, minuto: number): Date {
  const agora = new Date();
  let ano = agora.getFullYear();
  let mes = agora.getMonth();

  for (let i = 0; i < 24; i++) {
    const candidata = primeiraOcorrenciaDoDiaNoMes(ano, mes, diaSemana);
    candidata.setHours(hora, minuto, 0, 0);
    if (candidata >= agora) {
      return candidata;
    }
    mes += 1;
    if (mes > 11) {
      mes = 0;
      ano += 1;
    }
  }

  return agora;
}

/** Próxima ocorrência a cada N semanas, a partir de uma data-âncora (ex.: Mocidade quinzenal). */
export function proximoACadaNSemanas(
  diaSemana: number,
  hora: number,
  minuto: number,
  intervaloSemanas: number,
  dataAncora: Date
): Date {
  const agora = new Date();
  const candidata = new Date(dataAncora);
  while (candidata.getDay() !== diaSemana) {
    candidata.setDate(candidata.getDate() + 1);
  }
  candidata.setHours(hora, minuto, 0, 0);

  while (candidata < agora) {
    candidata.setDate(candidata.getDate() + intervaloSemanas * 7);
  }

  return candidata;
}
