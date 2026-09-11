import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Meta } from '@angular/platform-browser';

import { mensagemDeErro } from '../../shared/errors/mensagem-erro';
import { LocalIgreja, LocaisService } from '../../shared/data/locais.service';
import { PedidoOracaoService } from '../../shared/data/pedido-oracao.service';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-contato',
  imports: [FormsModule, SectionHeading],
  templateUrl: './contato.html',
  styleUrl: './contato.css'
})
export class Contato {
  @ViewChild('mensagemInput') private mensagemInput?: ElementRef<HTMLTextAreaElement>;
  @ViewChild('contatoInput') private contatoInput?: ElementRef<HTMLInputElement>;

  private readonly locaisService = inject(LocaisService);
  private readonly pedidoOracaoService = inject(PedidoOracaoService);

  protected readonly locais = signal<LocalIgreja[]>([]);
  protected readonly carregandoLocais = signal(true);
  protected readonly erroLocais = signal(false);

  protected nome = '';
  protected contato = '';
  protected desejaFalarComPastor = false;
  protected mensagem = '';
  protected anonimo = false;

  protected readonly enviado = signal(false);
  protected readonly enviando = signal(false);
  protected readonly erroMensagem = signal(false);
  protected readonly erroContato = signal(false);
  protected readonly erroEnvio = signal<string | null>(null);

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content: 'Fale com a IBNELVE: endereços dos templos Vereda e Liberdade, e envie um pedido de oração.'
    });

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

    if (this.desejaFalarComPastor && !this.contato.trim()) {
      this.erroContato.set(true);
      this.contatoInput?.nativeElement.focus();
      return;
    }
    this.erroContato.set(false);

    this.erroEnvio.set(null);
    this.enviando.set(true);

    this.pedidoOracaoService
      .enviar({
        nome: this.anonimo ? null : this.nome.trim() || null,
        anonimo: this.anonimo,
        contato: this.contato.trim() || null,
        desejaFalarComPastor: this.desejaFalarComPastor,
        mensagem: this.mensagem.trim()
      })
      .subscribe({
        next: () => {
          this.enviando.set(false);
          this.enviado.set(true);
          this.nome = '';
          this.contato = '';
          this.desejaFalarComPastor = false;
          this.mensagem = '';
          this.anonimo = false;
        },
        error: (erro: HttpErrorResponse) => {
          this.erroEnvio.set(mensagemDeErro(erro, 'Não foi possível enviar seu pedido agora. Tente novamente.'));
          this.enviando.set(false);
        }
      });
  }
}
