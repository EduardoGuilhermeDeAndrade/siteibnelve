import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { mensagemDeErro } from '../../shared/errors/mensagem-erro';
import { FotoGaleria } from '../../shared/data/galeria.service';
import {
  FotoGaleriaEditavel,
  LIMITE_FOTOS,
  TAMANHO_MAXIMO_BYTES,
  TIPOS_IMAGEM_ACEITOS
} from './galeria-admin.models';
import { GaleriaAdminService } from './galeria-admin.service';

@Component({
  selector: 'app-galeria-admin',
  imports: [FormsModule],
  templateUrl: './galeria-admin.html',
  styleUrl: './galeria-admin.css'
})
export class GaleriaAdmin {
  private readonly galeriaAdminService = inject(GaleriaAdminService);

  protected readonly limiteFotos = LIMITE_FOTOS;

  protected readonly fotos = signal<FotoGaleriaEditavel[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  protected readonly enviandoNova = signal(false);
  protected readonly erroValidacao = signal<string | null>(null);
  protected readonly sucessoNova = signal(false);
  protected novaLegenda = '';

  protected readonly substituindoId = signal<string | null>(null);
  protected readonly excluindoId = signal<string | null>(null);
  protected readonly confirmandoId = signal<string | null>(null);

  constructor() {
    this.carregar();
  }

  private paraEditavel(foto: FotoGaleria): FotoGaleriaEditavel {
    return {
      ...foto,
      legendaRascunho: foto.legenda ?? '',
      ordemRascunho: foto.ordem,
      salvandoMetadados: false,
      sucessoMetadados: false,
      erroMetadados: null
    };
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.galeriaAdminService.listar().subscribe({
      next: (fotos) => {
        this.fotos.set(fotos.map((foto) => this.paraEditavel(foto)));
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar as fotos.');
        this.carregando.set(false);
      }
    });
  }

  private validarArquivo(arquivo: File): string | null {
    if (!TIPOS_IMAGEM_ACEITOS.includes(arquivo.type)) {
      return 'Formato não suportado. Envie JPEG, PNG ou WebP.';
    }
    if (arquivo.size > TAMANHO_MAXIMO_BYTES) {
      return 'Arquivo maior que o limite de 5 MB.';
    }
    return null;
  }

  protected onSelecionarArquivoNovo(evento: Event): void {
    const input = evento.target as HTMLInputElement;
    const arquivo = input.files?.[0];
    input.value = '';
    if (!arquivo) {
      return;
    }

    const problema = this.validarArquivo(arquivo);
    if (problema) {
      this.erroValidacao.set(problema);
      return;
    }

    this.erroValidacao.set(null);
    this.erro.set(null);
    this.enviandoNova.set(true);

    this.galeriaAdminService.adicionar(arquivo, this.novaLegenda.trim()).subscribe({
      next: (foto) => {
        this.fotos.update((lista) => [...lista, this.paraEditavel(foto)]);
        this.novaLegenda = '';
        this.enviandoNova.set(false);
        this.sucessoNova.set(true);
        setTimeout(() => this.sucessoNova.set(false), 4000);
      },
      error: (erro: HttpErrorResponse) => {
        this.erroValidacao.set(mensagemDeErro(erro, 'Não foi possível enviar a foto.'));
        this.enviandoNova.set(false);
      }
    });
  }

  protected onSelecionarArquivoSubstituir(id: string, evento: Event): void {
    const input = evento.target as HTMLInputElement;
    const arquivo = input.files?.[0];
    input.value = '';
    if (!arquivo) {
      return;
    }

    const problema = this.validarArquivo(arquivo);
    if (problema) {
      this.erroValidacao.set(problema);
      return;
    }

    this.erroValidacao.set(null);
    this.erro.set(null);
    this.substituindoId.set(id);

    this.galeriaAdminService.substituir(id, arquivo).subscribe({
      next: (foto) => {
        this.fotos.update((lista) => lista.map((f) => (f.id === id ? this.paraEditavel(foto) : f)));
        this.substituindoId.set(null);
      },
      error: (erro: HttpErrorResponse) => {
        this.erro.set(mensagemDeErro(erro, 'Não foi possível substituir a foto.'));
        this.substituindoId.set(null);
      }
    });
  }

  protected salvarMetadados(item: FotoGaleriaEditavel): void {
    item.salvandoMetadados = true;
    item.erroMetadados = null;

    this.galeriaAdminService.atualizarMetadados(item.id, item.legendaRascunho.trim() || null, item.ordemRascunho).subscribe({
      next: (foto) => {
        this.fotos.update((lista) => lista.map((f) => (f.id === item.id ? this.paraEditavel(foto) : f)));
        const atualizado = this.fotos().find((f) => f.id === item.id);
        if (atualizado) {
          atualizado.sucessoMetadados = true;
          setTimeout(() => {
            atualizado.sucessoMetadados = false;
          }, 4000);
        }
      },
      error: (erro: HttpErrorResponse) => {
        item.salvandoMetadados = false;
        item.erroMetadados = mensagemDeErro(erro, 'Não foi possível salvar a legenda/ordem.');
      }
    });
  }

  protected pedirConfirmacao(id: string): void {
    this.confirmandoId.set(id);
  }

  protected cancelarConfirmacao(): void {
    this.confirmandoId.set(null);
  }

  protected excluir(id: string): void {
    this.excluindoId.set(id);

    this.galeriaAdminService.excluir(id).subscribe({
      next: () => {
        this.fotos.update((lista) => lista.filter((f) => f.id !== id));
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      },
      error: () => {
        this.erro.set('Não foi possível excluir a foto.');
        this.excluindoId.set(null);
        this.confirmandoId.set(null);
      }
    });
  }
}
