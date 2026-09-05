import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { LocalUpsert } from '../local-admin.models';
import { LocaisService } from '../locais.service';

@Component({
  selector: 'app-local-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './local-form.html',
  styleUrl: './local-form.css'
})
export class LocalForm {
  private readonly fb = inject(FormBuilder);
  private readonly locaisService = inject(LocaisService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  protected readonly localId = signal<string | null>(null);
  protected readonly carregando = signal(false);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    nome: ['', Validators.required],
    tipo: ['', Validators.required],
    endereco: ['', Validators.required],
    bairro: ['', Validators.required],
    cidade: ['', Validators.required],
    estado: [''],
    cep: [''],
    googleMapsUrl: [''],
    latitude: [''],
    longitude: [''],
    ativo: [true],
    ordem: [0]
  });

  protected get modoEdicao(): boolean {
    return this.localId() !== null;
  }

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }

    this.localId.set(id);
    this.carregando.set(true);

    this.locaisService.obter(id).subscribe({
      next: (local) => {
        this.form.patchValue({
          nome: local.nome,
          tipo: local.tipo,
          endereco: local.endereco,
          bairro: local.bairro,
          cidade: local.cidade,
          estado: local.estado ?? '',
          cep: local.cep ?? '',
          googleMapsUrl: local.googleMapsUrl ?? '',
          latitude: local.latitude?.toString() ?? '',
          longitude: local.longitude?.toString() ?? '',
          ativo: local.ativo,
          ordem: local.ordem
        });
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o local.');
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

    const valores = this.form.getRawValue();

    this.erro.set(null);
    this.salvando.set(true);

    const payload: LocalUpsert = {
      nome: valores.nome,
      tipo: valores.tipo,
      endereco: valores.endereco,
      bairro: valores.bairro,
      cidade: valores.cidade,
      estado: valores.estado || null,
      cep: valores.cep || null,
      googleMapsUrl: valores.googleMapsUrl || null,
      latitude: valores.latitude ? Number(valores.latitude) : null,
      longitude: valores.longitude ? Number(valores.longitude) : null,
      ativo: valores.ativo,
      ordem: Number(valores.ordem)
    };

    const id = this.localId();
    const requisicao = id
      ? this.locaisService.atualizar(id, payload)
      : this.locaisService.criar(payload);

    requisicao.subscribe({
      next: () => this.router.navigateByUrl('/admin/locais'),
      error: (erro: { error?: { message?: string } }) => {
        this.erro.set(erro?.error?.message ?? 'Não foi possível salvar o local.');
        this.salvando.set(false);
      }
    });
  }
}
