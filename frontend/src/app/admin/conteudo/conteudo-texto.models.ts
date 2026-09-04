export interface ConteudoTexto {
  chave: string;
  titulo: string;
  conteudo: string;
  dataAtualizacao: string;
}

export interface ConteudoTextoUpsert {
  titulo: string;
  conteudo: string;
}

export interface TextoEditavelState extends ConteudoTexto {
  rascunho: string;
  salvando: boolean;
  sucesso: boolean;
  erro: string | null;
}
