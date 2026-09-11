import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { mensagemDeErro } from '../../../shared/errors/mensagem-erro';
import { LocalAdmin } from '../../locais/local-admin.models';
import { LocaisService } from '../../locais/locais.service';
import {
  CATEGORIA_LABELS,
  CategoriaEvento,
  DIA_SEMANA_LABELS,
  POSICAO_NO_MES_LABELS,
  SerieEventoUpsert,
  STATUS_LABELS,
  StatusEvento,
  TIPO_RECORRENCIA_LABELS,
  TipoRecorrencia,
  VISIBILIDADE_LABELS,
  VisibilidadeEvento
} from '../serie-evento-admin.models';
import { SeriesEventoService } from '../series-evento.service';

const TIPOS_COM_DIA_SEMANA: TipoRecorrencia[] = ['SEMANAL', 'A_CADA_N_SEMANAS', 'MENSAL_POR_POSICAO'];

@Component({
  selector: 'app-evento-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './evento-form.html',
  styleUrl: './evento-form.css'
})
export class EventoForm {
  private readonly fb = inject(FormBuilder);
  private readonly seriesEventoService = inject(SeriesEventoService);
  private readonly locaisService = inject(LocaisService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  protected readonly categoriaLabels = CATEGORIA_LABELS;
  protected readonly visibilidadeLabels = VISIBILIDADE_LABELS;
  protected readonly statusLabels = STATUS_LABELS;
  protected readonly tipoRecorrenciaLabels = TIPO_RECORRENCIA_LABELS;
  protected readonly diaSemanaLabels = DIA_SEMANA_LABELS;
  protected readonly posicaoNoMesLabels = POSICAO_NO_MES_LABELS;

  protected readonly categorias = Object.keys(CATEGORIA_LABELS) as CategoriaEvento[];
  protected readonly visibilidades = Object.keys(VISIBILIDADE_LABELS) as VisibilidadeEvento[];
  protected readonly statusList = Object.keys(STATUS_LABELS) as StatusEvento[];
  protected readonly tiposRecorrencia = Object.keys(TIPO_RECORRENCIA_LABELS) as TipoRecorrencia[];
  protected readonly posicoesNoMes = [1, 2, 3, 4, -1];

  protected readonly locais = signal<LocalAdmin[]>([]);
  protected readonly serieId = signal<string | null>(null);
  protected readonly apartirDe = signal<string | null>(null);
  protected readonly carregando = signal(false);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    titulo: ['', Validators.required],
    descricao: [''],
    categoria: ['CULTO' as CategoriaEvento, Validators.required],
    visibilidade: ['PUBLICO' as VisibilidadeEvento, Validators.required],
    status: ['RASCUNHO' as StatusEvento, Validators.required],
    tipoRecorrencia: ['NENHUMA' as TipoRecorrencia, Validators.required],
    intervalo: [2],
    diaSemana: [0],
    diaMes: [1],
    posicaoNoMes: [1],
    dataInicio: ['', Validators.required],
    dataFim: [''],
    horaInicio: ['', Validators.required],
    horaFim: [''],
    localId: [''],
    localTexto: [''],
    imagemUrl: [''],
    destaque: [false],
    ativo: [true]
  });

  protected get modoEdicao(): boolean {
    return this.serieId() !== null && this.apartirDe() === null;
  }

  protected get modoDivisao(): boolean {
    return this.apartirDe() !== null;
  }

  protected get precisaDiaSemana(): boolean {
    return TIPOS_COM_DIA_SEMANA.includes(this.form.controls.tipoRecorrencia.value);
  }

  protected get precisaIntervalo(): boolean {
    return this.form.controls.tipoRecorrencia.value === 'A_CADA_N_SEMANAS';
  }

  protected get precisaDiaMes(): boolean {
    return this.form.controls.tipoRecorrencia.value === 'MENSAL_POR_DIA';
  }

  protected get precisaPosicaoNoMes(): boolean {
    return this.form.controls.tipoRecorrencia.value === 'MENSAL_POR_POSICAO';
  }

  protected get ehAvulso(): boolean {
    return this.form.controls.tipoRecorrencia.value === 'NENHUMA';
  }

  constructor() {
    this.locaisService.listar().subscribe({ next: (locais) => this.locais.set(locais) });

    const id = this.route.snapshot.paramMap.get('id');
    const data = this.route.snapshot.paramMap.get('data');
    if (!id) {
      return;
    }

    this.serieId.set(id);
    if (data) {
      this.apartirDe.set(data);
    }

    this.carregando.set(true);

    this.seriesEventoService.obter(id).subscribe({
      next: (serie) => {
        this.form.patchValue({
          titulo: serie.titulo,
          descricao: serie.descricao ?? '',
          categoria: serie.categoria,
          visibilidade: serie.visibilidade,
          status: serie.status,
          tipoRecorrencia: serie.tipoRecorrencia,
          intervalo: serie.intervalo ?? 2,
          diaSemana: serie.diaSemana ?? 0,
          diaMes: serie.diaMes ?? 1,
          posicaoNoMes: serie.posicaoNoMes ?? 1,
          dataInicio: this.apartirDe() ?? serie.dataInicioRecorrencia,
          dataFim: serie.dataFimRecorrencia ?? '',
          horaInicio: serie.horaInicio.slice(0, 5),
          horaFim: serie.horaFim ? serie.horaFim.slice(0, 5) : '',
          localId: serie.localId ?? '',
          localTexto: serie.localTexto ?? '',
          imagemUrl: serie.imagemUrl ?? '',
          destaque: serie.destaque,
          ativo: serie.ativo
        });
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar a série.');
        this.carregando.set(false);
      }
    });
  }

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.elementRef.nativeElement.querySelector<HTMLElement>('.ng-invalid')?.focus();
      return;
    }

    const payload = this.paraPayload();

    if (!payload.localId && !payload.localTexto?.trim()) {
      this.erro.set('Informe um local cadastrado ou um local em texto livre.');
      return;
    }
    if (this.precisaDiaSemana && payload.diaSemana === null) {
      this.erro.set('Selecione o dia da semana para este tipo de recorrência.');
      return;
    }
    if (this.precisaIntervalo && (payload.intervalo === null || payload.intervalo < 1)) {
      this.erro.set('Informe a cada quantas semanas o evento se repete.');
      return;
    }
    if (this.precisaDiaMes && payload.diaMes === null) {
      this.erro.set('Informe o dia do mês.');
      return;
    }
    if (this.precisaPosicaoNoMes && payload.posicaoNoMes === null) {
      this.erro.set('Selecione a posição no mês (ex.: primeiro, último).');
      return;
    }

    this.erro.set(null);
    this.salvando.set(true);

    const id = this.serieId();
    const apartirDe = this.apartirDe();

    const requisicao = apartirDe && id
      ? this.seriesEventoService.dividirSerie(id, apartirDe, payload)
      : id
        ? this.seriesEventoService.atualizar(id, payload)
        : this.seriesEventoService.criar(payload);

    requisicao.subscribe({
      next: () => this.router.navigateByUrl('/admin/agenda'),
      error: (erro: HttpErrorResponse) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar a série.'));
        this.salvando.set(false);
      }
    });
  }

  private paraPayload(): SerieEventoUpsert {
    const v = this.form.getRawValue();
    const tipo = v.tipoRecorrencia;

    return {
      titulo: v.titulo,
      descricao: v.descricao || null,
      categoria: v.categoria,
      visibilidade: v.visibilidade,
      status: v.status,
      tipoRecorrencia: tipo,
      intervalo: tipo === 'A_CADA_N_SEMANAS' ? v.intervalo : null,
      diaSemana: TIPOS_COM_DIA_SEMANA.includes(tipo) ? v.diaSemana : null,
      diaMes: tipo === 'MENSAL_POR_DIA' ? v.diaMes : null,
      posicaoNoMes: tipo === 'MENSAL_POR_POSICAO' ? v.posicaoNoMes : null,
      horaInicio: `${v.horaInicio}:00`,
      horaFim: v.horaFim ? `${v.horaFim}:00` : null,
      dataInicioRecorrencia: v.dataInicio,
      dataFimRecorrencia: tipo === 'NENHUMA' ? null : v.dataFim || null,
      localId: v.localId || null,
      localTexto: v.localTexto || null,
      imagemUrl: v.imagemUrl || null,
      destaque: v.destaque,
      ativo: v.ativo
    };
  }
}
