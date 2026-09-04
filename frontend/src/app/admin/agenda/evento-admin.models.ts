import {
  CATEGORIA_LABELS,
  CategoriaEvento,
  DIA_SEMANA_LABELS,
  STATUS_LABELS,
  StatusEvento,
  VISIBILIDADE_LABELS,
  VisibilidadeEvento
} from '../../shared/data/evento.types';

export type { CategoriaEvento, StatusEvento, VisibilidadeEvento };
export { CATEGORIA_LABELS, DIA_SEMANA_LABELS, STATUS_LABELS, VISIBILIDADE_LABELS };

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
