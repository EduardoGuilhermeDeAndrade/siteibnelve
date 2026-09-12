import { Component, Input, OnDestroy, OnInit, signal } from '@angular/core';

export interface FotoCarrossel {
  url: string;
  alt: string;
}

/**
 * Carrossel automático do Hero (3 fotos). Respeita prefers-reduced-motion (não
 * avança sozinho nesse caso — controles manuais continuam disponíveis), pausa
 * ao passar o mouse/focar e enquanto a aba está em segundo plano.
 */
@Component({
  selector: 'app-hero-carousel',
  imports: [],
  templateUrl: './hero-carousel.html',
  styleUrl: './hero-carousel.css'
})
export class HeroCarousel implements OnInit, OnDestroy {
  @Input({ required: true }) slides: FotoCarrossel[] = [];

  protected readonly indiceAtual = signal(0);

  private intervalId?: ReturnType<typeof setInterval>;
  private pausado = false;
  private readonly duracaoMs = 6000;

  ngOnInit(): void {
    if (this.slides.length > 1 && !this.prefereMovimentoReduzido()) {
      this.intervalId = setInterval(() => {
        if (!this.pausado && document.visibilityState === 'visible') {
          this.proximo();
        }
      }, this.duracaoMs);
    }
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  protected irPara(indice: number): void {
    this.indiceAtual.set(indice);
  }

  protected anterior(): void {
    const total = this.slides.length;
    this.indiceAtual.set((this.indiceAtual() - 1 + total) % total);
  }

  protected proximo(): void {
    this.indiceAtual.set((this.indiceAtual() + 1) % this.slides.length);
  }

  protected pausar(): void {
    this.pausado = true;
  }

  protected retomar(): void {
    this.pausado = false;
  }

  private prefereMovimentoReduzido(): boolean {
    return window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  }
}
