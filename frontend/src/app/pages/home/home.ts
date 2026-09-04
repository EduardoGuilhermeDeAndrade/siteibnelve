import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { EventoAgenda, EventosService } from '../../shared/data/eventos.service';
import { ImagensSiteService } from '../../shared/data/imagens-site.service';
import { LocalIgreja, LocaisService } from '../../shared/data/locais.service';
import { Ministerio, MinisteriosService } from '../../shared/data/ministerios.service';
import { EventCard } from '../../shared/ui/event-card/event-card';
import { MinisterioCard } from '../../shared/ui/ministerio-card/ministerio-card';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

const HERO_PADRAO = '/img/comunidade-hero.png';

@Component({
  selector: 'app-home',
  imports: [RouterLink, EventCard, MinisterioCard, SectionHeading],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  private readonly eventosService = inject(EventosService);
  private readonly locaisService = inject(LocaisService);
  private readonly ministeriosService = inject(MinisteriosService);
  private readonly imagensSiteService = inject(ImagensSiteService);

  protected readonly heroUrl = signal(HERO_PADRAO);
  protected readonly proximosEventos = signal<EventoAgenda[]>([]);
  protected readonly locais = signal<LocalIgreja[]>([]);
  protected readonly ministerios = signal<Ministerio[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal(false);

  constructor() {
    this.imagensSiteService.buscarUrl('home-hero').subscribe({
      next: (url) => {
        if (url) {
          this.heroUrl.set(url);
        }
      },
      error: () => {
        // Silencioso: mantém a foto padrão se a API não responder.
      }
    });

    forkJoin({
      eventos: this.eventosService.listar(3),
      locais: this.locaisService.listar(),
      ministerios: this.ministeriosService.listar()
    }).subscribe({
      next: ({ eventos, locais, ministerios }) => {
        this.proximosEventos.set(eventos);
        this.locais.set(locais);
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
