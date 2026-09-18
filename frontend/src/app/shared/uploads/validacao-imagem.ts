/** Regras de upload de imagem compartilhadas com o backend (ver Services/ValidacaoImagem.cs). */
export const TIPOS_IMAGEM_ACEITOS = ['image/jpeg', 'image/png', 'image/webp'];
export const TAMANHO_MAXIMO_BYTES = 5 * 1024 * 1024;

export function validarArquivoImagem(arquivo: File): string | null {
  if (!TIPOS_IMAGEM_ACEITOS.includes(arquivo.type)) {
    return 'Formato não suportado. Envie JPEG, PNG ou WebP.';
  }
  if (arquivo.size > TAMANHO_MAXIMO_BYTES) {
    return 'Arquivo maior que o limite de 5 MB.';
  }
  return null;
}
