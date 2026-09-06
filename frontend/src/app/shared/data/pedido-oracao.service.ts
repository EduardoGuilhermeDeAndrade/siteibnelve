import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';

export interface PedidoOracaoCriar {
  nome: string | null;
  anonimo: boolean;
  contato: string | null;
  desejaFalarComPastor: boolean;
  mensagem: string;
}

@Injectable({ providedIn: 'root' })
export class PedidoOracaoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/pedidos-oracao`;

  enviar(payload: PedidoOracaoCriar): Observable<void> {
    return this.http.post<void>(this.baseUrl, payload);
  }
}
