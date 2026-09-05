import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { UsuarioAdmin } from './usuario-admin.models';
import { UsuariosService } from './usuarios.service';

@Component({
  selector: 'app-usuarios-admin',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './usuarios-admin.html',
  styleUrl: './usuarios-admin.css'
})
export class UsuariosAdmin {
  private readonly fb = inject(FormBuilder);
  private readonly usuariosService = inject(UsuariosService);

  protected readonly usuarios = signal<UsuarioAdmin[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly processandoId = signal<string | null>(null);
  protected readonly confirmandoId = signal<string | null>(null);

  protected readonly redefinindoId = signal<string | null>(null);
  protected readonly salvandoSenha = signal(false);
  protected readonly erroSenha = signal<string | null>(null);
  protected readonly sucessoSenhaId = signal<string | null>(null);

  protected readonly senhaForm = this.fb.nonNullable.group({
    novaSenha: ['', [Validators.required, Validators.minLength(8)]]
  });

  constructor() {
    this.carregar();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.usuariosService.listar().subscribe({
      next: (usuarios) => {
        this.usuarios.set(usuarios);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os usuários.');
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

  protected alternarAtivo(usuario: UsuarioAdmin): void {
    this.processandoId.set(usuario.id);
    this.erro.set(null);

    const requisicao = usuario.ativo
      ? this.usuariosService.desativar(usuario.id)
      : this.usuariosService.ativar(usuario.id);

    requisicao.subscribe({
      next: () => {
        this.usuarios.update((lista) =>
          lista.map((u) => (u.id === usuario.id ? { ...u, ativo: !usuario.ativo } : u))
        );
        this.processandoId.set(null);
        this.confirmandoId.set(null);
      },
      error: (erro: { error?: { message?: string } }) => {
        this.erro.set(erro?.error?.message ?? 'Não foi possível alterar o status do usuário.');
        this.processandoId.set(null);
        this.confirmandoId.set(null);
      }
    });
  }

  protected abrirRedefinirSenha(id: string): void {
    this.redefinindoId.set(id);
    this.erroSenha.set(null);
    this.senhaForm.reset({ novaSenha: '' });
  }

  protected cancelarRedefinirSenha(): void {
    this.redefinindoId.set(null);
  }

  protected redefinirSenha(id: string): void {
    if (this.senhaForm.invalid) {
      this.senhaForm.markAllAsTouched();
      return;
    }

    this.salvandoSenha.set(true);
    this.erroSenha.set(null);

    this.usuariosService.redefinirSenha(id, this.senhaForm.getRawValue().novaSenha).subscribe({
      next: () => {
        this.salvandoSenha.set(false);
        this.redefinindoId.set(null);
        this.sucessoSenhaId.set(id);
        setTimeout(() => this.sucessoSenhaId.set(null), 4000);
      },
      error: (erro: { error?: { message?: string } }) => {
        this.erroSenha.set(erro?.error?.message ?? 'Não foi possível redefinir a senha.');
        this.salvandoSenha.set(false);
      }
    });
  }
}
