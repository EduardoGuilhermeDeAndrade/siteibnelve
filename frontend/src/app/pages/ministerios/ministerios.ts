import { Component, inject, signal } from '@angular/core';
import { Meta } from '@angular/platform-browser';

import { Ministerio, MinisteriosService } from '../../shared/data/ministerios.service';
import { MinisterioCard } from '../../shared/ui/ministerio-card/ministerio-card';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-ministerios',
  imports: [MinisterioCard, SectionHeading],
  templateUrl: './ministerios.html'
})
export class Ministerios {
  private readonly ministeriosService = inject(MinisteriosService);

  protected readonly ministerios = signal<Ministerio[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal(false);

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content:
        'Ministérios da IBNELVE: Infantil, Mocidade, Mulheres, Louvor e Diáconos — espaços de serviço e comunhão para todas as idades.'
    });

    this.ministeriosService.listar().subscribe({
      next: (ministerios) => {
        this.ministerios.set(ministerios);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set(true);
        this.carregando.set(false);
      }
    });
  }
}
