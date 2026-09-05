import { Component, inject, signal } from '@angular/core';
import { Meta } from '@angular/platform-browser';

import { ConteudoTextoService } from '../../shared/data/conteudo-texto.service';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-quem-somos',
  imports: [SectionHeading],
  templateUrl: './quem-somos.html',
  styleUrl: './quem-somos.css'
})
export class QuemSomos {
  private readonly conteudoTextoService = inject(ConteudoTextoService);

  protected readonly textos = signal<Record<string, string>>({});
  protected readonly carregando = signal(true);
  protected readonly erro = signal(false);

  /** "Valores" e "No que cremos" são cadastrados como uma linha por item. */
  protected get valores(): string[] {
    return (this.textos()['valores'] ?? '').split('\n').filter(Boolean);
  }

  protected get noQueCremos(): string[] {
    return (this.textos()['no-que-cremos'] ?? '').split('\n').filter(Boolean);
  }

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content:
        'Conheça a história, a missão e os valores da IBNELVE — Igreja Batista Nacional da Esperança do Liberdade e Vereda, em Ribeirão das Neves/MG.'
    });

    this.conteudoTextoService.listarComoMapa().subscribe({
      next: (textos) => {
        this.textos.set(textos);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set(true);
        this.carregando.set(false);
      }
    });
  }
}
