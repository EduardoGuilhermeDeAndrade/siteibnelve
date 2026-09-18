import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { FotoGaleria } from '../../shared/data/galeria.service';

@Injectable({ providedIn: 'root' })
export class GaleriaAdminService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/galeria`;

  listar(): Observable<FotoGaleria[]> {
    return this.http.get<FotoGaleria[]>(this.baseUrl);
  }

  adicionar(arquivo: File, legenda: string): Observable<FotoGaleria> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    if (legenda) {
      formData.append('legenda', legenda);
    }
    return this.http.post<FotoGaleria>(this.baseUrl, formData);
  }

  substituir(id: string, arquivo: File): Observable<FotoGaleria> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    return this.http.post<FotoGaleria>(`${this.baseUrl}/${id}/substituir`, formData);
  }

  atualizarMetadados(id: string, legenda: string | null, ordem: number): Observable<FotoGaleria> {
    return this.http.put<FotoGaleria>(`${this.baseUrl}/${id}`, { legenda, ordem });
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
