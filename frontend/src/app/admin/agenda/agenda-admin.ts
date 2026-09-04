import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import {
  CATEGORIA_LABELS,
  ExcecaoUpsert,
  Ocorrencia,
  SerieEventoAdmin,
  STATUS_LABELS,
  TIPO_RECORRENCIA_LABELS,
  VISIBILIDADE_LABELS
} from './serie-evento-admin.models';
import { SeriesEventoService } from './series-evento.service';

@Component({
  selector: 'app-agenda-admin',
  imports: [RouterLink, DatePipe, ReactiveFormsModule],
  templateUrl: './agenda-admin.html',
  styleUrl: './agenda-admin.css'
})
export class AgendaAdmin {
  private readonly seriesEventoService = inject(SeriesEventoService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  protected readonly categoriaLabels = CATEGORIA_LABELS;
  protected readonly visibilidadeLabels = VISIBILIDADE_LABELS;
  protected readonly statusLabels = STATUS_LABELS;
  protected readonly tipoRecorrenciaLabels = TIPO_RECORRENCIA_LABELS;

  protected readonly series = signal<SerieEventoAdmin[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly excluindoId = signal<string | null>(null);
  protected readonly confirmandoId = signal<string | null>(null);

  protected readonly expandidaId = signal<string | null>(null);
  protected readonly ocorrencias = signal<Ocorrencia[]>([]);
  protected readonly carregandoOcorrencias = signal(false);
  protected readonly erroOcorrencias = signal<string | null>(null);

  protected readonly editandoExcecaoData = signal<string | null>(null);
  protected readonly salvandoExcecao = signal(false);
  protected readonly erroExcecao = signal<string | null>(null);

  protected readonly excecaoForm = this.fb.nonNullable.group({
    cancelado: [false],
    novaData: [''],
    novaHoraInicio: [''],
    novaHoraFim: [''],
    tituloSubstituto: [''],
    descricaoSubstituta: [''],
    localTextoSubstituto: [''],
    motivo: ['']
  });

  constructor() {
    this.carregar();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.seriesEventoService.listar().subscribe({
      next: (series) => {
        this.series.set(series);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar a agenda.');
        this.carregando.set(false);
      }
    });
  }

  protected pedirConfirmacao(id: string): void {
    this.confirmandoId.set(id);
  }

  protected cancelarConfirmacao(): void {
    this.confirmandoId.set(null);
  }

  protected excluir(id: string): void {
    this.excluindoId.set(id);

    this.seriesEventoService.excluir(id).subscribe({
      next: () => {
        this.series.update((lista) => lista.filter((s) => s.id !== id));
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
        if (this.expandidaId() === id) {
          this.expandidaId.set(null);
        }
      },
      error: () => {
        this.erro.set('Não foi possível excluir a série.');
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      }
    });
  }

  protected alternarExpansao(serieId: string): void {
    if (this.expandidaId() === serieId) {
      this.expandidaId.set(null);
      return;
    }

    this.expandidaId.set(serieId);
    this.editandoExcecaoData.set(null);
    this.carregarOcorrencias(serieId);
  }

  private carregarOcorrencias(serieId: string): void {
    this.carregandoOcorrencias.set(true);
    this.erroOcorrencias.set(null);

    this.seriesEventoService.obterOcorrencias(serieId).subscribe({
      next: (ocorrencias) => {
        this.ocorrencias.set(ocorrencias);
        this.carregandoOcorrencias.set(false);
      },
      error: () => {
        this.erroOcorrencias.set('Não foi possível carregar as próximas ocorrências.');
        this.carregandoOcorrencias.set(false);
      }
    });
  }

  protected abrirEdicaoExcecao(ocorrencia: Ocorrencia): void {
    this.editandoExcecaoData.set(ocorrencia.dataOriginal);
    this.erroExcecao.set(null);

    const dataEfetiva = ocorrencia.dataHoraInicio.slice(0, 10);
    const horaEfetiva = ocorrencia.dataHoraInicio.slice(11, 16);
    const horaFimEfetiva = ocorrencia.dataHoraFim ? ocorrencia.dataHoraFim.slice(11, 16) : '';

    this.excecaoForm.reset({
      cancelado: ocorrencia.cancelada,
      novaData: dataEfetiva !== ocorrencia.dataOriginal ? dataEfetiva : '',
      novaHoraInicio: horaEfetiva,
      novaHoraFim: horaFimEfetiva,
      tituloSubstituto: ocorrencia.temExcecao ? ocorrencia.titulo : '',
      descricaoSubstituta: ocorrencia.temExcecao ? (ocorrencia.descricao ?? '') : '',
      localTextoSubstituto: ocorrencia.temExcecao ? (ocorrencia.localTexto ?? '') : '',
      motivo: ocorrencia.motivo ?? ''
    });
  }

  protected cancelarEdicaoExcecao(): void {
    this.editandoExcecaoData.set(null);
    this.erroExcecao.set(null);
  }

  protected salvarExcecaoInline(serieId: string, dataOriginal: string): void {
    const valores = this.excecaoForm.getRawValue();

    const payload: ExcecaoUpsert = {
      cancelado: valores.cancelado,
      novaData: valores.novaData || null,
      novaHoraInicio: valores.novaHoraInicio ? `${valores.novaHoraInicio}:00` : null,
      novaHoraFim: valores.novaHoraFim ? `${valores.novaHoraFim}:00` : null,
      tituloSubstituto: valores.tituloSubstituto || null,
      descricaoSubstituta: valores.descricaoSubstituta || null,
      localSubstitutoId: null,
      localTextoSubstituto: valores.localTextoSubstituto || null,
      motivo: valores.motivo || null
    };

    this.salvandoExcecao.set(true);
    this.erroExcecao.set(null);

    this.seriesEventoService.salvarExcecao(serieId, dataOriginal, payload).subscribe({
      next: () => {
        this.salvandoExcecao.set(false);
        this.editandoExcecaoData.set(null);
        this.carregarOcorrencias(serieId);
      },
      error: () => {
        this.salvandoExcecao.set(false);
        this.erroExcecao.set('Não foi possível salvar a alteração desta ocorrência.');
      }
    });
  }

  protected cancelarOcorrenciaRapido(serieId: string, ocorrencia: Ocorrencia): void {
    const payload: ExcecaoUpsert = {
      cancelado: true,
      novaData: null,
      novaHoraInicio: null,
      novaHoraFim: null,
      tituloSubstituto: null,
      descricaoSubstituta: null,
      localSubstitutoId: null,
      localTextoSubstituto: null,
      motivo: null
    };

    this.seriesEventoService.salvarExcecao(serieId, ocorrencia.dataOriginal, payload).subscribe({
      next: () => this.carregarOcorrencias(serieId),
      error: () => this.erroOcorrencias.set('Não foi possível cancelar esta ocorrência.')
    });
  }

  protected reverterExcecao(serieId: string, dataOriginal: string): void {
    this.seriesEventoService.removerExcecao(serieId, dataOriginal).subscribe({
      next: () => this.carregarOcorrencias(serieId),
      error: () => this.erroOcorrencias.set('Não foi possível reverter a exceção desta ocorrência.')
    });
  }

  protected editarEstaEProximas(serieId: string, dataOriginal: string): void {
    this.router.navigate(['/admin/agenda', serieId, 'dividir', dataOriginal]);
  }
}
