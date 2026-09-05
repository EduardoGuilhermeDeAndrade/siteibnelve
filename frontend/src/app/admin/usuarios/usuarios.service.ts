import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { UsuarioAdmin, UsuarioAtualizar, UsuarioCriar } from './usuario-admin.models';

@Injectable({ providedIn: 'root' })
export class UsuariosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/usuarios`;

  listar(): Observable<UsuarioAdmin[]> {
    return this.http.get<UsuarioAdmin[]>(this.baseUrl);
  }

  criar(payload: UsuarioCriar): Observable<UsuarioAdmin> {
    return this.http.post<UsuarioAdmin>(this.baseUrl, payload);
  }

  atualizar(id: string, payload: UsuarioAtualizar): Observable<UsuarioAdmin> {
    return this.http.put<UsuarioAdmin>(`${this.baseUrl}/${id}`, payload);
  }

  ativar(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/ativar`, {});
  }

  desativar(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/desativar`, {});
  }

  redefinirSenha(id: string, novaSenha: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/redefinir-senha`, { novaSenha });
  }
}
