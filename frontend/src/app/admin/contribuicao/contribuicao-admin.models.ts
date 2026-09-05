import { TIPO_CHAVE_PIX_LABELS, TipoChavePix } from '../../shared/data/contribuicao.types';

export type { TipoChavePix };
export { TIPO_CHAVE_PIX_LABELS };

export interface ConfiguracaoContribuicaoAdmin {
  id: string;
  chavePix: string;
  tipoChave: TipoChavePix;
  favorecido: string;
  dataAtualizacao: string | null;
}

export interface ConfiguracaoContribuicaoUpsert {
  chavePix: string;
  tipoChave: TipoChavePix;
  favorecido: string;
}
