export interface ImagemSite {
  chave: string;
  url: string;
  dataAtualizacao: string;
}

/** Chaves já usadas (ou previstas) pelo site público — sugestão, não trava outras. */
export const CHAVES_SUGERIDAS = ['home-hero', 'contribuicao-qrcode'];

export { TIPOS_IMAGEM_ACEITOS, TAMANHO_MAXIMO_BYTES } from '../../shared/uploads/validacao-imagem';
