import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { LocalAdmin, LocalUpsert } from './local-admin.models';

@Injectable({ providedIn: 'root' })
export class LocaisService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/locais`;

  listar(): Observable<LocalAdmin[]> {
    return this.http.get<LocalAdmin[]>(this.baseUrl);
  }

  obter(id: string): Observable<LocalAdmin> {
    return this.http.get<LocalAdmin>(`${this.baseUrl}/${id}`);
  }

  criar(payload: LocalUpsert): Observable<LocalAdmin> {
    return this.http.post<LocalAdmin>(this.baseUrl, payload);
  }

  atualizar(id: string, payload: LocalUpsert): Observable<LocalAdmin> {
    return this.http.put<LocalAdmin>(`${this.baseUrl}/${id}`, payload);
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
