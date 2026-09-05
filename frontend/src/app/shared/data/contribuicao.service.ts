import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';

export interface ConfiguracaoContribuicao {
  id: string;
  chavePix: string;
  tipoChave: string;
  favorecido: string;
  dataAtualizacao: string | null;
}

@Injectable({ providedIn: 'root' })
export class ContribuicaoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/configuracao-contribuicao`;

  obter(): Observable<ConfiguracaoContribuicao> {
    return this.http.get<ConfiguracaoContribuicao>(this.baseUrl);
  }
}
