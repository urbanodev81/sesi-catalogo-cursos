# CLAUDE.md — Catálogo de cursos SESI Indústria (teste técnico)

Teste prático do processo seletivo SESI-SP 519/2026 (Desenvolvedor de Sistemas I). Prazo: **26/10/2026, 8h**.
`docs/DESAFIO.md` é a fonte da verdade do requisito (com checklist do que já está coberto).
O Figma é a referência visual; diferenças precisam ser listadas e justificadas no README.

Repositório isolado: **não** segue o AFTSCD/hooks do ecossistema Urbano Dev. Fluxo: etapa → testes →
commit (Conventional Commits, em português) → parar para revisão do Ricardo.

O Ricardo apresenta este código na entrevista: **cada decisão precisa ser explicável por ele.** Nada de
dependência ou padrão que não se justifique em uma frase. Todo uso de IA relevante vai para `USO_DE_IA.md`
(inclusive erros da IA pegos por build/teste — o edital pede exemplos).

## Comandos

.NET 10 fica em `~/.dotnet` (fora do PATH) e o Node Linux em `~/.local/node/bin` (o `npm` do PATH pode
ser o do Windows e quebrar):

```bash
./preparar.sh                                          # máquina nova: instala .NET 10/Node 24, deps, banco, testes
export DOTNET_ROOT=~/.dotnet PATH=~/.dotnet:~/.local/node/bin:$PATH
docker compose up -d postgres                         # banco local em 127.0.0.1:5470
dotnet run --project src/CatalogoCursos.Api           # API em http://localhost:5070 (aplica migrations)
(cd frontend && npm run dev)                          # front em http://localhost:5173, /api via proxy
dotnet test                                           # 51 testes; sobem Postgres próprio (Testcontainers)
docker compose up -d --build                          # tudo junto em http://localhost:8080
dotnet ef migrations add <Nome> -p src/CatalogoCursos.Api -o Dados/Migrations
dotnet ef migrations script --idempotent -p src/CatalogoCursos.Api -o database/catalogo.sql
```

## Convenções

- Dados fictícios em `Dados/DadosIniciais.cs`, semeados por `HasData` (vão na migration e no SQL).
  Mudou o seed → nova migration → regerar `database/catalogo.sql`.
- Enum trafega como `valor` (kebab, na URL e no JSON) + `rotulo` (tela); a tabela é `Dominio/Rotulos`.
- Busca compara contra `NomeBusca` (minúsculo, sem acento, gravado no seed) — sem extensão de banco.
- Filtro com valor desconhecido = 400 com o campo; nunca "lista vazia" silenciosa.
- No front, a URL é a fonte da verdade de busca/filtros (`useCatalogo`); requisição anterior é abortada.
- Acessibilidade não é polimento: rótulo em todo campo, contagem em `role=status`, foco visível,
  foco no `<main>` ao trocar de página, alvo de toque ≥ 44px, `prefers-reduced-motion`.

## Estado (23/10/2026)

- Back-end completo: API, banco, seed com os cursos do Figma (12), migrations, script SQL, 51 testes verdes.
- Front fiel ao Figma: home (`/`), listagem (`/cursos`, montada com peças do Figma — o protótipo não a tem),
  curso (`/cursos/:slug`) e aula (`/cursos/:slug/modulos/:m/aulas/:a`, sem casca, `meta.semCasca`).
  Progresso das aulas no localStorage atrás de `progresso/RepositorioProgresso` (trocar pela API se o login entrar).
  Texto institucional em `frontend/src/conteudo/institucional.ts`; ícones de traço em `IconeLinha.vue`.
- Assets otimizados (WebP 1200 px, Open Sans woff2 Latin). Auditoria a11y refeita em 23/10 (14 telas × 1440/390,
  axe zerado, reflow 320 px, ordem de Tab). README (com "diferenças em relação ao Figma") e USO_DE_IA completos.
- Publicação preparada: `render.yaml` (Blueprint, Docker, free, Virgínia) + Neon us-east-1 via `DATABASE_URL`;
  `./gerar-zip-publicacao.sh` gera o ZIP do entregável 8.
- Login de demonstração (aluno e professor) na `main` desde 23/10: cookie HttpOnly, PasswordHasher, 5 logins/min
  por IP, contas do seed com senha `sesi2026` (`Dados/ContasDeDemonstracao.cs`).
- Publicado em 23/10: https://sesi-catalogo-cursos-yict.onrender.com (Render, Blueprint, conta GitHub urbanodev81;
  Neon us-east-1, string sem pooler). Deploy automático a cada push na `main`.
- Barra de carregamento entre páginas (`ProgressoNavegacao`, 180 ms de carência) e "voltar ao topo"
  (`BotaoTopo`, rolagem animada por rAF mesmo com reduce — decisão do Ricardo) desde 23/10. Painel de
  acessibilidade + VLibras em `frontend/src/acessibilidade/` (storage `sesi-a11y-`). Modo escuro e redes
  flutuantes recusados (USO_DE_IA nº 7). Seta da aula volta para a área do aluno quando há login.
- Painel do aluno (23/10, pedido do Ricardo, fora do Figma): próximas avaliações, documentos com upload
  (bytea, 2 MB, tipo pelos bytes), colegas com "online" (POST `/api/conta/presenca` a cada minuto, janela
  de 2 min) e `/area-do-aluno/perfil`. Endpoints em `Contas/AlunoEndpoints.cs`; `TimeProvider` injetável
  (testes fixam 24/10/2026 em `ApiFactory.Relogio`). Migration `AreaDoAluno`.
- Área do professor (23/10): `/area-do-professor` (mesma `AreaAlunoView`, endereço acertado pelo papel),
  `components/professor/PainelProfessor.vue` com turmas e alunos em `<details>` (`.acordeao >` filhos
  diretos), horários (`horarios_turma`) e materiais (`materiais`, 10 por turma) em `Contas/ProfessorEndpoints.cs`.
  Regras de arquivo comuns em `Contas/Arquivos.cs`. CSP em `Seguranca.cs` (medida com o VLibras aberto).
- Cabeçalho: "Entrar" (visitante) / "Meu painel" (com sessão). Botão "Ajuda" (`AjudaChat.vue`): respostas
  prontas de `AJUDA_PLATAFORMA` + `DUVIDAS_FREQUENTES`, campo de IA desligado com "Em breve".
- Antes de push (publica no Render), subir local e o Ricardo olhar.
- Falta: repo público antes da entrega (hoje privado), depois da revisão final do Ricardo.
