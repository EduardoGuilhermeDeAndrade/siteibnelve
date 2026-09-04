import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';

export interface LocalIgreja {
  id: string;
  nome: string;
  tipo: string;
  endereco: string;
  bairro: string;
  cidade: string;
  estado: string | null;
  googleMapsUrl: string | null;
}

@Injectable({ providedIn: 'root' })
export class LocaisService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/locais`;

  listar(): Observable<LocalIgreja[]> {
    return this.http.get<LocalIgreja[]>(this.baseUrl);
  }
}
