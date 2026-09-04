import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';

export interface Ministerio {
  id: string;
  nome: string;
  lideres: string;
  descricao: string | null;
  imagemUrl: string | null;
}

@Injectable({ providedIn: 'root' })
export class MinisteriosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/ministerios`;

  listar(): Observable<Ministerio[]> {
    return this.http.get<Ministerio[]>(this.baseUrl);
  }
}
