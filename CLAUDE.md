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
dotnet test                                           # 22 testes; sobem Postgres próprio (Testcontainers)
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

## Estado (21/10/2026)

- Back-end completo para o requisito: API, banco, seed, migrations, script SQL, 22 testes verdes.
- Front funcional (catálogo, filtros, detalhe, estados) com **layout provisório** — aguardando as telas
  do Figma (o protótipo não abre por fetch; o Ricardo exporta os frames em PNG).
- Falta: layout fiel ao Figma, otimizar imagens (PNG de 3–4 MB) e fonte (TTF → woff2), publicação
  (Render + Neon, gratuitos), ZIP de publicação, README e USO_DE_IA finais, auditoria de acessibilidade.
