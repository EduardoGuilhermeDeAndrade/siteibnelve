import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { EventoAdmin, EventoUpsert } from './evento-admin.models';

@Injectable({ providedIn: 'root' })
export class EventosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/eventos`;

  listar(): Observable<EventoAdmin[]> {
    return this.http.get<EventoAdmin[]>(this.baseUrl);
  }

  obter(id: string): Observable<EventoAdmin> {
    return this.http.get<EventoAdmin>(`${this.baseUrl}/${id}`);
  }

  criar(payload: EventoUpsert): Observable<EventoAdmin> {
    return this.http.post<EventoAdmin>(this.baseUrl, payload);
  }

  atualizar(id: string, payload: EventoUpsert): Observable<EventoAdmin> {
    return this.http.put<EventoAdmin>(`${this.baseUrl}/${id}`, payload);
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
