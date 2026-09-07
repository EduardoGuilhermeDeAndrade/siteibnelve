# CLAUDE.md — Site e Portal IBNELVE

## Objetivo
Criar um site institucional moderno, simples, responsivo e profissional para a Igreja Batista Nacional da Esperança do Liberdade e Vereda (IBNELVE), acompanhado de Portal Administrativo. O MVP não terá vídeos/streaming.

## Stack
- Angular + TypeScript
- ASP.NET Core Web API + C#
- PostgreSQL
- HTML semântico e CSS moderno, mobile-first
- API entre front-end e banco; nunca acesso direto do front ao PostgreSQL.

## Sitemap público
- Início
- Quem Somos
- Ministérios
- Agenda / Eventos
- Conecte-se / Contato
- Contribuições

## Portal Administrativo
- Login
- Dashboard
- Agenda
- Conteúdo do Site
- Ministérios
- Fotos
- Locais/Templos
- Contribuições
- Usuários
- Configurações

Inicialmente, pastores e secretária poderão acessar. No MVP pode existir um perfil ADMIN amplo, mas a arquitetura deve permitir futuramente perfis como PASTOR, SECRETARIA, LIDER_MINISTERIO e EDITOR_CONTEUDO. Não permitir cadastro público de administradores.

## Dados institucionais confirmados
- Nome oficial: Igreja Batista Nacional da Esperança do Liberdade e Vereda
- Sigla: IBNELVE
- Denominação: Batista Nacional
- História informada: 29 anos
- Fundação: 15 de novembro de 1996
- Pastor Presidente: Pr. Eduardo
- Fundador: Pr. Antônio
- Não divulgar telefones pessoais como telefone institucional.
- E-mail institucional confirmado: ibnelve271@gmail.com.br
- Redes sociais confirmadas: Instagram e Facebook, ambos @ibnelve (YouTube/TikTok ainda pendentes)

## Locais
### Templo Vereda — Principal
Rua Angelina Tavares, 271 - Vereda - Ribeirão das Neves/MG - CEP 33822-515

### Templo Liberdade — Segundo templo
Rua Quinze, 41 - Liberdade - Ribeirão das Neves/MG - CEP 33822-785

Uso atual informado do Templo Liberdade: Ceia aos domingos pela manhã, Escola Bíblica Dominical e eventos especiais.

Criar entidade `Local`. Eventos e séries devem usar `LocalId`, permitindo também local textual excepcional para atividades fora dos templos.

## Ministérios
- Infantil — Silvani
- Mocidade — Wendell e Hudson
- Mulheres — Sildeni e Claudiene
- Louvor — Marcus
- Diáconos — Ronildo

A Escola Bíblica Dominical deve existir como programação/categoria da Agenda, mesmo que não seja inicialmente um ministério.

## Contribuições
Faz parte do MVP.
- PIX: CNPJ
- Chave: 11.080.185/0001-08
- Favorecido: Igreja Batista Nacional da Esperança do Liberdade e Vereda
- Exibir botão “Copiar chave PIX”.
- Preparar suporte futuro a QR Code PIX.

## Conteúdo institucional provisório
Estes textos são iniciais e devem permanecer editáveis no Portal, sujeitos à aprovação da liderança.

### Quem Somos
Há 29 anos, a Igreja Batista Nacional da Esperança do Liberdade e Vereda é uma comunidade cristã em Ribeirão das Neves comprometida com a Palavra de Deus, a comunhão, o discipulado e a proclamação do Evangelho de Jesus Cristo. Com presença nos bairros Vereda e Liberdade, buscamos ser uma igreja acolhedora, onde pessoas e famílias possam conhecer a Cristo, crescer na fé e servir a Deus e ao próximo.

### Missão
Glorificar a Deus, proclamando o Evangelho de Jesus Cristo, fazendo discípulos, ensinando a Palavra e servindo pessoas e famílias por meio de uma igreja viva, acolhedora e comprometida com a Grande Comissão.

### Visão
Ser uma igreja bíblica, relevante e acolhedora, que cresce em comunhão, maturidade espiritual e compromisso missionário, alcançando vidas e famílias para Cristo em nossa comunidade e além dela.

### Valores provisórios
- Palavra de Deus
- Comunhão
- Discipulado
- Família
- Serviço
- Missões

### No que cremos — resumo provisório
- Bíblia como Palavra de Deus e regra de fé e prática.
- Deus — Pai, Filho e Espírito Santo.
- Jesus Cristo como Senhor e Salvador.
- Salvação pela graça de Deus mediante a fé em Jesus Cristo.
- Igreja como comunidade de discípulos chamada à comunhão, serviço e missão.
- Missão de anunciar o Evangelho e fazer discípulos.

Revisar posteriormente com a liderança e a Declaração de Fé Batista Nacional.

# Agenda — Fonte Única de Verdade
A Agenda é parte central do MVP. Datas e horários não devem ser duplicados manualmente nas páginas quando puderem ser derivados dela.

## Visibilidade
- PUBLICO
- INTERNO
- PESSOAL

PUBLICO pode aparecer no site. INTERNO somente para usuários autorizados. PESSOAL somente para o proprietário. A API deve aplicar essas regras; nunca confiar apenas no Angular.

## Categorias
Exemplos:
CULTO, CEIA, ESCOLA_BIBLICA, JOVENS, MULHERES, HOMENS, OBREIROS, LIDERANCA, BATISMO, CASAMENTO, REUNIAO, EVENTO_ESPECIAL, ADMINISTRATIVO, PESSOAL.

Categoria e visibilidade são conceitos diferentes.

## Status
- RASCUNHO
- PUBLICADO
- CANCELADO

Somente eventos ativos + PUBLICO + PUBLICADO podem alimentar o site.

## Próximo evento por categoria
Páginas devem consultar automaticamente a próxima ocorrência válida. Ex.: página da EBD mostra a próxima Escola Bíblica; quando ela passar, mostra a seguinte. O mesmo vale para Mocidade, Ceia, Cultos etc.

A Home mostra inicialmente os 3 próximos eventos públicos, ordenados por data/hora. Eventos passados desaparecem automaticamente.

## Recorrência
Suportar no MVP:
- sem repetição
- semanal
- a cada N semanas
- mensal por dia
- mensal por posição (primeiro/segundo/terceiro/quarto/último + dia da semana)
- anual

Exemplos:
- Jovens: a cada 2 semanas, sábado.
- Ceia: primeiro domingo do mês às 08:30.
- Culto de Louvor e Adoração: domingos às 18:00, exceto primeiro domingo.

## Séries, ocorrências e exceções
Distinguir:
- Evento: evento único.
- SerieEvento: regra recorrente.
- OcorrenciaEvento: data calculada.
- ExcecaoEvento: alteração/cancelamento de uma ocorrência.
- Divisão de série: quando “esta e as próximas” cria nova série futura.

Uma exceção específica prevalece sobre a recorrência.

Ao editar evento recorrente oferecer:
- Somente esta ocorrência
- Esta e as próximas
- Toda a série

“Esta e as próximas” deve preservar o passado, encerrar a série anterior e criar nova série a partir da alteração.

Antes de recalcular uma série, mostrar prévia das próximas datas e pedir confirmação.

Permitir mover uma ocorrência sem duplicá-la e permitir evento extraordinário sem necessariamente reiniciar a série.

Não gerar fisicamente eventos para muitos anos; persistir regras e exceções e calcular ocorrências por janela de consulta.

## Exemplo de domingo
- Ceia: primeiro domingo de cada mês, 08:30, Templo Liberdade.
- Culto de Louvor e Adoração: demais domingos, 18:00.
Uma ocorrência específica pode ser alterada (ex.: Ceia excepcionalmente à noite) sem alterar os meses seguintes.

## Modelos conceituais
### Local
Id, Nome, Tipo, Endereco, Bairro, Cidade, Estado, CEP, GoogleMapsUrl, Latitude, Longitude, Ativo, Ordem.

### SerieEvento
Id, Titulo, Descricao, CategoriaId, Visibilidade, Status, TipoRecorrencia, Intervalo, DiaSemana, DiaMes, PosicaoNoMes, HoraInicio, HoraFim, DataInicioRecorrencia, DataFimRecorrencia, LocalId, ImagemUrl, Destaque, Ativo, CriadoPorUsuarioId, DataCriacao, AtualizadoPorUsuarioId, DataAtualizacao.

### ExcecaoEvento
Id, SerieEventoId, DataHoraOriginal, NovaDataHoraInicio, NovaDataHoraFim, TituloSubstituto, DescricaoSubstituta, LocalSubstituto/LocalId, Cancelado, Motivo, CriadoPorUsuarioId, DataCriacao, AtualizadoPorUsuarioId, DataAtualizacao.

### Fuso horário (implementado na Fase 6)
`HoraInicio`/`HoraFim` de `SerieEvento` e `NovaHoraInicio`/`NovaHoraFim` de `ExcecaoEvento` são sempre horário local de Brasília (o horário que o admin digita e que aparece na Agenda) — nunca UTC. O backend converte para `DateTimeOffset` aplicando um offset fixo -03:00 (`Domain/OcorrenciaCalculator.OffsetBrasilia`). O Brasil não observa horário de verão desde 2019, então um offset fixo é suficiente; não usar `TimeZoneInfo`/banco de fusos horários para isso enquanto a igreja operar num único fuso.

# UI/UX
- Mobile-first.
- Layout limpo, contemporâneo, institucional e acolhedor.
- Fotografias reais da IBNELVE em preferência a bancos de imagem.
- Hero forte, sem carrossel automático.
- Hierarquia tipográfica clara, bastante espaço, poucas cores, CTAs objetivos.
- CSS Grid, Flexbox, clamp(), min(), max(), minmax(), auto-fit/auto-fill e container queries quando justificadas.
- Criar Design System com CSS Custom Properties para cores, tipografia, espaçamento, radius, sombras, largura, breakpoints e animações.
- Evitar template genérico, excesso de banners, sombras pesadas, animações chamativas e dependências desnecessárias.

## Fotos já disponíveis
- Foto horizontal com letreiro IBNELVE e fotos da comunidade: candidata ao Hero.
- Foto de louvor: Cultos/Louvor.
- Foto da congregação/comunidade: Quem Somos/Família.
- Foto individual: Liderança somente após confirmação de nome/cargo/autorização.

# Segurança
- HTTPS
- autenticação segura
- hash adequado de senha
- autorização na API
- validação server-side
- uploads validados e limitados
- secrets/connection strings fora do repositório
- CORS restrito
- logs sem dados sensíveis
- rate limiting quando necessário
- nunca inventar criptografia própria

# Acessibilidade, Performance e SEO
- HTML semântico, teclado, foco visível, contraste, labels, alt, headings corretos e prefers-reduced-motion.
- Priorizar Core Web Vitals, imagens WebP/AVIF, lazy loading, code splitting, cache e fontes otimizadas.
- Title, meta description, Open Graph, URLs legíveis, sitemap.xml, robots.txt e Schema.org adequado.

# Fluxo de desenvolvimento
1. Fundação: Angular, API, PostgreSQL, Design System e layout.
2. Site público inicialmente com mock data.
3. API e banco.
4. Integração do site com API.
5. Portal administrativo.
6. Agenda recorrente/exceções e integração com páginas.
7. Segurança, acessibilidade, SEO, testes e performance.
8. Deploy.

# Regra para Claude Code
Antes de implementar uma fase:
1. analisar código existente;
2. resumir o que será alterado;
3. preservar padrões definidos;
4. evitar dependências desnecessárias;
5. não reescrever código funcionando sem necessidade;
6. executar build/testes;
7. corrigir erros relevantes;
8. atualizar este arquivo quando decisões arquiteturais mudarem.

Preferir solução simples que preserve segurança, qualidade e evolução.

# Pendências — não inventar
- links definitivos do Google Maps (Place ID oficial, se a igreja tiver um);
- história completa;
- missão/visão/valores oficiais após revisão;
- declaração de fé definitiva;
- descrições dos ministérios;
- programação regular completa;
- demais lideranças públicas;
- QR Code PIX;
- logo original em alta resolução/SVG.
