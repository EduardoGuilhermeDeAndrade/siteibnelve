import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { CATEGORIA_LABELS, CategoriaEvento } from './evento.types';

interface EventoPublicoResponse {
  id: string;
  titulo: string;
  descricao: string | null;
  categoria: CategoriaEvento;
  data: string;
  local: string | null;
  recorrenciaSemanal: boolean;
}

export interface EventoAgenda {
  id: string;
  titulo: string;
  descricao: string | null;
  categoria: CategoriaEvento;
  categoriaLabel: string;
  data: Date;
  local: string | null;
  recorrenciaSemanal: boolean;
}

@Injectable({ providedIn: 'root' })
export class EventosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/eventos`;

  listar(limite?: number): Observable<EventoAgenda[]> {
    const params = limite ? { limite: limite.toString() } : undefined;
    return this.http
      .get<EventoPublicoResponse[]>(this.baseUrl, { params })
      .pipe(map((eventos) => eventos.map(paraEventoAgenda)));
  }
}

function paraEventoAgenda(evento: EventoPublicoResponse): EventoAgenda {
  return {
    id: evento.id,
    titulo: evento.titulo,
    descricao: evento.descricao,
    categoria: evento.categoria,
    categoriaLabel: CATEGORIA_LABELS[evento.categoria] ?? evento.categoria,
    data: new Date(evento.data),
    local: evento.local,
    recorrenciaSemanal: evento.recorrenciaSemanal
  };
}
