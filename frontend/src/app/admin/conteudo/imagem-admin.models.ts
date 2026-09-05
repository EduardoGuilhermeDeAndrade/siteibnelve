export interface ImagemSite {
  chave: string;
  url: string;
  dataAtualizacao: string;
}

/** Chaves já usadas (ou previstas) pelo site público — sugestão, não trava outras. */
export const CHAVES_SUGERIDAS = ['home-hero', 'contribuicao-qrcode'];

export const TIPOS_IMAGEM_ACEITOS = ['image/jpeg', 'image/png', 'image/webp'];
export const TAMANHO_MAXIMO_BYTES = 5 * 1024 * 1024;
