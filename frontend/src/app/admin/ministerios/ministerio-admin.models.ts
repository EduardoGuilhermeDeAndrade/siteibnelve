export interface MinisterioAdmin {
  id: string;
  nome: string;
  lideres: string;
  descricao: string | null;
  imagemUrl: string | null;
  ativo: boolean;
  ordem: number;
}

export interface MinisterioUpsert {
  nome: string;
  lideres: string;
  descricao: string | null;
  imagemUrl: string | null;
  ativo: boolean;
  ordem: number;
}
