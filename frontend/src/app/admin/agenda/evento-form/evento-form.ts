import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { LocalAdmin } from '../../locais/local-admin.models';
import { LocaisService } from '../../locais/locais.service';
import {
  CATEGORIA_LABELS,
  CategoriaEvento,
  DIA_SEMANA_LABELS,
  EventoUpsert,
  STATUS_LABELS,
  StatusEvento,
  VISIBILIDADE_LABELS,
  VisibilidadeEvento
} from '../evento-admin.models';
import { EventosService } from '../eventos.service';

function paraDataHoraLocal(iso: string): { data: string; hora: string } {
  const dt = new Date(iso);
  const pad = (n: number) => n.toString().padStart(2, '0');
  return {
    data: `${dt.getFullYear()}-${pad(dt.getMonth() + 1)}-${pad(dt.getDate())}`,
    hora: `${pad(dt.getHours())}:${pad(dt.getMinutes())}`
  };
}

function paraIso(data: string, hora: string): string {
  return new Date(`${data}T${hora}`).toISOString();
}

@Component({
  selector: 'app-evento-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './evento-form.html',
  styleUrl: './evento-form.css'
})
export class EventoForm {
  private readonly fb = inject(FormBuilder);
  private readonly eventosService = inject(EventosService);
  private readonly locaisService = inject(LocaisService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly categoriaLabels = CATEGORIA_LABELS;
  protected readonly visibilidadeLabels = VISIBILIDADE_LABELS;
  protected readonly statusLabels = STATUS_LABELS;
  protected readonly diaSemanaLabels = DIA_SEMANA_LABELS;

  protected readonly categorias = Object.keys(CATEGORIA_LABELS) as CategoriaEvento[];
  protected readonly visibilidades = Object.keys(VISIBILIDADE_LABELS) as VisibilidadeEvento[];
  protected readonly statusList = Object.keys(STATUS_LABELS) as StatusEvento[];

  protected readonly locais = signal<LocalAdmin[]>([]);
  protected readonly eventoId = signal<string | null>(null);
  protected readonly carregando = signal(false);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    titulo: ['', Validators.required],
    descricao: [''],
    categoria: ['CULTO' as CategoriaEvento, Validators.required],
    visibilidade: ['PUBLICO' as VisibilidadeEvento, Validators.required],
    status: ['RASCUNHO' as StatusEvento, Validators.required],
    data: ['', Validators.required],
    hora: ['', Validators.required],
    localId: [''],
    localTexto: [''],
    recorrenciaSemanal: [false],
    diaSemana: [0]
  });

  protected get modoEdicao(): boolean {
    return this.eventoId() !== null;
  }

  constructor() {
    this.locaisService.listar().subscribe({ next: (locais) => this.locais.set(locais) });

    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }

    this.eventoId.set(id);
    this.carregando.set(true);

    this.eventosService.obter(id).subscribe({
      next: (evento) => {
        const { data, hora } = paraDataHoraLocal(evento.dataHoraInicio);
        this.form.patchValue({
          titulo: evento.titulo,
          descricao: evento.descricao ?? '',
          categoria: evento.categoria,
          visibilidade: evento.visibilidade,
          status: evento.status,
          data,
          hora,
          localId: evento.localId ?? '',
          localTexto: evento.localTexto ?? '',
          recorrenciaSemanal: evento.recorrenciaSemanal,
          diaSemana: evento.diaSemana ?? 0
        });
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o evento.');
        this.carregando.set(false);
      }
    });
  }

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const valores = this.form.getRawValue();

    if (!valores.localId && !valores.localTexto.trim()) {
      this.erro.set('Informe um local cadastrado ou um local em texto livre.');
      return;
    }
    if (valores.recorrenciaSemanal && valores.diaSemana === null) {
      this.erro.set('Selecione o dia da semana para eventos recorrentes.');
      return;
    }

    this.erro.set(null);
    this.salvando.set(true);

    const payload: EventoUpsert = {
      titulo: valores.titulo,
      descricao: valores.descricao || null,
      categoria: valores.categoria,
      visibilidade: valores.visibilidade,
      status: valores.status,
      dataHoraInicio: paraIso(valores.data, valores.hora),
      dataHoraFim: null,
      localId: valores.localId || null,
      localTexto: valores.localTexto || null,
      recorrenciaSemanal: valores.recorrenciaSemanal,
      diaSemana: valores.recorrenciaSemanal ? valores.diaSemana : null
    };

    const id = this.eventoId();
    const requisicao = id
      ? this.eventosService.atualizar(id, payload)
      : this.eventosService.criar(payload);

    requisicao.subscribe({
      next: () => this.router.navigateByUrl('/admin/agenda'),
      error: (erro: { error?: { message?: string } }) => {
        this.erro.set(erro?.error?.message ?? 'Não foi possível salvar o evento.');
        this.salvando.set(false);
      }
    });
  }
}
