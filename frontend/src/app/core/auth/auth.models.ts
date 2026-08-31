export interface UsuarioLogado {
  id: string;
  email: string;
  nomeCompleto: string;
  roles: string[];
}

export interface LoginResponse {
  accessToken: string;
  expiraEm: string;
  usuario: UsuarioLogado;
}
