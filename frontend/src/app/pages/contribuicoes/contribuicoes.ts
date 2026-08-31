import { Component, signal } from '@angular/core';

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
