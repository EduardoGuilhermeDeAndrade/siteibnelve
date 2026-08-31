import {
  CategoriaEvento,
  StatusEvento,
  VisibilidadeEvento
} from '../../shared/data/eventos.mock';

export type { CategoriaEvento, StatusEvento, VisibilidadeEvento };

export interface EventoAdmin {
  id: string;
  titulo: string;
  descricao: string | null;
  categoria: CategoriaEvento;
  visibilidade: VisibilidadeEvento;
  status: StatusEvento;
  dataHoraInicio: string;
  dataHoraFim: string | null;
  localId: string | null;
  localNome: string | null;
  localTexto: string | null;
  recorrenciaSemanal: boolean;
  diaSemana: number | null;
  imagemUrl: string | null;
}

export interface EventoUpsert {
  titulo: string;
  descricao: string | null;
  categoria: CategoriaEvento;
  visibilidade: VisibilidadeEvento;
  status: StatusEvento;
  dataHoraInicio: string;
  dataHoraFim: string | null;
  localId: string | null;
  localTexto: string | null;
  recorrenciaSemanal: boolean;
  diaSemana: number | null;
}

export const CATEGORIA_LABELS: Record<CategoriaEvento, string> = {
  CULTO: 'Culto',
  CEIA: 'Ceia',
  ESCOLA_BIBLICA: 'Escola Bíblica',
  JOVENS: 'Mocidade',
  MULHERES: 'Mulheres',
  HOMENS: 'Homens',
  OBREIROS: 'Obreiros',
  LIDERANCA: 'Liderança',
  BATISMO: 'Batismo',
  CASAMENTO: 'Casamento',
  REUNIAO: 'Reunião',
  EVENTO_ESPECIAL: 'Evento especial',
  ADMINISTRATIVO: 'Administrativo',
  PESSOAL: 'Pessoal'
};

export const VISIBILIDADE_LABELS: Record<VisibilidadeEvento, string> = {
  PUBLICO: 'Público',
  INTERNO: 'Interno',
  PESSOAL: 'Pessoal'
};

export const STATUS_LABELS: Record<StatusEvento, string> = {
  RASCUNHO: 'Rascunho',
  PUBLICADO: 'Publicado',
  CANCELADO: 'Cancelado'
};

export const DIA_SEMANA_LABELS = [
  'Domingo',
  'Segunda-feira',
  'Terça-feira',
  'Quarta-feira',
  'Quinta-feira',
  'Sexta-feira',
  'Sábado'
];
