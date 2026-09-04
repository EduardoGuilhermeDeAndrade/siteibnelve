import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';

export interface ImagemSite {
  chave: string;
  url: string;
}

@Injectable({ providedIn: 'root' })
export class ImagensSiteService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/imagens-site`;

  listar(): Observable<ImagemSite[]> {
    return this.http.get<ImagemSite[]>(this.baseUrl);
  }

  /** Busca a URL de uma chave específica; retorna null se não houver imagem cadastrada. */
  buscarUrl(chave: string): Observable<string | null> {
    return this.listar().pipe(
      map((imagens) => imagens.find((imagem) => imagem.chave === chave)?.url ?? null)
    );
  }
}
