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
- [ ] `Contribuicao` (config) — não modelada ainda; a página de Contribuições continua com a chave PIX fixa no frontend.
- [x] Migrations aplicadas (`PortalAdminNucleo`, `ConteudoTexto`, `Ministerios`).
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
- [x] Telas implementadas: **Agenda** (CRUD completo de eventos), **Locais/Templos** (CRUD), **Ministérios** (CRUD), **Conteúdo do Site** (textos institucionais editáveis + upload de imagens por chave, S3-compatível via MinIO em dev).
- [ ] Telas **Usuários** e **Configurações** — não implementadas ainda; hoje só existe o ADMIN semeado, sem tela para criar/gerenciar outros usuários pelo Portal.
- [x] Autorização checada na API (`[Authorize(Roles = "ADMIN")]` em todo `/api/admin/**`), nunca só no guard do Angular — testado explicitamente (sem token → 401).

## Fase 6 — Agenda recorrente + exceções (parte mais complexa)
Objetivo: motor de recorrência completo, conforme especificado no `CLAUDE.md`.

- Implementar tipos de recorrência: sem repetição, semanal, a cada N semanas, mensal por dia, mensal por posição, anual.
- Cálculo de ocorrências por **janela de consulta** (nunca gerar fisicamente anos de eventos).
- CRUD de `SerieEvento` e `ExcecaoEvento`; exceção sempre prevalece sobre a regra.
- Edição com 3 escopos: "somente esta ocorrência" / "esta e as próximas" (encerra série antiga, cria nova) / "toda a série".
- Prévia das próximas datas antes de confirmar recálculo de série.
- Portal: tela de Agenda completa com esses fluxos.
- Site público: páginas por categoria mostram automaticamente a próxima ocorrência válida (Home = 3 próximos eventos públicos); eventos passados somem sozinhos.
- Validar com dados reais de exemplo do `CLAUDE.md` (Ceia 1º domingo 08:30 no Templo Liberdade; Culto de Louvor domingos 18:00 exceto 1º domingo; Jovens a cada 2 semanas aos sábados).

## Fase 7 — Segurança, acessibilidade, SEO, testes e performance
- HTTPS em dev/homolog, revisão de CORS, rate limiting nos endpoints sensíveis (login, formulários públicos), logs sem dados sensíveis.
- Auditoria de acessibilidade (teclado, contraste, `prefers-reduced-motion`, labels/alt).
- SEO: meta tags, Open Graph, sitemap.xml, robots.txt, Schema.org (Organization/Event).
- Otimização de imagens (WebP/AVIF, lazy loading), code splitting, Core Web Vitals.
- Testes: unitários (Angular + .NET), pelo menos testes de integração para o motor de recorrência (é a parte de maior risco de bugs).

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

**Status em 2026-09-04**: Fases 0–5 têm núcleo funcional rodando localmente (site público + Portal Admin conversando com a API real). Pendências que restaram dentro dessas fases: telas de Usuários/Configurações (Fase 5), entidade `Contribuicao` (Fase 3), e a trilha paralela de conteúdo abaixo continua bloqueando publicação definitiva. Próximo passo natural: Fase 6 (motor de recorrência completo) ou fechar as pendências de Usuários/Configurações — a decidir com o usuário.

Cada fase segue a regra do `CLAUDE.md`: analisar o existente, resumir o que muda, preservar padrões definidos, evitar dependências desnecessárias, rodar build/testes e corrigir erros relevantes antes de avançar.
