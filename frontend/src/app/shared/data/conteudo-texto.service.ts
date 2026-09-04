import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';

export interface ConteudoTexto {
  chave: string;
  titulo: string;
  conteudo: string;
}

@Injectable({ providedIn: 'root' })
export class ConteudoTextoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/conteudo-texto`;

  listar(): Observable<ConteudoTexto[]> {
    return this.http.get<ConteudoTexto[]>(this.baseUrl);
  }

  /** Retorna um mapa chave -> conteúdo, mais prático para os templates. */
  listarComoMapa(): Observable<Record<string, string>> {
    return this.listar().pipe(
      map((itens) => Object.fromEntries(itens.map((item) => [item.chave, item.conteudo])))
    );
  }
}
