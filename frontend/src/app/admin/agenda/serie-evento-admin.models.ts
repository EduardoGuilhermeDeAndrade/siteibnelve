import {
  CATEGORIA_LABELS,
  CategoriaEvento,
  DIA_SEMANA_LABELS,
  POSICAO_NO_MES_LABELS,
  STATUS_LABELS,
  StatusEvento,
  TIPO_RECORRENCIA_LABELS,
  TipoRecorrencia,
  VISIBILIDADE_LABELS,
  VisibilidadeEvento
} from '../../shared/data/evento.types';

export type { CategoriaEvento, StatusEvento, TipoRecorrencia, VisibilidadeEvento };
export {
  CATEGORIA_LABELS,
  DIA_SEMANA_LABELS,
  POSICAO_NO_MES_LABELS,
  STATUS_LABELS,
  TIPO_RECORRENCIA_LABELS,
  VISIBILIDADE_LABELS
};

export interface SerieEventoAdmin {
  id: string;
  titulo: string;
  descricao: string | null;
  categoria: CategoriaEvento;
  visibilidade: VisibilidadeEvento;
  status: StatusEvento;
  tipoRecorrencia: TipoRecorrencia;
  intervalo: number | null;
  diaSemana: number | null;
  diaMes: number | null;
  posicaoNoMes: number | null;
  /** "HH:mm:ss" */
  horaInicio: string;
  horaFim: string | null;
  /** "yyyy-MM-dd" */
  dataInicioRecorrencia: string;
  dataFimRecorrencia: string | null;
  localId: string | null;
  localNome: string | null;
  localTexto: string | null;
  imagemUrl: string | null;
  destaque: boolean;
  ativo: boolean;
}

export interface SerieEventoUpsert {
  titulo: string;
  descricao: string | null;
  categoria: CategoriaEvento;
  visibilidade: VisibilidadeEvento;
  status: StatusEvento;
  tipoRecorrencia: TipoRecorrencia;
  intervalo: number | null;
  diaSemana: number | null;
  diaMes: number | null;
  posicaoNoMes: number | null;
  horaInicio: string;
  horaFim: string | null;
  dataInicioRecorrencia: string;
  dataFimRecorrencia: string | null;
  localId: string | null;
  localTexto: string | null;
  imagemUrl: string | null;
  destaque: boolean;
  ativo: boolean;
}

/** Ocorrência calculada — nunca persistida, sempre vinda da API. */
export interface Ocorrencia {
  dataOriginal: string;
  dataHoraInicio: string;
  dataHoraFim: string | null;
  titulo: string;
  descricao: string | null;
  localId: string | null;
  localNome: string | null;
  localTexto: string | null;
  cancelada: boolean;
  temExcecao: boolean;
  motivo: string | null;
}

export interface ExcecaoUpsert {
  cancelado: boolean;
  novaData: string | null;
  novaHoraInicio: string | null;
  novaHoraFim: string | null;
  tituloSubstituto: string | null;
  descricaoSubstituta: string | null;
  localSubstitutoId: string | null;
  localTextoSubstituto: string | null;
  motivo: string | null;
}
