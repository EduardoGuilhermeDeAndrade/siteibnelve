import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { PedidoOracaoAdmin } from './pedido-oracao-admin.models';
import { PedidosOracaoService } from './pedidos-oracao.service';

@Component({
  selector: 'app-pedidos-oracao-admin',
  imports: [DatePipe],
  templateUrl: './pedidos-oracao-admin.html',
  styleUrl: './pedidos-oracao-admin.css'
})
export class PedidosOracaoAdmin {
  private readonly pedidosOracaoService = inject(PedidosOracaoService);

  protected readonly pedidos = signal<PedidoOracaoAdmin[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly processandoId = signal<string | null>(null);
  protected readonly confirmandoExclusaoId = signal<string | null>(null);

  constructor() {
    this.carregar();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.pedidosOracaoService.listar().subscribe({
      next: (pedidos) => {
        this.pedidos.set(pedidos);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os pedidos de oração.');
        this.carregando.set(false);
      }
    });
  }

  protected alternarLido(pedido: PedidoOracaoAdmin): void {
    this.processandoId.set(pedido.id);
    this.erro.set(null);

    const requisicao = pedido.lido
      ? this.pedidosOracaoService.marcarNaoLido(pedido.id)
      : this.pedidosOracaoService.marcarLido(pedido.id);

    requisicao.subscribe({
      next: () => {
        this.pedidos.update((lista) =>
          lista.map((p) => (p.id === pedido.id ? { ...p, lido: !pedido.lido } : p))
        );
        this.processandoId.set(null);
      },
      error: () => {
        this.erro.set('Não foi possível atualizar o status do pedido.');
        this.processandoId.set(null);
      }
    });
  }

  protected pedirConfirmacaoExclusao(id: string): void {
    this.confirmandoExclusaoId.set(id);
  }

  protected cancelarExclusao(): void {
    this.confirmandoExclusaoId.set(null);
  }

  protected excluir(id: string): void {
    this.processandoId.set(id);

    this.pedidosOracaoService.excluir(id).subscribe({
      next: () => {
        this.pedidos.update((lista) => lista.filter((p) => p.id !== id));
        this.processandoId.set(null);
        this.confirmandoExclusaoId.set(null);
      },
      error: () => {
        this.erro.set('Não foi possível excluir o pedido.');
        this.processandoId.set(null);
        this.confirmandoExclusaoId.set(null);
      }
    });
  }
}
