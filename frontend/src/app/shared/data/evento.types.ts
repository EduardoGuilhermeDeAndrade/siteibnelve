/**
 * Tipos e rótulos de Evento compartilhados entre o site público (shared/data/eventos.service.ts)
 * e o Portal Admin (admin/agenda/evento-admin.models.ts) — mesmos valores que a API usa
 * (Ibnelve.Api.Domain.EnumMappings), assim os dois lados falam a mesma língua sem duplicar strings.
 */
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
