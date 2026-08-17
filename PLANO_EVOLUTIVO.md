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

## Fase 2 — Site público com mock data
Objetivo: todas as páginas públicas navegáveis, com dados mockados no Angular (sem API ainda).

- Páginas: Início, Quem Somos, Ministérios, Agenda/Eventos, Conecte-se/Contato, Contribuições.
- Home com Hero (foto candidata: letreiro + comunidade), 3 próximos eventos mockados, ministérios em destaque.
- Página de Contribuições com chave PIX fixa e botão "Copiar chave PIX" funcional.
- Aplicar Design System, acessibilidade básica (semântica, foco visível, alt text) e responsividade.

## Fase 3 — Modelagem de dados e API real
Objetivo: schema definitivo e endpoints reais substituindo os mocks.

- Modelar entidades: `Local`, `Ministerio`, `SerieEvento`, `OcorrenciaEvento` (calculada), `ExcecaoEvento`, `Usuario`, `ConteudoSite`, `Contribuicao` (config), conforme os modelos conceituais do `CLAUDE.md`.
- Migrations (EF Core) para PostgreSQL.
- Endpoints CRUD básicos para Locais, Ministérios e Conteúdo institucional.
- Regras de visibilidade (PUBLICO/INTERNO/PESSOAL) e status (RASCUNHO/PUBLICADO/CANCELADO) aplicadas **na API**, nunca só no Angular.

## Fase 4 — Integração site público ↔ API
Objetivo: site público consumindo dados reais.

- Home, Quem Somos, Ministérios e Contato passam a ler da API (textos editáveis, ministérios cadastrados).
- Agenda pública ainda simplificada (sem recorrência completa): lista eventos únicos PUBLICO+PUBLICADO por data.
- Tratamento de loading/erro e cache leve no frontend.

## Fase 5 — Portal Administrativo (núcleo)
Objetivo: pastores/secretária conseguem logar e gerenciar conteúdo básico.

- Autenticação (login seguro, hash de senha, sem cadastro público de admin).
- Perfil ADMIN amplo no MVP, mas modelo de dados já preparado para PASTOR/SECRETARIA/LIDER_MINISTERIO/EDITOR_CONTEUDO no futuro.
- Dashboard simples.
- Telas: Conteúdo do Site, Ministérios, Locais/Templos, Fotos (upload validado/limitado), Usuários (gestão restrita), Configurações.
- Autorização por rota na API (não confiar no guard do Angular sozinho).

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
Fase 0 → Fase 1 → Fase 2 (com trilha paralela em curso) → Fase 3 → Fase 4 → Fase 5 → Fase 6 → Fase 7. Deploy (Fase 8) só quando o usuário pedir.

Cada fase segue a regra do `CLAUDE.md`: analisar o existente, resumir o que muda, preservar padrões definidos, evitar dependências desnecessárias, rodar build/testes e corrigir erros relevantes antes de avançar.
