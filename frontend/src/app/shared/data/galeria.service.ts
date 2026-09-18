import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';

export interface FotoGaleria {
  id: string;
  url: string;
  legenda: string | null;
  ordem: number;
  dataAtualizacao: string;
}

@Injectable({ providedIn: 'root' })
export class GaleriaService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/galeria`;

  listar(): Observable<FotoGaleria[]> {
    return this.http.get<FotoGaleria[]>(this.baseUrl);
  }
}
