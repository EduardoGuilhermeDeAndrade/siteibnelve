import {
  proximoACadaNSemanas,
  proximoDiaSemana,
  proximoPrimeiroDiaSemanaDoMes
} from './recorrencia.util';

export type CategoriaEvento =
  | 'CULTO'
  | 'CEIA'
  | 'ESCOLA_BIBLICA'
  | 'JOVENS'
  | 'MULHERES'
  | 'HOMENS'
  | 'OBREIROS'
  | 'LIDERANCA'
  | 'BATISMO'
  | 'CASAMENTO'
  | 'REUNIAO'
  | 'EVENTO_ESPECIAL'
  | 'ADMINISTRATIVO'
  | 'PESSOAL';

export type VisibilidadeEvento = 'PUBLICO' | 'INTERNO' | 'PESSOAL';
export type StatusEvento = 'RASCUNHO' | 'PUBLICADO' | 'CANCELADO';

export interface EventoAgenda {
  id: string;
  titulo: string;
  categoria: CategoriaEvento;
  categoriaLabel: string;
  visibilidade: VisibilidadeEvento;
  status: StatusEvento;
  local: string;
  localObservacao?: string;
  data: Date;
  descricao: string;
}

// Data-âncora usada só para fixar em qual sábado cai a quinzena da Mocidade no mock.
const ANCORA_MOCIDADE = new Date(2026, 7, 1);

/**
 * Dados de exemplo (mock) da Agenda, calculados a partir de agora com as regras
 * de recorrência descritas no CLAUDE.md. Serão substituídos por SerieEvento/
 * OcorrenciaEvento vindos da API nas Fases 3, 4 e 6.
 */
const eventosBase: EventoAgenda[] = [
  {
    id: 'culto-domingo',
    titulo: 'Culto de Louvor e Adoração',
    categoria: 'CULTO',
    categoriaLabel: 'Culto',
    visibilidade: 'PUBLICO',
    status: 'PUBLICADO',
    local: 'Templo Vereda',
    localObservacao: 'local a confirmar',
    data: proximoDiaSemana(0, 18, 0, { excetoPrimeiraOcorrenciaDoMes: true }),
    descricao: 'Todo domingo às 18h, exceto no primeiro domingo do mês (Santa Ceia).'
  },
  {
    id: 'ceia',
    titulo: 'Santa Ceia',
    categoria: 'CEIA',
    categoriaLabel: 'Ceia',
    visibilidade: 'PUBLICO',
    status: 'PUBLICADO',
    local: 'Templo Liberdade',
    data: proximoPrimeiroDiaSemanaDoMes(0, 8, 30),
    descricao: 'No primeiro domingo de cada mês, às 08h30.'
  },
  {
    id: 'ebd',
    titulo: 'Escola Bíblica Dominical',
    categoria: 'ESCOLA_BIBLICA',
    categoriaLabel: 'Escola Bíblica',
    visibilidade: 'PUBLICO',
    status: 'PUBLICADO',
    local: 'Templo Liberdade',
    data: proximoDiaSemana(0, 9, 0),
    descricao: 'Todo domingo pela manhã.'
  },
  {
    id: 'mocidade',
    titulo: 'Encontro da Mocidade',
    categoria: 'JOVENS',
    categoriaLabel: 'Mocidade',
    visibilidade: 'PUBLICO',
    status: 'PUBLICADO',
    local: 'Templo Vereda',
    localObservacao: 'horário a confirmar',
    data: proximoACadaNSemanas(6, 19, 0, 2, ANCORA_MOCIDADE),
    descricao: 'A cada duas semanas, aos sábados.'
  }
];

export const EVENTOS_MOCK: EventoAgenda[] = [...eventosBase].sort(
  (a, b) => a.data.getTime() - b.data.getTime()
);

export function proximosEventosPublicos(limite: number): EventoAgenda[] {
  return EVENTOS_MOCK.filter(
    (evento) => evento.visibilidade === 'PUBLICO' && evento.status === 'PUBLICADO'
  ).slice(0, limite);
}
