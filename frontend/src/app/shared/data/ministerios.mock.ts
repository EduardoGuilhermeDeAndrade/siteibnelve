export interface Ministerio {
  id: string;
  nome: string;
  lideres: string;
  imagem?: string;
  imagemAlt?: string;
}

/**
 * Dados de exemplo (mock) dos ministérios, conforme o CLAUDE.md.
 * Descrições curtas ainda não foram confirmadas pela liderança (checklist §6),
 * por isso não são inventadas aqui — só nome e liderança.
 */
export const MINISTERIOS_MOCK: Ministerio[] = [
  {
    id: 'infantil',
    nome: 'Ministério Infantil',
    lideres: 'Silvani',
    imagem: '/img/ministerio-infantil.jpg',
    imagemAlt: 'Crianças reunidas sentadas durante uma atividade do Ministério Infantil'
  },
  {
    id: 'mocidade',
    nome: 'Mocidade',
    lideres: 'Wendell e Hudson'
  },
  {
    id: 'mulheres',
    nome: 'Mulheres',
    lideres: 'Sildeni e Claudiene'
  },
  {
    id: 'louvor',
    nome: 'Louvor',
    lideres: 'Marcus'
  },
  {
    id: 'diaconos',
    nome: 'Diáconos',
    lideres: 'Ronildo'
  }
];
