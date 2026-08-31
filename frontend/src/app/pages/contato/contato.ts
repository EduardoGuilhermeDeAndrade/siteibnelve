import { Component, ElementRef, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { LOCAIS_MOCK } from '../../shared/data/locais.mock';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-contato',
  imports: [FormsModule, SectionHeading],
  templateUrl: './contato.html',
  styleUrl: './contato.css'
})
export class Contato {
  @ViewChild('mensagemInput') private mensagemInput?: ElementRef<HTMLTextAreaElement>;

  protected readonly locais = LOCAIS_MOCK;

  protected nome = '';
  protected mensagem = '';
  protected anonimo = false;

  protected readonly enviado = signal(false);
  protected readonly erroMensagem = signal(false);

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
