import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService, PAPEL_PATRIMONIO_EDITOR } from './auth.service';

/**
 * Bloqueia /admin/** sem sessão válida ou sem nenhum papel reconhecido do Portal
 * (ADMIN ou PATRIMONIO_EDITOR — ver CLAUDE.md, decisão de 2026-09-18). Como o
 * access token vive só em memória, um F5 na página derruba a sessão e volta pro
 * login — o refresh automático via cookie httpOnly fica para um próximo passo
 * (interceptor de 401).
 */
export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const temAcesso = auth.autenticado() && (auth.isAdmin() || auth.temPapel(PAPEL_PATRIMONIO_EDITOR));
  return temAcesso ? true : router.createUrlTree(['/admin/login']);
};

/**
 * Bloqueia rotas restritas a ADMIN (tudo no Portal exceto Patrimônio). Um usuário
 * autenticado mas sem ADMIN (ex.: só PATRIMONIO_EDITOR) é redirecionado pra
 * Patrimônio em vez de pro login, já que ele tem sessão válida — só não pode ver
 * essa tela específica.
 */
export const adminOnlyGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.isAdmin() ? true : router.createUrlTree(['/admin/patrimonio']);
};
