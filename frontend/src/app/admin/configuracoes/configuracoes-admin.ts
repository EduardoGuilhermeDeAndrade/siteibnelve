import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';

import { mensagemDeErro } from '../../shared/errors/mensagem-erro';
import { ContaService } from './conta.service';

function senhasIguaisValidator(grupo: AbstractControl): ValidationErrors | null {
  const novaSenha = grupo.get('novaSenha')?.value;
  const confirmarSenha = grupo.get('confirmarSenha')?.value;
  return novaSenha === confirmarSenha ? null : { senhasDiferentes: true };
}

@Component({
  selector: 'app-configuracoes-admin',
  imports: [ReactiveFormsModule],
  templateUrl: './configuracoes-admin.html',
  styleUrl: './configuracoes-admin.css'
})
export class ConfiguracoesAdmin {
  private readonly fb = inject(FormBuilder);
  private readonly contaService = inject(ContaService);
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);
  protected readonly sucesso = signal(false);

  protected readonly form = this.fb.nonNullable.group(
    {
      senhaAtual: ['', Validators.required],
      novaSenha: ['', [Validators.required, Validators.minLength(8)]],
      confirmarSenha: ['', Validators.required]
    },
    { validators: senhasIguaisValidator }
  );

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.elementRef.nativeElement.querySelector<HTMLElement>('.ng-invalid')?.focus();
      return;
    }

    this.erro.set(null);
    this.sucesso.set(false);
    this.salvando.set(true);

    const { senhaAtual, novaSenha } = this.form.getRawValue();

    this.contaService.trocarSenha(senhaAtual, novaSenha).subscribe({
      next: () => {
        this.salvando.set(false);
        this.sucesso.set(true);
        this.form.reset();
      },
      error: (erro: HttpErrorResponse) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível trocar a senha.'));
        this.salvando.set(false);
      }
    });
  }
}
