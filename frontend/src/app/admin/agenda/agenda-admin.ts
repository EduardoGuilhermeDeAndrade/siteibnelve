import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import {
  CATEGORIA_LABELS,
  EventoAdmin,
  STATUS_LABELS,
  VISIBILIDADE_LABELS
} from './evento-admin.models';
import { EventosService } from './eventos.service';

@Component({
  selector: 'app-agenda-admin',
  imports: [RouterLink, DatePipe],
  templateUrl: './agenda-admin.html',
  styleUrl: './agenda-admin.css'
})
export class AgendaAdmin {
  private readonly eventosService = inject(EventosService);

  protected readonly categoriaLabels = CATEGORIA_LABELS;
  protected readonly visibilidadeLabels = VISIBILIDADE_LABELS;
  protected readonly statusLabels = STATUS_LABELS;

  protected readonly eventos = signal<EventoAdmin[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly excluindoId = signal<string | null>(null);
  protected readonly confirmandoId = signal<string | null>(null);

  constructor() {
    this.carregar();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.eventosService.listar().subscribe({
      next: (eventos) => {
        this.eventos.set(eventos);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar a agenda.');
        this.carregando.set(false);
      }
    });
  }

  protected pedirConfirmacao(id: string): void {
    this.confirmandoId.set(id);
  }

  protected cancelarConfirmacao(): void {
    this.confirmandoId.set(null);
  }

  protected excluir(id: string): void {
    this.excluindoId.set(id);

    this.eventosService.excluir(id).subscribe({
      next: () => {
        this.eventos.update((lista) => lista.filter((e) => e.id !== id));
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      },
      error: () => {
        this.erro.set('Não foi possível excluir o evento.');
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      }
    });
  }
}
