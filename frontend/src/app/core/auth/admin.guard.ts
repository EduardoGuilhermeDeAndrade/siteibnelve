import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from './auth.service';

/**
 * Bloqueia /admin/** sem sessão válida + role ADMIN. Como o access token vive só
 * em memória, um F5 na página derruba a sessão e volta pro login — o refresh
 * automático via cookie httpOnly fica para um próximo passo (interceptor de 401).
 */
export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.autenticado() && auth.isAdmin() ? true : router.createUrlTree(['/admin/login']);
};
