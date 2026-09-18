import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../core/api-config';
import {
  EmprestimoPatrimonio,
  ItemPatrimonio,
  ItemPatrimonioAtualizar,
  ItemPatrimonioDevolucao,
  ItemPatrimonioEmprestimo,
  ItemPatrimonioSituacao,
  TipoControlePatrimonio
} from './patrimonio-admin.models';

export interface ItemPatrimonioCriar {
  descricao: string;
  numeroPatrimonio: string | null;
  tipoControle: TipoControlePatrimonio;
  quantidadeTotal: number;
  localId: string | null;
  observacao: string | null;
}

@Injectable({ providedIn: 'root' })
export class PatrimonioService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE_URL}/admin/patrimonio`;

  listar(): Observable<ItemPatrimonio[]> {
    return this.http.get<ItemPatrimonio[]>(this.baseUrl);
  }

  obter(id: string): Observable<ItemPatrimonio> {
    return this.http.get<ItemPatrimonio>(`${this.baseUrl}/${id}`);
  }

  listarEmprestimos(id: string): Observable<EmprestimoPatrimonio[]> {
    return this.http.get<EmprestimoPatrimonio[]>(`${this.baseUrl}/${id}/emprestimos`);
  }

  criar(dados: ItemPatrimonioCriar, arquivo: File | null): Observable<ItemPatrimonio> {
    const formData = new FormData();
    formData.append('descricao', dados.descricao);
    if (dados.numeroPatrimonio) {
      formData.append('numeroPatrimonio', dados.numeroPatrimonio);
    }
    formData.append('tipoControle', dados.tipoControle);
    formData.append('quantidadeTotal', String(dados.quantidadeTotal));
    if (dados.localId) {
      formData.append('localId', dados.localId);
    }
    if (dados.observacao) {
      formData.append('observacao', dados.observacao);
    }
    if (arquivo) {
      formData.append('arquivo', arquivo);
    }
    return this.http.post<ItemPatrimonio>(this.baseUrl, formData);
  }

  atualizar(id: string, dados: ItemPatrimonioAtualizar): Observable<ItemPatrimonio> {
    return this.http.put<ItemPatrimonio>(`${this.baseUrl}/${id}`, dados);
  }

  substituirFoto(id: string, arquivo: File): Observable<ItemPatrimonio> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    return this.http.post<ItemPatrimonio>(`${this.baseUrl}/${id}/foto`, formData);
  }

  alterarSituacao(id: string, dados: ItemPatrimonioSituacao): Observable<ItemPatrimonio> {
    return this.http.put<ItemPatrimonio>(`${this.baseUrl}/${id}/situacao`, dados);
  }

  darBaixa(id: string, fotoDefeito: File, observacaoBaixa: string): Observable<ItemPatrimonio> {
    const formData = new FormData();
    formData.append('fotoDefeito', fotoDefeito);
    formData.append('observacaoBaixa', observacaoBaixa);
    return this.http.post<ItemPatrimonio>(`${this.baseUrl}/${id}/baixa`, formData);
  }

  emprestar(id: string, dados: ItemPatrimonioEmprestimo): Observable<EmprestimoPatrimonio> {
    return this.http.post<EmprestimoPatrimonio>(`${this.baseUrl}/${id}/emprestimos`, dados);
  }

  devolver(id: string, emprestimoId: string, dados: ItemPatrimonioDevolucao): Observable<EmprestimoPatrimonio> {
    return this.http.put<EmprestimoPatrimonio>(`${this.baseUrl}/${id}/emprestimos/${emprestimoId}/devolucao`, dados);
  }
}
