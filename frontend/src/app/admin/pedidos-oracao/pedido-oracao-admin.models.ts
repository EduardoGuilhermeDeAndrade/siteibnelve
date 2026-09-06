export interface PedidoOracaoAdmin {
  id: string;
  nome: string | null;
  anonimo: boolean;
  contato: string | null;
  desejaFalarComPastor: boolean;
  mensagem: string;
  lido: boolean;
  dataCriacao: string;
}
