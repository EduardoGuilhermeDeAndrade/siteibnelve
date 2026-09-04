import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { MinisterioAdmin } from './ministerio-admin.models';
import { MinisteriosService } from './ministerios.service';

@Component({
  selector: 'app-ministerios-admin',
  imports: [RouterLink],
  templateUrl: './ministerios-admin.html',
  styleUrl: './ministerios-admin.css'
})
export class MinisteriosAdmin {
  private readonly ministeriosService = inject(MinisteriosService);

  protected readonly ministerios = signal<MinisterioAdmin[]>([]);
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

    this.ministeriosService.listar().subscribe({
      next: (ministerios) => {
        this.ministerios.set(ministerios);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os ministérios.');
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

    this.ministeriosService.excluir(id).subscribe({
      next: () => {
        this.ministerios.update((lista) => lista.filter((m) => m.id !== id));
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      },
      error: () => {
        this.erro.set('Não foi possível excluir o ministério.');
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      }
    });
  }
}
