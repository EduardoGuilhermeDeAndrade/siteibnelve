import { HttpErrorResponse } from '@angular/common/http';

/**
 * Extrai uma mensagem legível de um erro HTTP da API: usa a mensagem própria da
 * API quando existe, junta os erros de validação automáticos do ASP.NET Core
 * (ValidationProblemDetails) quando é esse o caso, e só cai no fallback quando
 * não sobra nada melhor pra mostrar.
 */
export function mensagemDeErro(erro: HttpErrorResponse, fallback: string): string {
  if (erro.status === 0) {
    return 'Não foi possível conectar ao servidor. Verifique sua internet e tente de novo.';
  }

  const corpo = erro.error as { message?: unknown; errors?: Record<string, unknown> } | null;

  if (typeof corpo?.message === 'string' && corpo.message) {
    return corpo.message;
  }

  if (corpo?.errors && typeof corpo.errors === 'object') {
    const mensagens = Object.values(corpo.errors)
      .flat()
      .filter((m): m is string => typeof m === 'string');
    if (mensagens.length > 0) {
      return mensagens.join(' ');
    }
  }

  return fallback;
}
