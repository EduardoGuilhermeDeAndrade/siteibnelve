import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { MinisterioAdmin, MinisterioUpsert } from './ministerio-admin.models';

@Injectable({ providedIn: 'root' })
export class MinisteriosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/ministerios`;

  listar(): Observable<MinisterioAdmin[]> {
    return this.http.get<MinisterioAdmin[]>(this.baseUrl);
  }

  obter(id: string): Observable<MinisterioAdmin> {
    return this.http.get<MinisterioAdmin>(`${this.baseUrl}/${id}`);
  }

  criar(payload: MinisterioUpsert): Observable<MinisterioAdmin> {
    return this.http.post<MinisterioAdmin>(this.baseUrl, payload);
  }

  atualizar(id: string, payload: MinisterioUpsert): Observable<MinisterioAdmin> {
    return this.http.put<MinisterioAdmin>(`${this.baseUrl}/${id}`, payload);
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
