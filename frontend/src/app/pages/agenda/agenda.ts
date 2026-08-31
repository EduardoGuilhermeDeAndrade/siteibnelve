import { Component, computed, signal } from '@angular/core';

import { CategoriaEvento, EVENTOS_MOCK } from '../../shared/data/eventos.mock';
import { EventCard } from '../../shared/ui/event-card/event-card';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

interface FiltroCategoria {
  valor: CategoriaEvento | 'TODAS';
  rotulo: string;
}

@Component({
  selector: 'app-agenda',
  imports: [EventCard, SectionHeading],
  templateUrl: './agenda.html',
  styleUrl: './agenda.css'
})
export class Agenda {
  private readonly eventosPublicos = EVENTOS_MOCK.filter(
    (evento) => evento.visibilidade === 'PUBLICO' && evento.status === 'PUBLICADO'
  );

  protected readonly filtros: FiltroCategoria[] = [
    { valor: 'TODAS', rotulo: 'Todas as categorias' },
    ...Array.from(new Map(this.eventosPublicos.map((e) => [e.categoria, e.categoriaLabel])), (
      [valor, rotulo]
    ) => ({ valor: valor as CategoriaEvento, rotulo }))
  ];

  protected readonly categoriaSelecionada = signal<CategoriaEvento | 'TODAS'>('TODAS');

  protected readonly eventosFiltrados = computed(() => {
    const categoria = this.categoriaSelecionada();
    return categoria === 'TODAS'
      ? this.eventosPublicos
      : this.eventosPublicos.filter((evento) => evento.categoria === categoria);
  });

  protected selecionarCategoria(valor: string): void {
    this.categoriaSelecionada.set(valor as CategoriaEvento | 'TODAS');
  }
}
