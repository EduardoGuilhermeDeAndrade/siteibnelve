import { FotoGaleria } from '../../shared/data/galeria.service';

export interface FotoGaleriaEditavel extends FotoGaleria {
  legendaRascunho: string;
  ordemRascunho: number;
  salvandoMetadados: boolean;
  sucessoMetadados: boolean;
  erroMetadados: string | null;
}

export const LIMITE_FOTOS = 10;
export const TIPOS_IMAGEM_ACEITOS = ['image/jpeg', 'image/png', 'image/webp'];
export const TAMANHO_MAXIMO_BYTES = 5 * 1024 * 1024;
