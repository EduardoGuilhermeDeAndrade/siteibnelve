export interface LocalAdmin {
  id: string;
  nome: string;
  tipo: string;
  endereco: string;
  bairro: string;
  cidade: string;
  estado: string | null;
  cep: string | null;
  googleMapsUrl: string | null;
  latitude: number | null;
  longitude: number | null;
  ativo: boolean;
  ordem: number;
}

export interface LocalUpsert {
  nome: string;
  tipo: string;
  endereco: string;
  bairro: string;
  cidade: string;
  estado: string | null;
  cep: string | null;
  googleMapsUrl: string | null;
  latitude: number | null;
  longitude: number | null;
  ativo: boolean;
  ordem: number;
}
