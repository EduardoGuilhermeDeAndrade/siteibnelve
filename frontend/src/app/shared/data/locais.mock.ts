export interface LocalIgreja {
  id: string;
  nome: string;
  tipo: string;
  endereco: string;
  bairro: string;
  cidade: string;
  uso?: string;
}

export const LOCAIS_MOCK: LocalIgreja[] = [
  {
    id: 'vereda',
    nome: 'Templo Vereda',
    tipo: 'Templo principal',
    endereco: 'Rua Rosina Tavares, 271',
    bairro: 'Vereda',
    cidade: 'Ribeirão das Neves/MG'
  },
  {
    id: 'liberdade',
    nome: 'Templo Liberdade',
    tipo: 'Segundo templo',
    endereco: 'Rua Quinze, 41',
    bairro: 'Liberdade',
    cidade: 'Ribeirão das Neves/MG',
    uso: 'Ceia aos domingos pela manhã, Escola Bíblica Dominical e eventos especiais.'
  }
];
