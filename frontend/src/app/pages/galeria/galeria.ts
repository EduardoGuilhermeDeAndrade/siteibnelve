import { Component, inject, signal } from '@angular/core';
import { Meta } from '@angular/platform-browser';

import { FotoGaleria, GaleriaService } from '../../shared/data/galeria.service';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

@Component({
  selector: 'app-galeria',
  imports: [SectionHeading],
  templateUrl: './galeria.html',
  styleUrl: './galeria.css'
})
export class Galeria {
  private readonly galeriaService = inject(GaleriaService);

  protected readonly fotos = signal<FotoGaleria[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal(false);

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content: 'Galeria de fotos da IBNELVE: momentos da comunidade, cultos e eventos.'
    });

    this.galeriaService.listar().subscribe({
      next: (fotos) => {
        this.fotos.set(fotos);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set(true);
        this.carregando.set(false);
      }
    });
  }
}
