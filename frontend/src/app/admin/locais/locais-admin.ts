import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { LocalAdmin } from './local-admin.models';
import { LocaisService } from './locais.service';

@Component({
  selector: 'app-locais-admin',
  imports: [RouterLink],
  templateUrl: './locais-admin.html',
  styleUrl: './locais-admin.css'
})
export class LocaisAdmin {
  private readonly locaisService = inject(LocaisService);

  protected readonly locais = signal<LocalAdmin[]>([]);
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

    this.locaisService.listar().subscribe({
      next: (locais) => {
        this.locais.set(locais);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os locais.');
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

    this.locaisService.excluir(id).subscribe({
      next: () => {
        this.locais.update((lista) => lista.filter((l) => l.id !== id));
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      },
      error: () => {
        this.erro.set('Não foi possível excluir o local.');
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      }
    });
  }
}
