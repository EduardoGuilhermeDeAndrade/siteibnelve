import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  CHAVES_SUGERIDAS,
  ImagemSite,
  TAMANHO_MAXIMO_BYTES,
  TIPOS_IMAGEM_ACEITOS
} from './imagem-admin.models';
import { ImagensService } from './imagens.service';
import { TextoEditavelState } from './conteudo-texto.models';
import { ConteudoTextoService } from './conteudo-texto.service';

@Component({
  selector: 'app-conteudo-admin',
  imports: [DatePipe, FormsModule],
  templateUrl: './conteudo-admin.html',
  styleUrl: './conteudo-admin.css'
})
export class ConteudoAdmin {
  private readonly imagensService = inject(ImagensService);
  private readonly conteudoTextoService = inject(ConteudoTextoService);

  protected readonly chavesSugeridas = CHAVES_SUGERIDAS;

  protected readonly imagens = signal<ImagemSite[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly enviandoChave = signal<string | null>(null);
  protected readonly sucessoChave = signal<string | null>(null);
  protected readonly erroValidacao = signal<string | null>(null);

  protected readonly textos = signal<TextoEditavelState[]>([]);
  protected readonly carregandoTextos = signal(true);
  protected readonly erroTextos = signal<string | null>(null);

  protected novaChave = '';

  constructor() {
    this.carregar();
    this.carregarTextos();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.imagensService.listar().subscribe({
      next: (imagens) => {
        this.imagens.set(imagens);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar as imagens.');
        this.carregando.set(false);
      }
    });
  }

  private carregarTextos(): void {
    this.carregandoTextos.set(true);
    this.erroTextos.set(null);

    this.conteudoTextoService.listar().subscribe({
      next: (textos) => {
        this.textos.set(
          textos.map((texto) => ({ ...texto, rascunho: texto.conteudo, salvando: false, sucesso: false, erro: null }))
        );
        this.carregandoTextos.set(false);
      },
      error: () => {
        this.erroTextos.set('Não foi possível carregar os textos institucionais.');
        this.carregandoTextos.set(false);
      }
    });
  }

  protected salvarTexto(item: TextoEditavelState): void {
    item.salvando = true;
    item.erro = null;

    this.conteudoTextoService.salvar(item.chave, { titulo: item.titulo, conteudo: item.rascunho }).subscribe({
      next: (atualizado) => {
        item.conteudo = atualizado.conteudo;
        item.dataAtualizacao = atualizado.dataAtualizacao;
        item.salvando = false;
        item.sucesso = true;
        setTimeout(() => {
          item.sucesso = false;
        }, 4000);
      },
      error: () => {
        item.salvando = false;
        item.erro = 'Não foi possível salvar este texto.';
      }
    });
  }

  private validarArquivo(arquivo: File): string | null {
    if (!TIPOS_IMAGEM_ACEITOS.includes(arquivo.type)) {
      return 'Formato não suportado. Envie JPEG, PNG ou WebP.';
    }
    if (arquivo.size > TAMANHO_MAXIMO_BYTES) {
      return 'Arquivo maior que o limite de 5 MB.';
    }
    return null;
  }

  protected onSelecionarArquivoExistente(chave: string, evento: Event): void {
    const input = evento.target as HTMLInputElement;
    const arquivo = input.files?.[0];
    input.value = '';
    if (arquivo) {
      this.enviar(chave, arquivo);
    }
  }

  protected onSelecionarArquivoNovo(evento: Event): void {
    const chave = this.novaChave.trim();
    const input = evento.target as HTMLInputElement;
    const arquivo = input.files?.[0];
    input.value = '';

    if (!chave) {
      this.erroValidacao.set('Informe uma chave para a nova imagem antes de escolher o arquivo.');
      return;
    }
    if (arquivo) {
      this.enviar(chave, arquivo);
    }
  }

  private enviar(chave: string, arquivo: File): void {
    const problema = this.validarArquivo(arquivo);
    if (problema) {
      this.erroValidacao.set(problema);
      return;
    }

    this.erroValidacao.set(null);
    this.erro.set(null);
    this.enviandoChave.set(chave);

    this.imagensService.upload(chave, arquivo).subscribe({
      next: (imagem) => {
        this.imagens.update((lista) => {
          const existe = lista.some((i) => i.chave === imagem.chave);
          return existe
            ? lista.map((i) => (i.chave === imagem.chave ? imagem : i))
            : [...lista, imagem];
        });
        this.enviandoChave.set(null);
        this.sucessoChave.set(chave);
        setTimeout(() => this.sucessoChave.set(null), 4000);
      },
      error: (erro: { error?: { message?: string } }) => {
        this.erro.set(erro?.error?.message ?? 'Não foi possível enviar a imagem.');
        this.enviandoChave.set(null);
      }
    });
  }
}
