import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { API_BASE_URL } from '../api-config';
import { LoginResponse, UsuarioLogado } from './auth.models';

const PAPEL_ADMIN = 'ADMIN';

/**
 * Sessão do Portal Administrativo. O access token vive só em memória (nunca
 * localStorage/sessionStorage) — mitiga roubo de token via XSS. O refresh token
 * fica num cookie httpOnly setado pela API, invisível para este código.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly accessTokenSignal = signal<string | null>(null);
  private readonly usuarioSignal = signal<UsuarioLogado | null>(null);

  readonly usuario = this.usuarioSignal.asReadonly();
  readonly autenticado = computed(() => this.accessTokenSignal() !== null);
  readonly isAdmin = computed(() => this.usuarioSignal()?.roles.includes(PAPEL_ADMIN) ?? false);

  get accessToken(): string | null {
    return this.accessTokenSignal();
  }

  async login(email: string, password: string): Promise<void> {
    const resposta = await firstValueFrom(
      this.http.post<LoginResponse>(
        `${API_BASE_URL}/auth/login`,
        { email, password },
        { withCredentials: true }
      )
    );
    this.accessTokenSignal.set(resposta.accessToken);
    this.usuarioSignal.set(resposta.usuario);
  }

  async logout(): Promise<void> {
    try {
      await firstValueFrom(
        this.http.post(`${API_BASE_URL}/auth/logout`, {}, { withCredentials: true })
      );
    } finally {
      this.accessTokenSignal.set(null);
      this.usuarioSignal.set(null);
    }
  }
}
