import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import { LocalAdmin } from './local-admin.models';

@Injectable({ providedIn: 'root' })
export class LocaisService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/locais`;

  listar(): Observable<LocalAdmin[]> {
    return this.http.get<LocalAdmin[]>(this.baseUrl);
  }
}
