import { Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { LocalIgreja, LocaisService } from '../../shared/data/locais.service';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-contato',
  imports: [FormsModule, SectionHeading],
  templateUrl: './contato.html',
  styleUrl: './contato.css'
})
export class Contato {
  @ViewChild('mensagemInput') private mensagemInput?: ElementRef<HTMLTextAreaElement>;

  private readonly locaisService = inject(LocaisService);

  protected readonly locais = signal<LocalIgreja[]>([]);
  protected readonly carregandoLocais = signal(true);
  protected readonly erroLocais = signal(false);

  protected nome = '';
  protected mensagem = '';
  protected anonimo = false;

  protected readonly enviado = signal(false);
  protected readonly erroMensagem = signal(false);

  constructor() {
    this.locaisService.listar().subscribe({
      next: (locais) => {
        this.locais.set(locais);
        this.carregandoLocais.set(false);
      },
      error: () => {
        this.erroLocais.set(true);
        this.carregandoLocais.set(false);
      }
    });
  }

  protected enviarPedido(): void {
    if (!this.mensagem.trim()) {
      this.erroMensagem.set(true);
      this.mensagemInput?.nativeElement.focus();
      return;
    }

    this.erroMensagem.set(false);
    this.enviado.set(true);
    this.nome = '';
    this.mensagem = '';
    this.anonimo = false;
  }
}
