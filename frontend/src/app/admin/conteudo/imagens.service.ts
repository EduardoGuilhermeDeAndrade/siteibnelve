import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { ImagemSite } from './imagem-admin.models';

@Injectable({ providedIn: 'root' })
export class ImagensService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/imagens`;

  listar(): Observable<ImagemSite[]> {
    return this.http.get<ImagemSite[]>(this.baseUrl);
  }

  upload(chave: string, arquivo: File): Observable<ImagemSite> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    return this.http.post<ImagemSite>(`${this.baseUrl}/${encodeURIComponent(chave)}`, formData);
  }
}
