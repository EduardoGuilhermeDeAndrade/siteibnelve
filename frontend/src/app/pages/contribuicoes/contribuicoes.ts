import { Component, inject, signal } from '@angular/core';
import { Meta } from '@angular/platform-browser';

import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-contribuicoes',
  imports: [SectionHeading],
  templateUrl: './contribuicoes.html',
  styleUrl: './contribuicoes.css'
})
export class Contribuicoes {
  protected readonly chavePix = '11.080.185/0001-08';
  protected readonly copiado = signal(false);

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content: 'Contribua com a obra da IBNELVE via PIX e conheça outras formas de apoiar a igreja.'
    });
  }

  protected async copiarChave(): Promise<void> {
    try {
      await navigator.clipboard.writeText(this.chavePix);
      this.copiado.set(true);
      setTimeout(() => this.copiado.set(false), 4000);
    } catch {
      this.copiado.set(false);
    }
  }
}
