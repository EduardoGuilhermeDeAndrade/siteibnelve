import { Component, inject, signal } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { forkJoin } from 'rxjs';

import { ConfiguracaoContribuicao, ContribuicaoService } from '../../shared/data/contribuicao.service';
import { TIPO_CHAVE_PIX_LABELS, TipoChavePix } from '../../shared/data/contribuicao.types';
import { ImagensSiteService } from '../../shared/data/imagens-site.service';
import { SectionHeading } from '../../shared/ui/section-heading/section-heading';

const CHAVE_QR_CODE = 'contribuicao-qrcode';

@Component({
  selector: 'app-contribuicoes',
  imports: [SectionHeading],
  templateUrl: './contribuicoes.html',
  styleUrl: './contribuicoes.css'
})
export class Contribuicoes {
  private readonly contribuicaoService = inject(ContribuicaoService);
  private readonly imagensSiteService = inject(ImagensSiteService);

  protected readonly tipoChaveLabels = TIPO_CHAVE_PIX_LABELS;

  protected readonly configuracao = signal<ConfiguracaoContribuicao | null>(null);
  protected readonly urlQrCode = signal<string | null>(null);
  protected readonly copiado = signal(false);
  protected readonly carregando = signal(true);
  protected readonly erro = signal(false);

  constructor() {
    inject(Meta).updateTag({
      name: 'description',
      content: 'Contribua com a obra da IBNELVE via PIX e conheça outras formas de apoiar a igreja.'
    });

    forkJoin({
      configuracao: this.contribuicaoService.obter(),
      urlQrCode: this.imagensSiteService.buscarUrl(CHAVE_QR_CODE)
    }).subscribe({
      next: ({ configuracao, urlQrCode }) => {
        this.configuracao.set(configuracao);
        this.urlQrCode.set(urlQrCode);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set(true);
        this.carregando.set(false);
      }
    });
  }

  protected get chavePix(): string {
    return this.configuracao()?.chavePix ?? '';
  }

  protected get tipoChaveLabel(): string {
    const tipo = this.configuracao()?.tipoChave as TipoChavePix | undefined;
    return tipo ? this.tipoChaveLabels[tipo] : '';
  }

  protected get favorecido(): string {
    return this.configuracao()?.favorecido ?? '';
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
