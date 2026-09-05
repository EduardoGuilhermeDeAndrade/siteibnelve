# Plano Evolutivo — Site e Portal IBNELVE

> Baseado no `CLAUDE.md` (visão/stack/regras) e no `CheckList_informacoes_site_IBNELVE.md` (pendências de conteúdo). Sem deploy por enquanto — foco em ambiente local/dev.

## Estado atual do repositório
- Nenhum código ainda: só `CLAUDE.md`, o checklist e a pasta `Img Antigas/` com fotos, mockups de página (`pageOne/Two/Tree*.png`), logo (`Nova Logo Ibnelve.jpeg`) e imagem do PIX.
- Não é repositório git ainda.
- **Divergência encontrada:** a logo em `Nova Logo Ibnelve.jpeg` traz "IGREJA BATISTA NOVO ELVE", diferente do nome oficial no `CLAUDE.md` ("Esperança do Liberdade e Vereda"). Precisa validação antes de usar a logo no site — ver Fase 0.

---

## Fase 0 — Preparação (sem código) ✅ concluída em 2026-08-16
Objetivo: reduzir pendências que bloqueiam decisões de design/estrutura, e organizar o repositório.

- [x] Iniciar repositório git local (`git init` feito; `.gitignore` criado para Angular/.NET/Postgres/secrets).
- [ ] Criar estrutura de pastas `frontend/`/`backend/` — adiado para a Fase 1 (o scaffolding do Angular/.NET cria essas pastas sozinho; criá-las vazias agora só geraria conflito).
- [x] Organizar `Img Antigas/` em subpastas por uso: `logo/`, `culto/`, `comunidade/`, `ministerio-infantil/`, `visitas-acao-social/`, `eventos-especiais/`, `contribuicoes/`, `elementos-graficos/`, `lideranca-cutouts-pendente-confirmacao/`, `revisar/`.
- [ ] Confirmar com a liderança os itens do checklist que bloqueiam UI — continua pendente, ver `CheckList_informacoes_site_IBNELVE.md`.
- [ ] Definir paleta e tipografia provisórias — adiado para a Fase 1 (Design System), pode usar as fotos de `culto/` e `comunidade/` como referência de tom.

### Achados da curadoria de fotos (Fase 0)
- **Divergência na logo:** `Img Antigas/logo/Nova Logo Ibnelve.jpeg` traz "IGREJA BATISTA NOVO ELVE", diferente do nome oficial. Precisa validação antes de usar.
- **`lideranca-cutouts-pendente-confirmacao/`:** 11 arquivos com recorte de fundo (Canva), representando **3 pessoas distintas**. Nenhuma foi usada em conteúdo até confirmar nome/cargo/autorização (regra do `CLAUDE.md`).
- **Fortes candidatas a Hero/Quem Somos:** `comunidade/IMG_8849.jpg` e `comunidade/Carrousel1.png` (mural/photo-wall com o nome "Ibnelve"), `comunidade/carrousel2.png` (congregação sorrindo, boa para "Família").
- **Candidata a Ceia:** `culto/WhatsApp Image 2023-10-31 at 19.46.11 (2).jpeg` (púlpito com uvas, elementos de comunhão).
- **`revisar/`:** 2 fotos de uso incerto — uma parece foto pessoal (não institucional); decidir com a liderança se entram no acervo do site.

Saída: repositório git iniciado + acervo de fotos organizado e catalogado + pendências atualizadas no checklist.

## Fase 1 — Fundação técnica ✅ validada rodando localmente em 2026-08-17
Objetivo: scaffolding rodando localmente, sem funcionalidade real ainda.

- [x] `frontend/`: Angular novo (standalone components, roteamento, mobile-first), Design System em CSS Custom Properties (cores, tipografia, espaçamento, radius, sombras, breakpoints) — `frontend/src/styles.css`.
- [x] `backend/`: ASP.NET Core Web API novo (Controllers, `HealthController`), OpenAPI em dev, CORS configurável via `Cors:AllowedOrigins`.
- [x] `PostgreSQL` local via docker-compose — container `ibnelve-postgres` (postgres:16-alpine) rodando e saudável (`pg_isready` OK, porta 5432). Precisou de WSL2 instalado manualmente pelo usuário nesta máquina (Docker Desktop não iniciava sem nenhuma distro WSL); depois disso `docker compose up -d postgres` funcionou normalmente.
- [x] Layout base do site público (Header, Footer, navegação do sitemap) só com placeholders — confirmado visualmente no browser.
- [x] Build e execução local validados nesta sessão:
  - Backend: `dotnet build` sem erros; `dotnet run --project backend/Ibnelve.Api` sobe em `http://localhost:5240`; `GET /api/health` → `{"status":"ok"}`.
  - Frontend: `npm start` (Angular CLI) compila e serve em `http://localhost:4200`; header/footer/rotas (Início, Quem Somos, Ministérios, Agenda, Conecte-se, Contribuições) renderizando.
  - Postgres: container up e `healthy` via docker-compose; ainda não há EF Core/Npgsql no backend (isso é escopo da Fase 3 — modelagem de dados), então a API não se conecta ao banco ainda, só o container está disponível.
- Adicionado `.claude/launch.json` com os perfis `backend` (dotnet run, porta 5240) e `frontend` (npm start, porta 4200) para facilitar reexecução local via preview.

## Fase 2 — Site público com mock data ✅ concluída em 2026-08-25
Objetivo: todas as páginas públicas navegáveis, com dados mockados no Angular (sem API ainda).

- [x] 6 páginas públicas implementadas (Início, Quem Somos, Ministérios, Agenda, Conecte-se, Contribuições), todas com conteúdo real do `CLAUDE.md` — nenhum texto/descrição inventado; pendências (descrições de ministério, e-mail, local exato do culto de domingo à noite) marcadas na tela como "a confirmar"/"em validação" em vez de preenchidas.
- [x] Fotos reais do acervo `Img Antigas/` copiadas para `frontend/public/img/`: `carrousel2.png` (Hero da Home), `IMG_8849.jpg` (Quem Somos), `WhatsApp...19.46.09(2).jpeg` (Ministério Infantil). Fotos de `lideranca-cutouts-pendente-confirmacao/` e `revisar/` não usadas (regra do `CLAUDE.md`). Logo atual não usada (baixo contraste, identidade pendente de validação) — mantido wordmark textual "IBNELVE".
- [x] Mock data tipado e centralizado em `frontend/src/app/shared/data/` (`eventos.mock.ts`, `ministerios.mock.ts`, `locais.mock.ts`) + utilitário de recorrência simplificado (`recorrencia.util.ts`) que calcula a próxima ocorrência a partir de agora (Ceia 1º domingo 08h30, Culto domingos 18h exceto 1º domingo, EBD domingos de manhã, Mocidade quinzenal aos sábados) — não é o motor de recorrência completo, isso fica para a Fase 6.
- [x] Componentes compartilhados em `frontend/src/app/shared/ui/`: `section-heading` (com `level` 1/2 para nunca faltar `<h1>` na página), `event-card`, `ministerio-card`.
- [x] Home com Hero estático (sem carrossel automático), 3 próximos eventos públicos, ministérios em destaque, os dois templos e CTA de Contribuições.
- [x] Agenda com filtro por categoria (client-side) sobre os eventos mock.
- [x] Contato com os dois templos e formulário de Pedido de Oração (validação client-side, erro focado no campo, só front-end — sem envio real, isso é Fase 5).
- [x] Contribuições com chave PIX fixa e botão "Copiar chave PIX" funcional (Clipboard API + confirmação acessível via `aria-live`).
- [x] Locale `pt-BR` registrado no Angular (`app.config.ts`) para datas por extenso corretas na Agenda.
- [x] Acessibilidade: skip link para o conteúdo, hierarquia de headings corrigida (cada página tem exatamente um `<h1>`), labels associados a todos os campos de formulário, `alt` descritivo e `width`/`height` em todas as fotos, `loading="lazy"` fora do Hero.
- [x] Revisão aplicada com a skill `web-design-guidelines` (Web Interface Guidelines) sobre todos os arquivos novos; achados corrigidos (heading hierarchy, capitalização de datas em português, foco no primeiro erro do formulário).
- [x] `ng build` e navegação manual (via preview) validados nas 6 rotas, sem erros de console.

## Fase 3 — Modelagem de dados e API real ✅ núcleo concluído em 2026-09-04 (junto com a Fase 5)
Objetivo: schema definitivo e endpoints reais substituindo os mocks.

- [x] Entidades implementadas via EF Core/PostgreSQL: `Local`, `Evento` (versão simplificada — avulso ou semanal fixo, não o `SerieEvento`/`OcorrenciaEvento`/`ExcecaoEvento` completo, isso continua Fase 6), `Ministerio`, `ImagemSite`, `ConteudoTexto` (textos institucionais), `RefreshToken`, mais as tabelas do ASP.NET Core Identity (`ApplicationUser`/`ApplicationRole`).
- [x] `ConfiguracaoContribuicao` (config, singleton — 1 linha só) modelada e integrada em 2026-09-05: chave PIX, tipo de chave e favorecido saem do banco (antes eram fixos no frontend); QR Code PIX reaproveita a infraestrutura de imagens por chave já existente (`contribuicao-qrcode`), sem código de upload novo. Tela própria "Contribuições" no Portal Admin (GET/PUT, sem criar/listar/excluir).
- [x] Migrations aplicadas (`PortalAdminNucleo`, `ConteudoTexto`, `Ministerios`, `SerieEventoRecorrencia`, `ConfiguracaoContribuicao`).
- [x] Endpoints CRUD completos (`/api/admin/**`, autenticados) para Eventos, Locais, Ministérios, Conteúdo de Texto e Imagens.
- [x] Regras de visibilidade (PUBLICO/INTERNO/PESSOAL) e status (RASCUNHO/PUBLICADO/CANCELADO) aplicadas na API: os endpoints públicos (`/api/eventos`, `/api/locais`, `/api/ministerios`) só devolvem o que é `PUBLICO`+`PUBLICADO` (eventos) ou `Ativo` (locais/ministérios) — filtro no servidor, não no Angular.

## Fase 4 — Integração site público ↔ API ✅ concluída em 2026-09-04
Objetivo: site público consumindo dados reais.

- [x] Home, Quem Somos, Ministérios, Agenda e Contato passam a ler da API real (nenhuma página pública usa mock data — os 4 arquivos `*.mock.ts` + `recorrencia.util.ts` foram apagados).
- [x] Agenda pública: eventos avulsos e semanais fixos com a próxima ocorrência calculada no servidor (`RecorrenciaCalculator`, C#) — cobre os casos que a entidade `Evento` suporta hoje; o motor completo (mensal por posição, exceções, divisão de série) continua Fase 6.
- [x] Tratamento de loading/erro no frontend (sinais `carregando`/`erro` por página, mensagens amigáveis). "Cache leve" = sem biblioteca nova, cada navegação busca de novo.
- [x] Endpoints públicos anônimos novos (`Controllers/`, sem `[Authorize]`): `/api/eventos`, `/api/locais`, `/api/ministerios`, `/api/conteudo-texto`, `/api/imagens-site` — distintos dos `/api/admin/**` autenticados.
- [x] Testado ao vivo: evento criado no Portal aparece na Agenda pública e na Home sem alterar código.

## Fase 5 — Portal Administrativo (núcleo) ✅ núcleo concluído em 2026-09-04
Objetivo: pastores/secretária conseguem logar e gerenciar conteúdo básico.

- [x] Autenticação: ASP.NET Core Identity + JWT (access token de 20 min em memória no Angular, nunca em localStorage) + refresh token rotativo em cookie httpOnly. Rate limiting no login. Sem cadastro público — só o primeiro ADMIN semeado via `user-secrets`.
- [x] Perfil ADMIN amplo no MVP; modelo de roles do Identity já preparado para adicionar PASTOR/SECRETARIA/LIDER_MINISTERIO/EDITOR_CONTEUDO depois (basta seedar o role, sem mudar código).
- [x] Dashboard simples (`/admin`).
- [x] Telas implementadas: **Agenda** (CRUD completo de eventos), **Locais/Templos** (CRUD), **Ministérios** (CRUD), **Conteúdo do Site** (textos institucionais editáveis + upload de imagens por chave, S3-compatível via MinIO em dev), **Contribuições** (configuração de chave PIX), **Usuários** (criar/editar/ativar/desativar/redefinir senha de administradores) e **Configurações** (trocar a própria senha).
- [x] Autorização checada na API (`[Authorize(Roles = "ADMIN")]` em todo `/api/admin/**`), nunca só no guard do Angular — testado explicitamente (sem token → 401).
- [x] Usuários fechado em 2026-09-05: gerencia só o papel ADMIN (perfil amplo do MVP, conforme o próprio `CLAUDE.md` autoriza), reaproveitando 100% as tabelas do ASP.NET Identity — nenhuma migration nova. Desativação em vez de exclusão definitiva (via lockout do Identity), com travas testadas: não deixa desativar a própria conta nem o último ADMIN ativo, e corrigiu uma lacuna real (`Refresh()` não conferia usuário bloqueado, então uma conta desativada continuava renovando sessão via refresh token válido). Configurações ficou restrita a "trocar minha senha" — o `CLAUDE.md` não detalha mais nada para essa tela, e o usuário confirmou esse recorte para não inventar conteúdo institucional que ainda está pendente de confirmação.

## Fase 6 — Agenda recorrente + exceções (parte mais complexa) ✅ concluída em 2026-09-04
Objetivo: motor de recorrência completo, conforme especificado no `CLAUDE.md`.

- [x] `SerieEvento` (substitui a `Evento` simplificada) + `ExcecaoEvento`; `OcorrenciaEvento` não é tabela — é um `record` (`Domain/Ocorrencia.cs`) sempre calculado em memória, nunca persistido.
- [x] 6 tipos de recorrência implementados e testados contra o calendário real: sem repetição, semanal, a cada N semanas, mensal por dia (pula meses sem aquele dia), mensal por posição (1º-4º/último + dia da semana), anual.
- [x] Cálculo de ocorrências por **janela de consulta** (`Domain/OcorrenciaCalculator.cs`) — nunca gera fisicamente anos de eventos.
- [x] CRUD de `SerieEvento` + `ExcecaoEvento` (`Controllers/Admin/SeriesEventoController.cs`); exceção sempre prevalece sobre a regra da série.
- [x] Edição com 3 escopos: "somente esta ocorrência" (`PUT/DELETE .../ocorrencias/{data}`) / "esta e as próximas" (`POST .../dividir`, encerra a série antiga e cria uma nova a partir da data) / "toda a série" (PUT normal).
- [x] Prévia das próximas ocorrências antes de qualquer confirmação (`GET .../ocorrencias`), tanto no fluxo de divisão quanto na lista expansível da Agenda.
- [x] Portal: tela de Agenda com "Ver próximas ocorrências" expansível, mini-formulário inline para editar/cancelar/reverter uma ocorrência, e fluxo dedicado para "esta e as próximas".
- [x] Site público (Home/Agenda) somando ocorrências de todas as séries `PUBLICO`+`PUBLICADO`+`Ativo` numa janela; contrato `EventoPublicoDto` não mudou, nenhuma página pública precisou de alteração de código.
- [x] Validado com dados reais de exemplo do `CLAUDE.md` (Ceia 1º domingo, Culto semanal, Jovens a cada 2 semanas aos sábados) e com a migração dos dados já cadastrados (Culto de Louvor e Adoração) preservada sem perda.
- [x] Horários (`HoraInicio`/`HoraFim`/exceções) tratados como horário local de Brasília com offset fixo -03:00 (Brasil não observa horário de verão desde 2019) — decisão registrada no `CLAUDE.md`.

## Fase 7 — Segurança, acessibilidade, SEO, testes e performance ✅ recorte concluído em 2026-09-04
Levantamento inicial (3 agentes de exploração) mostrou que segurança já estava bem encaminhada e acessibilidade/performance já tinham núcleo das Fases 2/6; o recorte desta fase fechou as lacunas reais encontradas, documentando o que fica para depois (ver "Fora de escopo desta passada").

- [x] Segurança: `UseExceptionHandler`+`ProblemDetails` e `UseHsts` fora de `Development` (`Program.cs`); rate limiting adicionado no `refresh` (login já tinha); validação de magic-bytes (assinatura binária) no upload de imagem, além do `Content-Type` já checado (`ImagensController.cs`) — testado rejeitando um `.txt` disfarçado de PNG e aceitando um PNG real.
- [x] Acessibilidade: Portal Admin não tinha recebido a revisão que o site público já teve na Fase 2 — adicionado skip-link e `aria-label` na nav (`admin-layout.html`, mesmo padrão do `app.html` público); os 3 formulários que só faziam `markAllAsTouched()` (Locais, Ministérios, Agenda) passaram a focar o primeiro campo inválido no submit, igual ao Login já fazia. Revisão completa com a skill `web-design-guidelines` sobre todo o Portal Admin.
- [x] SEO: Open Graph, Twitter Card e Schema.org (`Church`) em `index.html`; `Meta` do Angular setando uma descrição própria por página pública (antes só existia uma description genérica e estática); `robots.txt`/`sitemap.xml` estáticos (6 rotas fixas) — com domínio placeholder `example.org` até a Fase 8 definir o domínio real.
- [x] Performance: `fetchpriority="high"` no Hero já estava implementado; code splitting já cobria todas as rotas lazy (confirmado, nada a mudar).
- [x] Testes: novo projeto `backend/Ibnelve.Api.Tests` (xUnit), 14 testes cobrindo os 6 tipos de recorrência do `OcorrenciaCalculator` contra datas reais de calendário, aplicação de exceções (troca de horário/cancelamento), `ProximaOcorrencia` e uma regressão para o offset de -03:00 — era zero antes, e é a peça que o `CLAUDE.md` aponta como maior risco de bugs.

### Fora de escopo desta passada (documentado, não esquecido)
- SSR/prerendering (mudaria a hospedagem — decisão melhor junto da Fase 8).
- Conversão de imagens para WebP/AVIF (sem `cwebp`/`sharp` disponível no ambiente sem adicionar dependência nova).
- Suíte ampla de testes de frontend (hoje é majoritariamente serviços HTTP finos/formulários, pouca lógica pura a testar).
- Rate limiting global em todos os endpoints (só login/refresh são alvos realistas de abuso hoje).
- Aviso de alterações não salvas ao navegar para fora de um formulário (`beforeunload`/guard) — identificado na revisão de UI, mas é uma funcionalidade nova, não um ajuste pontual de acessibilidade.

## Fase 8 — Deploy (fora de escopo agora)
Fica só planejado, não executado: hospedagem do Angular, da API .NET e do PostgreSQL, secrets fora do repositório, pipeline de build. Retomar quando o usuário decidir publicar.

---

## Trilha paralela — Conteúdo e assets (independe do código)
Pode avançar em qualquer fase, idealmente antes das Fases 2 e 5:
1. Validar nome/tagline da logo (divergência encontrada) e obter versão em alta resolução/SVG.
2. Confirmar e-mail institucional (`ibnelve@gmail.com.br` vs `ibnelve@gmail.com`).
3. Dia/ano exatos de fundação; história completa.
4. Revisão de Missão/Visão/Valores/"No que cremos" com a liderança.
5. Descrições curtas de cada ministério + fotos organizadas por ministério/templo.
6. Confirmar redes sociais atuais.
7. CEPs e links definitivos do Google Maps dos dois templos.
8. Definir funcionamento do Pedido de Oração (nome obrigatório? anônimo? destino).
9. Título/subtítulo/CTA do Hero.

Essas pendências não bloqueiam o código de infraestrutura (Fases 0–1), mas bloqueiam publicar conteúdo definitivo nas Fases 2, 4 e 5.

---

## Ordem recomendada de execução
Fase 0 → Fase 1 → Fase 2 → Fase 3 → Fase 4 → Fase 5 → Fase 6 → Fase 7. Deploy (Fase 8) só quando o usuário pedir.

**Status em 2026-09-05**: Fases 0–7 têm núcleo funcional rodando localmente (site público + Portal Admin conversando com a API real, agenda com motor de recorrência completo, primeiro recorte de segurança/acessibilidade/SEO/testes/performance, configuração de Contribuições no banco, gestão de usuários administradores). Todas as pendências internas das Fases 0–7 estão fechadas — só restam os itens listados em "Fora de escopo desta passada" (Fase 7, deliberadamente adiados) e a trilha paralela de conteúdo abaixo, que continua bloqueando publicação definitiva. Próximo passo natural: Fase 8 (Deploy, quando o usuário decidir publicar) ou avançar na trilha paralela de conteúdo.

Cada fase segue a regra do `CLAUDE.md`: analisar o existente, resumir o que muda, preservar padrões definidos, evitar dependências desnecessárias, rodar build/testes e corrigir erros relevantes antes de avançar.
