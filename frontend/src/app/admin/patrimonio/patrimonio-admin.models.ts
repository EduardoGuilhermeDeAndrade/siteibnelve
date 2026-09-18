export type TipoControlePatrimonio = 'Unitario' | 'Quantidade';
export type SituacaoPatrimonio = 'Ativo' | 'Inativo' | 'Baixado';

export interface ItemPatrimonio {
  id: string;
  descricao: string;
  numeroPatrimonio: string | null;
  tipoControle: TipoControlePatrimonio;
  quantidadeTotal: number;
  quantidadeEmprestada: number;
  quantidadeDisponivel: number;
  fotoUrl: string | null;
  localId: string | null;
  localNome: string | null;
  situacao: SituacaoPatrimonio;
  observacao: string | null;
  fotoDefeitoUrl: string | null;
  observacaoBaixa: string | null;
  dataBaixa: string | null;
  dataAtualizacao: string;
}

export interface EmprestimoPatrimonio {
  id: string;
  quantidade: number;
  quemRetirou: string;
  observacaoRetirada: string;
  dataHoraRetirada: string;
  quemDevolveu: string | null;
  observacaoDevolucao: string | null;
  dataHoraDevolucao: string | null;
}

export interface ItemPatrimonioAtualizar {
  descricao: string;
  numeroPatrimonio: string | null;
  quantidadeTotal: number;
  localId: string | null;
  observacao: string | null;
}

export interface ItemPatrimonioSituacao {
  ativo: boolean;
  observacao: string | null;
}

export interface ItemPatrimonioEmprestimo {
  quantidade: number;
  quemRetirou: string;
  observacao: string;
}

export interface ItemPatrimonioDevolucao {
  quemDevolveu: string;
  observacao: string;
}
