import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { UsuarioCriar } from '../usuario-admin.models';
import { UsuariosService } from '../usuarios.service';

@Component({
  selector: 'app-usuario-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './usuario-form.html',
  styleUrl: './usuario-form.css'
})
export class UsuarioForm {
  private readonly fb = inject(FormBuilder);
  private readonly usuariosService = inject(UsuariosService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  protected readonly usuarioId = signal<string | null>(null);
  protected readonly carregando = signal(false);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    nomeCompleto: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    senha: ['', [Validators.required, Validators.minLength(8)]]
  });

  protected get modoEdicao(): boolean {
    return this.usuarioId() !== null;
  }

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }

    this.usuarioId.set(id);
    this.carregando.set(true);

    // Edição só permite trocar o nome — email e senha não fazem parte do payload de atualização,
    // então os controles ficam desabilitados aqui (redefinir senha é uma ação separada na lista).
    this.form.controls.email.disable();
    this.form.controls.senha.disable();

    this.usuariosService.listar().subscribe({
      next: (usuarios) => {
        const usuario = usuarios.find((u) => u.id === id);
        if (usuario) {
          this.form.patchValue({ nomeCompleto: usuario.nomeCompleto, email: usuario.email });
        } else {
          this.erro.set('Usuário não encontrado.');
        }
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o usuário.');
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
    this.salvando.set(true);

    const id = this.usuarioId();
    const valores = this.form.getRawValue();

    const requisicao = id
      ? this.usuariosService.atualizar(id, { nomeCompleto: valores.nomeCompleto })
      : this.usuariosService.criar(valores as UsuarioCriar);

    requisicao.subscribe({
      next: () => this.router.navigateByUrl('/admin/usuarios'),
      error: (erro: { error?: { message?: string } }) => {
        this.erro.set(erro?.error?.message ?? 'Não foi possível salvar o usuário.');
        this.salvando.set(false);
      }
    });
  }
}
