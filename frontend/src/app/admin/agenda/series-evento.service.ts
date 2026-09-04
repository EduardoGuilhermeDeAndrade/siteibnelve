import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { ExcecaoUpsert, Ocorrencia, SerieEventoAdmin, SerieEventoUpsert } from './serie-evento-admin.models';

@Injectable({ providedIn: 'root' })
export class SeriesEventoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/series-evento`;

  listar(): Observable<SerieEventoAdmin[]> {
    return this.http.get<SerieEventoAdmin[]>(this.baseUrl);
  }

  obter(id: string): Observable<SerieEventoAdmin> {
    return this.http.get<SerieEventoAdmin>(`${this.baseUrl}/${id}`);
  }

  criar(payload: SerieEventoUpsert): Observable<SerieEventoAdmin> {
    return this.http.post<SerieEventoAdmin>(this.baseUrl, payload);
  }

  atualizar(id: string, payload: SerieEventoUpsert): Observable<SerieEventoAdmin> {
    return this.http.put<SerieEventoAdmin>(`${this.baseUrl}/${id}`, payload);
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /** Prévia de ocorrências calculadas numa janela — nunca persistidas. */
  obterOcorrencias(id: string, desde?: string, ate?: string): Observable<Ocorrencia[]> {
    let params = new HttpParams();
    if (desde) {
      params = params.set('desde', desde);
    }
    if (ate) {
      params = params.set('ate', ate);
    }
    return this.http.get<Ocorrencia[]>(`${this.baseUrl}/${id}/ocorrencias`, { params });
  }

  /** Escopo "somente esta ocorrência". */
  salvarExcecao(id: string, data: string, payload: ExcecaoUpsert): Observable<Ocorrencia> {
    return this.http.put<Ocorrencia>(`${this.baseUrl}/${id}/ocorrencias/${data}`, payload);
  }

  removerExcecao(id: string, data: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}/ocorrencias/${data}`);
  }

  /** Escopo "esta e as próximas": encerra a série atual e cria uma nova a partir de apartirDe. */
  dividirSerie(id: string, apartirDe: string, payload: SerieEventoUpsert): Observable<SerieEventoAdmin> {
    const params = new HttpParams().set('apartirDe', apartirDe);
    return this.http.post<SerieEventoAdmin>(`${this.baseUrl}/${id}/dividir`, payload, { params });
  }
}
