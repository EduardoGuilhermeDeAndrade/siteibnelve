import { FotoGaleria } from '../../shared/data/galeria.service';

export interface FotoGaleriaEditavel extends FotoGaleria {
  legendaRascunho: string;
  ordemRascunho: number;
  salvandoMetadados: boolean;
  sucessoMetadados: boolean;
  erroMetadados: string | null;
}

export const LIMITE_FOTOS = 10;
export { TIPOS_IMAGEM_ACEITOS, TAMANHO_MAXIMO_BYTES } from '../../shared/uploads/validacao-imagem';
