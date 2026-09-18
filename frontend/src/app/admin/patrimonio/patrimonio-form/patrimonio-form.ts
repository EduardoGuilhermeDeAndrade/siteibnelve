import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { mensagemDeErro } from '../../../shared/errors/mensagem-erro';
import { LocaisService, LocalIgreja } from '../../../shared/data/locais.service';
import { validarArquivoImagem } from '../../../shared/uploads/validacao-imagem';
import { TipoControlePatrimonio } from '../patrimonio-admin.models';
import { PatrimonioService } from '../patrimonio.service';

@Component({
  selector: 'app-patrimonio-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './patrimonio-form.html',
  styleUrl: './patrimonio-form.css'
})
export class PatrimonioForm {
  private readonly fb = inject(FormBuilder);
  private readonly patrimonioService = inject(PatrimonioService);
  private readonly locaisService = inject(LocaisService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly elementRef: ElementRef<HTMLElement> = inject(ElementRef);

  protected readonly itemId = signal<string | null>(null);
  protected readonly carregando = signal(false);
  protected readonly salvando = signal(false);
  protected readonly erro = signal<string | null>(null);
  protected readonly locais = signal<LocalIgreja[]>([]);

  protected readonly enviandoFoto = signal(false);
  protected readonly erroFoto = signal<string | null>(null);
  protected readonly sucessoFoto = signal(false);
  protected fotoUrlAtual: string | null = null;

  protected arquivoSelecionado: File | null = null;
  protected readonly erroArquivo = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    descricao: ['', Validators.required],
    numeroPatrimonio: [''],
    tipoControle: ['Unitario' as TipoControlePatrimonio, Validators.required],
    quantidadeTotal: [1, [Validators.required, Validators.min(1)]],
    localId: [''],
    observacao: ['']
  });

  protected get modoEdicao(): boolean {
    return this.itemId() !== null;
  }

  constructor() {
    this.locaisService.listar().subscribe({ next: (locais) => this.locais.set(locais) });

    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }

    this.itemId.set(id);
    this.carregando.set(true);
    this.form.controls.tipoControle.disable();

    this.patrimonioService.obter(id).subscribe({
      next: (item) => {
        this.form.patchValue({
          descricao: item.descricao,
          numeroPatrimonio: item.numeroPatrimonio ?? '',
          tipoControle: item.tipoControle,
          quantidadeTotal: item.quantidadeTotal,
          localId: item.localId ?? '',
          observacao: item.observacao ?? ''
        });
        this.fotoUrlAtual = item.fotoUrl;
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o item.');
        this.carregando.set(false);
      }
    });
  }

  protected onSelecionarArquivo(evento: Event): void {
    const input = evento.target as HTMLInputElement;
    const arquivo = input.files?.[0] ?? null;

    if (!arquivo) {
      this.arquivoSelecionado = null;
      this.erroArquivo.set(null);
      return;
    }

    const problema = validarArquivoImagem(arquivo);
    if (problema) {
      this.erroArquivo.set(problema);
      input.value = '';
      this.arquivoSelecionado = null;
      return;
    }

    this.erroArquivo.set(null);
    this.arquivoSelecionado = arquivo;

    if (this.modoEdicao) {
      this.substituirFotoImediatamente(arquivo);
      input.value = '';
      this.arquivoSelecionado = null;
    }
  }

  private substituirFotoImediatamente(arquivo: File): void {
    const id = this.itemId();
    if (!id) {
      return;
    }

    this.enviandoFoto.set(true);
    this.erroFoto.set(null);

    this.patrimonioService.substituirFoto(id, arquivo).subscribe({
      next: (item) => {
        this.fotoUrlAtual = item.fotoUrl;
        this.enviandoFoto.set(false);
        this.sucessoFoto.set(true);
        setTimeout(() => this.sucessoFoto.set(false), 4000);
      },
      error: (erro: HttpErrorResponse) => {
        this.erroFoto.set(mensagemDeErro(erro, 'Não foi possível enviar a foto.'));
        this.enviandoFoto.set(false);
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

    const valores = this.form.getRawValue();
    const id = this.itemId();

    if (id) {
      this.patrimonioService
        .atualizar(id, {
          descricao: valores.descricao,
          numeroPatrimonio: valores.numeroPatrimonio || null,
          quantidadeTotal: Number(valores.quantidadeTotal),
          localId: valores.localId || null,
          observacao: valores.observacao || null
        })
        .subscribe({
          next: () => this.router.navigateByUrl(`/admin/patrimonio/${id}`),
          error: (erro: HttpErrorResponse) => {
            this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar o item.'));
            this.salvando.set(false);
          }
        });
      return;
    }

    this.patrimonioService
      .criar(
        {
          descricao: valores.descricao,
          numeroPatrimonio: valores.numeroPatrimonio || null,
          tipoControle: valores.tipoControle,
          quantidadeTotal: Number(valores.quantidadeTotal),
          localId: valores.localId || null,
          observacao: valores.observacao || null
        },
        this.arquivoSelecionado
      )
      .subscribe({
        next: (item) => this.router.navigateByUrl(`/admin/patrimonio/${item.id}`),
        error: (erro: HttpErrorResponse) => {
          this.erro.set(mensagemDeErro(erro, 'Não foi possível salvar o item.'));
          this.salvando.set(false);
        }
      });
  }
}
