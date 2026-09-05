import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { ConfiguracaoContribuicaoAdmin, ConfiguracaoContribuicaoUpsert } from './contribuicao-admin.models';

@Injectable({ providedIn: 'root' })
export class ContribuicaoAdminService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/configuracao-contribuicao`;

  obter(): Observable<ConfiguracaoContribuicaoAdmin> {
    return this.http.get<ConfiguracaoContribuicaoAdmin>(this.baseUrl);
  }

  atualizar(payload: ConfiguracaoContribuicaoUpsert): Observable<ConfiguracaoContribuicaoAdmin> {
    return this.http.put<ConfiguracaoContribuicaoAdmin>(this.baseUrl, payload);
  }
}
