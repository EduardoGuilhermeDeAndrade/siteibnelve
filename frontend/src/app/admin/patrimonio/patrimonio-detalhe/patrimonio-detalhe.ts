import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { AuthService, PAPEL_PATRIMONIO_EDITOR } from '../../../core/auth/auth.service';
import { mensagemDeErro } from '../../../shared/errors/mensagem-erro';
import { validarArquivoImagem } from '../../../shared/uploads/validacao-imagem';
import { EmprestimoPatrimonio, ItemPatrimonio } from '../patrimonio-admin.models';
import { PatrimonioService } from '../patrimonio.service';

@Component({
  selector: 'app-patrimonio-detalhe',
  imports: [FormsModule, RouterLink],
  templateUrl: './patrimonio-detalhe.html',
  styleUrl: './patrimonio-detalhe.css'
})
export class PatrimonioDetalhe {
  private readonly patrimonioService = inject(PatrimonioService);
  protected readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);

  private readonly itemId = this.route.snapshot.paramMap.get('id')!;

  protected readonly item = signal<ItemPatrimonio | null>(null);
  protected readonly emprestimos = signal<EmprestimoPatrimonio[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly podeEditar = computed(
    () => this.authService.isAdmin() || this.authService.temPapel(PAPEL_PATRIMONIO_EDITOR)
  );

  protected readonly mostrarFormEmprestimo = signal(false);
  protected quemRetirou = '';
  protected observacaoRetirada = '';
  protected quantidadeEmprestimo = 1;
  protected readonly salvandoEmprestimo = signal(false);
  protected readonly erroEmprestimo = signal<string | null>(null);

  protected readonly devolvendoId = signal<string | null>(null);
  protected quemDevolveu = '';
  protected observacaoDevolucao = '';
  protected readonly salvandoDevolucao = signal(false);
  protected readonly erroDevolucao = signal<string | null>(null);

  protected readonly mostrarFormSituacao = signal<'inativar' | 'reativar' | null>(null);
  protected observacaoSituacao = '';
  protected readonly salvandoSituacao = signal(false);
  protected readonly erroSituacao = signal<string | null>(null);

  protected readonly mostrarFormBaixa = signal(false);
  protected observacaoBaixa = '';
  protected arquivoDefeito: File | null = null;
  protected readonly erroArquivoBaixa = signal<string | null>(null);
  protected readonly salvandoBaixa = signal(false);
  protected readonly erroBaixa = signal<string | null>(null);

  constructor() {
    this.carregar();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    forkJoin({
      item: this.patrimonioService.obter(this.itemId),
      emprestimos: this.patrimonioService.listarEmprestimos(this.itemId)
    }).subscribe({
      next: ({ item, emprestimos }) => {
        this.item.set(item);
        this.emprestimos.set(emprestimos);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar este item.');
        this.carregando.set(false);
      }
    });
  }

  protected abrirEmprestimo(): void {
    this.mostrarFormEmprestimo.set(true);
    this.quemRetirou = '';
    this.observacaoRetirada = '';
    this.quantidadeEmprestimo = 1;
    this.erroEmprestimo.set(null);
  }

  protected cancelarEmprestimo(): void {
    this.mostrarFormEmprestimo.set(false);
  }

  protected confirmarEmprestimo(): void {
    const item = this.item();
    if (!item) {
      return;
    }
    if (!this.quemRetirou.trim() || !this.observacaoRetirada.trim()) {
      this.erroEmprestimo.set('Preencha quem retirou e a observação.');
      return;
    }

    this.salvandoEmprestimo.set(true);
    this.erroEmprestimo.set(null);

    this.patrimonioService
      .emprestar(item.id, {
        quantidade: item.tipoControle === 'Unitario' ? 1 : this.quantidadeEmprestimo,
        quemRetirou: this.quemRetirou.trim(),
        observacao: this.observacaoRetirada.trim()
      })
      .subscribe({
        next: () => {
          this.salvandoEmprestimo.set(false);
          this.mostrarFormEmprestimo.set(false);
          this.carregar();
        },
        error: (erro: HttpErrorResponse) => {
          this.erroEmprestimo.set(mensagemDeErro(erro, 'Não foi possível registrar o empréstimo.'));
          this.salvandoEmprestimo.set(false);
        }
      });
  }

  protected abrirDevolucao(emprestimoId: string): void {
    this.devolvendoId.set(emprestimoId);
    this.quemDevolveu = '';
    this.observacaoDevolucao = '';
    this.erroDevolucao.set(null);
  }

  protected cancelarDevolucao(): void {
    this.devolvendoId.set(null);
  }

  protected confirmarDevolucao(): void {
    const item = this.item();
    const emprestimoId = this.devolvendoId();
    if (!item || !emprestimoId) {
      return;
    }
    if (!this.quemDevolveu.trim() || !this.observacaoDevolucao.trim()) {
      this.erroDevolucao.set('Preencha quem devolveu e a observação.');
      return;
    }

    this.salvandoDevolucao.set(true);
    this.erroDevolucao.set(null);

    this.patrimonioService
      .devolver(item.id, emprestimoId, {
        quemDevolveu: this.quemDevolveu.trim(),
        observacao: this.observacaoDevolucao.trim()
      })
      .subscribe({
        next: () => {
          this.salvandoDevolucao.set(false);
          this.devolvendoId.set(null);
          this.carregar();
        },
        error: (erro: HttpErrorResponse) => {
          this.erroDevolucao.set(mensagemDeErro(erro, 'Não foi possível registrar a devolução.'));
          this.salvandoDevolucao.set(false);
        }
      });
  }

  protected abrirSituacao(acao: 'inativar' | 'reativar'): void {
    this.mostrarFormSituacao.set(acao);
    this.observacaoSituacao = '';
    this.erroSituacao.set(null);
  }

  protected cancelarSituacao(): void {
    this.mostrarFormSituacao.set(null);
  }

  protected confirmarSituacao(): void {
    const item = this.item();
    const acao = this.mostrarFormSituacao();
    if (!item || !acao) {
      return;
    }
    if (acao === 'inativar' && !this.observacaoSituacao.trim()) {
      this.erroSituacao.set('Informe uma observação explicando o motivo da inativação.');
      return;
    }

    this.salvandoSituacao.set(true);
    this.erroSituacao.set(null);

    this.patrimonioService
      .alterarSituacao(item.id, { ativo: acao === 'reativar', observacao: this.observacaoSituacao.trim() || null })
      .subscribe({
        next: (atualizado) => {
          this.item.set(atualizado);
          this.salvandoSituacao.set(false);
          this.mostrarFormSituacao.set(null);
        },
        error: (erro: HttpErrorResponse) => {
          this.erroSituacao.set(mensagemDeErro(erro, 'Não foi possível alterar a situação.'));
          this.salvandoSituacao.set(false);
        }
      });
  }

  protected abrirBaixa(): void {
    this.mostrarFormBaixa.set(true);
    this.observacaoBaixa = '';
    this.arquivoDefeito = null;
    this.erroArquivoBaixa.set(null);
    this.erroBaixa.set(null);
  }

  protected cancelarBaixa(): void {
    this.mostrarFormBaixa.set(false);
  }

  protected onSelecionarArquivoDefeito(evento: Event): void {
    const input = evento.target as HTMLInputElement;
    const arquivo = input.files?.[0] ?? null;

    if (!arquivo) {
      this.arquivoDefeito = null;
      return;
    }

    const problema = validarArquivoImagem(arquivo);
    if (problema) {
      this.erroArquivoBaixa.set(problema);
      input.value = '';
      this.arquivoDefeito = null;
      return;
    }

    this.erroArquivoBaixa.set(null);
    this.arquivoDefeito = arquivo;
  }

  protected confirmarBaixa(): void {
    const item = this.item();
    if (!item) {
      return;
    }
    if (!this.arquivoDefeito) {
      this.erroBaixa.set('Selecione uma foto mostrando o defeito.');
      return;
    }
    if (!this.observacaoBaixa.trim()) {
      this.erroBaixa.set('Informe uma observação com as informações do descarte.');
      return;
    }

    this.salvandoBaixa.set(true);
    this.erroBaixa.set(null);

    this.patrimonioService.darBaixa(item.id, this.arquivoDefeito, this.observacaoBaixa.trim()).subscribe({
      next: (atualizado) => {
        this.item.set(atualizado);
        this.salvandoBaixa.set(false);
        this.mostrarFormBaixa.set(false);
      },
      error: (erro: HttpErrorResponse) => {
        this.erroBaixa.set(mensagemDeErro(erro, 'Não foi possível dar baixa neste item.'));
        this.salvandoBaixa.set(false);
      }
    });
  }
}
