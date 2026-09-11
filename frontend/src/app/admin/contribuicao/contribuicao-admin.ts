import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { mensagemDeErro } from '../../shared/errors/mensagem-erro';
import { TIPO_CHAVE_PIX_LABELS, TipoChavePix } from './contribuicao-admin.models';
import { ContribuicaoAdminService } from './contribuicao.service';

@Component({
  selector: 'app-contribuicao-admin',
  imports: [ReactiveFormsModule],
  templateUrl: './contribuicao-admin.html',
  styleUrl: './contribuicao-admin.css'
})
export class ContribuicaoAdmin {
  private readonly fb = inject(FormBuilder);
  private readonly contribuicaoService = inject(ContribuicaoAdminService);
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  protected readonly tipoChaveLabels = TIPO_CHAVE_PIX_LABELS;
  protected readonly tiposChave = Object.keys(TIPO_CHAVE_PIX_LABELS) as TipoChavePix[];

  protected readonly carregando = signal(true);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);
  protected readonly sucesso = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    chavePix: ['', Validators.required],
    tipoChave: ['CNPJ' as TipoChavePix, Validators.required],
    favorecido: ['', Validators.required]
  });

  constructor() {
    this.contribuicaoService.obter().subscribe({
      next: (configuracao) => {
        this.form.patchValue({
          chavePix: configuracao.chavePix,
          tipoChave: configuracao.tipoChave,
          favorecido: configuracao.favorecido
        });
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar a configuração de contribuições.');
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

    this.erro.set(null);
    this.sucesso.set(false);
    this.salvando.set(true);

    this.contribuicaoService.atualizar(this.form.getRawValue()).subscribe({
      next: () => {
        this.salvando.set(false);
        this.sucesso.set(true);
        setTimeout(() => this.sucesso.set(false), 4000);
      },
      error: (erro: HttpErrorResponse) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar a configuração.'));
        this.salvando.set(false);
      }
    });
  }
}
