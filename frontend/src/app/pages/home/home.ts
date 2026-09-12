import { Component, inject, signal } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { EventoAgenda, EventosService } from '../../shared/data/eventos.service';
import { ImagensSiteService } from '../../shared/data/imagens-site.service';
import { LocalIgreja, LocaisService } from '../../shared/data/locais.service';
import { Ministerio, MinisteriosService } from '../../shared/data/ministerios.service';
import { EventCard } from '../../shared/ui/event-card/event-card';
import { FotoCarrossel, HeroCarousel } from '../../shared/ui/hero-carousel/hero-carousel';
import { MinisterioCard } from '../../shared/ui/ministerio-card/ministerio-card';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

/** As 3 fotos do carrossel são editáveis no Portal (Conteúdo do Site) pelas chaves abaixo;
 *  os arquivos aqui são só o fallback caso alguma chave ainda não tenha sido cadastrada. */
const SLIDES_PADRAO: FotoCarrossel[] = [
  {
    url: '/img/comunidade-hero.png',
    alt: 'Membros da IBNELVE reunidos, sorrindo e acenando, celebrando juntos em um culto'
  },
  {
    url: '/img/home-carrossel-2.jpg',
    alt: 'Mural de fotografias da comunidade da IBNELVE penduradas em varal, com o letreiro da igreja ao fundo'
  },
  {
    url: '/img/home-carrossel-3.jpg',
    alt: 'Equipe de louvor da IBNELVE cantando durante um culto'
  }
];

@Component({
  selector: 'app-home',
  imports: [RouterLink, EventCard, MinisterioCard, SectionHeading, HeroCarousel],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  private readonly eventosService = inject(EventosService);
  private readonly locaisService = inject(LocaisService);
  private readonly ministeriosService = inject(MinisteriosService);
  private readonly imagensSiteService = inject(ImagensSiteService);

  protected readonly heroSlides = signal<FotoCarrossel[]>(SLIDES_PADRAO);
  protected readonly proximosEventos = signal<EventoAgenda[]>([]);
  protected readonly locais = signal<LocalIgreja[]>([]);
  protected readonly ministerios = signal<Ministerio[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal(false);

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content:
        'Igreja Batista Nacional da Esperança do Liberdade e Vereda — comunidade cristã em Ribeirão das Neves/MG. Cultos, ministérios e agenda.'
    });

    this.imagensSiteService.listar().subscribe({
      next: (imagens) => {
        const url = (chave: string) => imagens.find((i) => i.chave === chave)?.url;
        this.heroSlides.set([
          { ...SLIDES_PADRAO[0], url: url('home-hero') ?? SLIDES_PADRAO[0].url },
          { ...SLIDES_PADRAO[1], url: url('home-carrossel-2') ?? SLIDES_PADRAO[1].url },
          { ...SLIDES_PADRAO[2], url: url('home-carrossel-3') ?? SLIDES_PADRAO[2].url }
        ]);
      },
      error: () => {
        // Silencioso: mantém as fotos padrão se a API não responder.
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
