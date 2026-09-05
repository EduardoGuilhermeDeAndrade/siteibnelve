import { Component, computed, inject, signal } from '@angular/core';
import { Meta } from '@angular/platform-browser';

import { CategoriaEvento } from '../../shared/data/evento.types';
import { EventoAgenda, EventosService } from '../../shared/data/eventos.service';
import { EventCard } from '../../shared/ui/event-card/event-card';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

interface FiltroCategoria {
  valor: CategoriaEvento | 'TODAS';
  rotulo: string;
}

const FILTRO_TODAS: FiltroCategoria = { valor: 'TODAS', rotulo: 'Todas as categorias' };

@Component({
  selector: 'app-agenda',
  imports: [EventCard, SectionHeading],
  templateUrl: './agenda.html',
  styleUrl: './agenda.css'
})
export class Agenda {
  private readonly eventosService = inject(EventosService);

  protected readonly eventosPublicos = signal<EventoAgenda[]>([]);
  protected readonly filtros = signal<FiltroCategoria[]>([FILTRO_TODAS]);
  protected readonly categoriaSelecionada = signal<CategoriaEvento | 'TODAS'>('TODAS');
  protected readonly carregando = signal(true);
  protected readonly erro = signal(false);

  protected readonly eventosFiltrados = computed(() => {
    const categoria = this.categoriaSelecionada();
    const eventos = this.eventosPublicos();
    return categoria === 'TODAS' ? eventos : eventos.filter((evento) => evento.categoria === categoria);
  });

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content: 'Agenda de cultos, Escola Bíblica, Ceia e eventos da IBNELVE em Ribeirão das Neves/MG.'
    });

    this.eventosService.listar().subscribe({
      next: (eventos) => {
        this.eventosPublicos.set(eventos);

        const categoriasUnicas = new Map(eventos.map((evento) => [evento.categoria, evento.categoriaLabel]));
        this.filtros.set([
          FILTRO_TODAS,
          ...Array.from(categoriasUnicas, ([valor, rotulo]) => ({ valor, rotulo }))
        ]);

        this.carregando.set(false);
      },
      error: () => {
        this.erro.set(true);
        this.carregando.set(false);
      }
    });
  }

  protected selecionarCategoria(valor: string): void {
    this.categoriaSelecionada.set(valor as CategoriaEvento | 'TODAS');
  }
}
