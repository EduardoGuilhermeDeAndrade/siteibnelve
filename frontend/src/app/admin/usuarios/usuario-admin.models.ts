export interface UsuarioAdmin {
  id: string;
  nomeCompleto: string;
  email: string;
  ativo: boolean;
}

export interface UsuarioCriar {
  nomeCompleto: string;
  email: string;
  senha: string;
}

export interface UsuarioAtualizar {
  nomeCompleto: string;
}
