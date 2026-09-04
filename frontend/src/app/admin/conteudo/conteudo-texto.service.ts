import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { ConteudoTexto, ConteudoTextoUpsert } from './conteudo-texto.models';

@Injectable({ providedIn: 'root' })
export class ConteudoTextoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/conteudo-texto`;

  listar(): Observable<ConteudoTexto[]> {
    return this.http.get<ConteudoTexto[]>(this.baseUrl);
  }

  salvar(chave: string, payload: ConteudoTextoUpsert): Observable<ConteudoTexto> {
    return this.http.put<ConteudoTexto>(`${this.baseUrl}/${encodeURIComponent(chave)}`, payload);
  }
}
