import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { PedidoOracaoAdmin } from './pedido-oracao-admin.models';

@Injectable({ providedIn: 'root' })
export class PedidosOracaoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/pedidos-oracao`;

  listar(): Observable<PedidoOracaoAdmin[]> {
    return this.http.get<PedidoOracaoAdmin[]>(this.baseUrl);
  }

  marcarLido(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/marcar-lido`, {});
  }

  marcarNaoLido(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/marcar-nao-lido`, {});
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
