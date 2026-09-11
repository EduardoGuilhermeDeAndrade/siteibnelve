import { Component, ElementRef, ViewChild, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-admin-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  @ViewChild('emailInput') private emailInput?: ElementRef<HTMLInputElement>;

  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  protected readonly enviando = signal(false);
  protected readonly erro = signal<string | null>(
    this.route.snapshot.queryParamMap.has('sessaoExpirada')
      ? 'Sua sessão expirou. Entre novamente.'
      : null
  );

  protected async enviar(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.emailInput?.nativeElement.focus();
      return;
    }

    this.erro.set(null);
    this.enviando.set(true);

    const { email, password } = this.form.getRawValue();

    try {
      await this.authService.login(email, password);
      await this.router.navigateByUrl('/admin');
    } catch {
      this.erro.set('Email ou senha inválidos.');
      this.emailInput?.nativeElement.focus();
    } finally {
      this.enviando.set(false);
    }
  }
}
