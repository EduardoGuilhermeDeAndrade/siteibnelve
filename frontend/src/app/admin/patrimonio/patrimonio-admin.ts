import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AuthService, PAPEL_PATRIMONIO_EDITOR } from '../../core/auth/auth.service';
import { mensagemDeErro } from '../../shared/errors/mensagem-erro';
import { EmprestimoPatrimonio, ItemPatrimonio } from './patrimonio-admin.models';
import { PatrimonioService } from './patrimonio.service';

@Component({
  selector: 'app-patrimonio-admin',
  imports: [FormsModule, RouterLink],
  templateUrl: './patrimonio-admin.html',
  styleUrl: './patrimonio-admin.css'
})
export class PatrimonioAdmin {
  private readonly patrimonioService = inject(PatrimonioService);
  protected readonly authService = inject(AuthService);

  protected readonly itens = signal<ItemPatrimonio[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly podeEditar = computed(
    () => this.authService.isAdmin() || this.authService.temPapel(PAPEL_PATRIMONIO_EDITOR)
  );

  protected readonly emprestandoId = signal<string | null>(null);
  protected quemRetirou = '';
  protected observacaoRetirada = '';
  protected quantidadeEmprestimo = 1;
  protected readonly salvandoEmprestimo = signal(false);
  protected readonly erroEmprestimo = signal<string | null>(null);

  protected readonly devolvendoId = signal<string | null>(null);
  protected readonly emprestimoParaDevolver = signal<EmprestimoPatrimonio | null>(null);
  protected readonly multiplosEmprestimosAtivos = signal(false);
  protected readonly carregandoEmprestimos = signal(false);
  protected quemDevolveu = '';
  protected observacaoDevolucao = '';
  protected readonly salvandoDevolucao = signal(false);
  protected readonly erroDevolucao = signal<string | null>(null);

  constructor() {
    this.carregar();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.patrimonioService.listar().subscribe({
      next: (itens) => {
        this.itens.set(itens);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o patrimônio.');
        this.carregando.set(false);
      }
    });
  }

  protected situacaoTexto(item: ItemPatrimonio): string {
    if (item.situacao === 'Baixado') {
      return 'Baixado';
    }
    if (item.situacao === 'Inativo') {
      return 'Inativo';
    }

    const local = item.localNome ?? 'Sem local definido';
    if (item.tipoControle === 'Unitario') {
      return item.quantidadeEmprestada > 0 ? 'Emprestado' : local;
    }

    return item.quantidadeEmprestada > 0
      ? `${local} — ${item.quantidadeDisponivel} disponíveis, ${item.quantidadeEmprestada} emprestadas`
      : local;
  }

  protected abrirEmprestimo(item: ItemPatrimonio): void {
    this.emprestandoId.set(item.id);
    this.quemRetirou = '';
    this.observacaoRetirada = '';
    this.quantidadeEmprestimo = 1;
    this.erroEmprestimo.set(null);
  }

  protected cancelarEmprestimo(): void {
    this.emprestandoId.set(null);
  }

  protected confirmarEmprestimo(item: ItemPatrimonio): void {
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
          this.emprestandoId.set(null);
          this.carregar();
        },
        error: (erro: HttpErrorResponse) => {
          this.erroEmprestimo.set(mensagemDeErro(erro, 'Não foi possível registrar o empréstimo.'));
          this.salvandoEmprestimo.set(false);
        }
      });
  }

  protected abrirDevolucao(item: ItemPatrimonio): void {
    this.devolvendoId.set(item.id);
    this.emprestimoParaDevolver.set(null);
    this.multiplosEmprestimosAtivos.set(false);
    this.erroDevolucao.set(null);
    this.quemDevolveu = '';
    this.observacaoDevolucao = '';
    this.carregandoEmprestimos.set(true);

    this.patrimonioService.listarEmprestimos(item.id).subscribe({
      next: (emprestimos) => {
        const ativos = emprestimos.filter((e) => !e.dataHoraDevolucao);
        if (ativos.length === 1) {
          this.emprestimoParaDevolver.set(ativos[0]);
        } else {
          this.multiplosEmprestimosAtivos.set(true);
        }
        this.carregandoEmprestimos.set(false);
      },
      error: () => {
        this.erroDevolucao.set('Não foi possível carregar os empréstimos deste item.');
        this.carregandoEmprestimos.set(false);
      }
    });
  }

  protected cancelarDevolucao(): void {
    this.devolvendoId.set(null);
  }

  protected confirmarDevolucao(item: ItemPatrimonio): void {
    const emprestimo = this.emprestimoParaDevolver();
    if (!emprestimo) {
      return;
    }
    if (!this.quemDevolveu.trim() || !this.observacaoDevolucao.trim()) {
      this.erroDevolucao.set('Preencha quem devolveu e a observação.');
      return;
    }

    this.salvandoDevolucao.set(true);
    this.erroDevolucao.set(null);

    this.patrimonioService
      .devolver(item.id, emprestimo.id, {
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
}
