import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { mensagemDeErro } from '../../../shared/errors/mensagem-erro';
import { MinisterioUpsert } from '../ministerio-admin.models';
import { MinisteriosService } from '../ministerios.service';

@Component({
  selector: 'app-ministerio-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './ministerio-form.html',
  styleUrl: './ministerio-form.css'
})
export class MinisterioForm {
  private readonly fb = inject(FormBuilder);
  private readonly ministeriosService = inject(MinisteriosService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  protected readonly ministerioId = signal<string | null>(null);
  protected readonly carregando = signal(false);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    nome: ['', Validators.required],
    lideres: ['', Validators.required],
    descricao: [''],
    imagemUrl: [''],
    ativo: [true],
    ordem: [0]
  });

  protected get modoEdicao(): boolean {
    return this.ministerioId() !== null;
  }

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }

    this.ministerioId.set(id);
    this.carregando.set(true);

    this.ministeriosService.obter(id).subscribe({
      next: (ministerio) => {
        this.form.patchValue({
          nome: ministerio.nome,
          lideres: ministerio.lideres,
          descricao: ministerio.descricao ?? '',
          imagemUrl: ministerio.imagemUrl ?? '',
          ativo: ministerio.ativo,
          ordem: ministerio.ordem
        });
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o ministério.');
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

    const payload: MinisterioUpsert = {
      nome: valores.nome,
      lideres: valores.lideres,
      descricao: valores.descricao || null,
      imagemUrl: valores.imagemUrl || null,
      ativo: valores.ativo,
      ordem: Number(valores.ordem)
    };

    const id = this.ministerioId();
    const requisicao = id
      ? this.ministeriosService.atualizar(id, payload)
      : this.ministeriosService.criar(payload);

    requisicao.subscribe({
      next: () => this.router.navigateByUrl('/admin/ministerios'),
      error: (erro: HttpErrorResponse) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar o ministério.'));
        this.salvando.set(false);
      }
    });
  }
}
