export interface UsuarioAdmin {
  id: string;
  nomeCompleto: string;
  email: string;
  ativo: boolean;
  papel: string;
}

export const PAPEIS_USUARIO = [
  { valor: 'ADMIN', rotulo: 'Administrador geral' },
  { valor: 'PATRIMONIO_EDITOR', rotulo: 'Somente Patrimônio' }
];

export interface UsuarioCriar {
  nomeCompleto: string;
  email: string;
  senha: string;
  papel: string;
}

export interface UsuarioAtualizar {
  nomeCompleto: string;
}
