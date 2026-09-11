import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { API_BASE_URL } from '../api-config';
import { AuthService } from './auth.service';

/**
 * Anexa o access token nas chamadas para a API de admin e, se a API responder 401
 * (token expirado — dura só 20 min em memória, sem refresh automático ainda),
 * encerra a sessão local e manda pro login em vez de deixar a tela travada com
 * um erro genérico.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const isAdminRequest = req.url.startsWith(`${API_BASE_URL}/admin`);
  const token = auth.accessToken;

  const request = token && isAdminRequest
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(request).pipe(
    catchError((erro: unknown) => {
      if (isAdminRequest && erro instanceof HttpErrorResponse && erro.status === 401) {
        auth.encerrarSessaoLocal();
        router.navigateByUrl('/admin/login?sessaoExpirada=1');
      }
      return throwError(() => erro);
    })
  );
};
