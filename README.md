# Catálogo de cursos — SESI Indústria

> **Pendente:** o layout fiel ao Figma e a publicação. As seções marcadas com *(a completar)* dependem
> de uma das duas; o resto descreve o código como ele está.

## Objetivo da aplicação

Aplicação web para o Centro de Tecnologias e Educação Digital (CTED) apresentar o catálogo de cursos do
ambiente virtual de aprendizagem. O usuário consulta, pesquisa pelo nome, filtra por categoria,
modalidade e status (combináveis) e abre os detalhes de cada curso, com módulos e aulas.

- **Aplicação publicada:** *(a completar)*
- **Requisito original:** [`docs/DESAFIO.md`](docs/DESAFIO.md)

## Tecnologias utilizadas

| Camada | Tecnologia | Por quê |
|---|---|---|
| Back-end | C# / ASP.NET Core 10 (Minimal API) | Exigência do edital; Minimal API mantém três endpoints sem cerimônia |
| Acesso a dados | Entity Framework Core 10 + Npgsql | Migrations versionadas e seed no mesmo lugar |
| Banco | PostgreSQL 16 | Código aberto; há oferta gratuita gerenciada para a publicação |
| Front-end | Vue 3 + TypeScript + Vite, Vue Router | Componentes simples, tipagem dos contratos da API |
| Testes | xUnit + Testcontainers | Integração contra um PostgreSQL real, não contra mock |
| Execução | Docker / Docker Compose | Um comando sobe banco e aplicação |

## Arquitetura da solução

```
Navegador ──► ASP.NET Core (uma imagem, uma porta)
              ├── /            → front-end (build do Vite em wwwroot; rotas da SPA caem no index.html)
              ├── /api/*       → Minimal API ──► EF Core ──► PostgreSQL
              ├── /openapi/v1.json
              └── /health      → verifica a conexão com o banco
```

- **Front servido pela própria API:** um endereço só, sem CORS e um único serviço para publicar.
  Em desenvolvimento, o Vite repassa `/api` para a API (proxy).
- **Migrations ao iniciar:** a API aplica as migrations (schema e catálogo fictício) quando sobe, porque
  a hospedagem gratuita não dá terminal para rodar comandos à mão.
- **A URL como estado:** busca e filtros ficam na query string (`/?busca=matematica&modalidade=ead`).
  O link é compartilhável e o "voltar" do navegador desfaz o último filtro.

## Estrutura das pastas

```
├── src/CatalogoCursos.Api/
│   ├── Program.cs              composição: banco, migrations, rotas, front estático
│   ├── Cursos/                 endpoints e DTOs da API
│   ├── Dominio/                entidades, enums e rótulos, normalização de busca
│   └── Dados/                  DbContext, seed fictício, conexão, Migrations/
├── tests/CatalogoCursos.Api.Tests/   testes de integração (API real + PostgreSQL real)
├── frontend/
│   ├── public/assets/          imagens, ícones e logos do material do Figma
│   └── src/
│       ├── api/                cliente HTTP e tipos espelhando os DTOs
│       ├── composables/        useCatalogo: busca, filtros, estados, sincronia com a URL
│       ├── components/ views/  telas e componentes
│       └── estilos/            tokens, fonte Open Sans, base de acessibilidade
├── database/catalogo.sql       script SQL completo (schema + dados), gerado das migrations
├── docs/DESAFIO.md             requisito transcrito, com checklist
├── Dockerfile  docker-compose.yml  render.yaml
├── preparar.sh                 máquina nova pronta com um comando
├── gerar-zip-publicacao.sh     monta o ZIP dos arquivos de publicação
├── README.md  USO_DE_IA.md
```

## Instruções para execução

Requisitos: **Docker** (com Compose) para as opções 1 e 3; **.NET SDK 10** e **Node 24** para a opção 2.

**Opção 1 — tudo em Docker** (requer só Docker):

```bash
docker compose up -d --build
# http://localhost:8080
```

**Opção 2 — desenvolvimento** (requer .NET SDK 10 e Node 24):

```bash
docker compose up -d postgres                 # banco em 127.0.0.1:5470
dotnet run --project src/CatalogoCursos.Api   # API em http://localhost:5070
cd frontend && npm ci && npm run dev          # front em http://localhost:5173
```

**Opção 3 — máquina Linux/WSL do zero** (requer Docker e `curl`):

```bash
./preparar.sh   # instala .NET 10 e Node 24 só no usuário, baixa dependências, sobe o banco e roda os testes
```

O script pode rodar de novo sem efeito colateral: ele pula o que já está instalado.

**Testes:** `dotnet test` roda os 22 testes de integração (requer Docker: cada execução cria um
PostgreSQL descartável). No front, `npm run build` faz a checagem de tipos (`vue-tsc`) antes do build.

## Configuração do banco

A API lê a conexão de uma destas variáveis, nesta ordem:

| Variável | Formato | Uso |
|---|---|---|
| `ConnectionStrings__Catalogo` | `Host=...;Database=...;Username=...;Password=...` | Local e Docker Compose |
| `DATABASE_URL` | `postgresql://usuario:senha@host:5432/banco?sslmode=require` | Hospedagens (formato do Neon e do Render) |

Ao subir, a API aplica as migrations pendentes (`Banco__MigrarAoIniciar=false` desliga). Para criar o
banco sem a aplicação, use o script [`database/catalogo.sql`](database/catalogo.sql) (idempotente).

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/cursos` | Lista cursos. Query opcional: `busca`, `categoria`, `modalidade`, `status` (combináveis). Retorna `{ total, itens }` |
| GET | `/api/cursos/{slug}` | Detalhe do curso, com módulos e aulas. `404` se não existir |
| GET | `/api/filtros` | Valores válidos de categoria, modalidade e status (`valor` + `rotulo`) |
| GET | `/health` | `200` se o banco responde, `503` se não |
| GET | `/openapi/v1.json` | Documento OpenAPI |

Regras:
- A busca ignora acento, maiúscula e espaços nas pontas, e procura no nome completo e no nome breve.
- Valor de filtro desconhecido responde `400` (ProblemDetails) dizendo qual campo, em vez de uma lista
  vazia silenciosa.
- A ordem é inscrições abertas → em breve → encerrado e, dentro de cada status, por nome.

Exemplo: `GET /api/cursos?busca=matematica&modalidade=ead`

## Modelo de dados

```
categorias (id, slug único, nome, icone)
    1 ── N
cursos (id, slug único, nome_completo, nome_breve, descricao, categoria_id, modalidade, carga_horaria_horas,
        publico_alvo, status, imagem_url, data_disponibilizacao?, nome_busca)
    1 ── N
modulos (id, curso_id, ordem)          ── ordem única por curso
    1 ── N
aulas (id, modulo_id, ordem, titulo, descricao)   ── ordem única por módulo
```

- `modalidade` (`Ead`, `Presencial`, `Hibrido`) e `status` (`InscricoesAbertas`, `EmBreve`, `Encerrado`)
  são gravados como texto: o banco fica legível e reordenar o enum no código não corrompe dados.
- `data_disponibilizacao` só existe para curso "em breve" ("quando aplicável", como pede o edital).
- Catálogo fictício: 8 cursos, 4 categorias, 3 modalidades e 3 status, com cargas horárias, módulos,
  aulas e públicos-alvo diferentes.

## Estratégia de responsividade

*(a completar após o alinhamento com o Figma)* Base atual: mobile-first; os filtros passam de uma coluna
(celular) para duas (tablet) e para uma linha (≥ 1024 px); grade de cards com
`auto-fill/minmax(300px)`; tipografia fluida com `clamp()`; imagens com proporção fixa (`aspect-ratio`),
sem salto de layout.

## Decisões de acessibilidade

Implementado:
- HTML semântico: `header`/`main`/`footer`, `form role="search"`, lista de cursos em `ul`, ficha do curso
  em `dl`, módulos em `details/summary` (teclado nativo).
- Todo campo tem `label` visível; a contagem de resultados fica em `role="status"` e o leitor de tela a
  anuncia a cada busca ou filtro; erro em `role="alert"`.
- Link "Pular para o conteúdo"; ao trocar de página, o foco vai para o conteúdo principal e o título da
  aba muda.
- Card clicável inteiro, mas com nome acessível só no título (o link cobre o card por `::after`).
- Foco visível em tudo, alvos de toque com pelo menos 44 px e `prefers-reduced-motion` respeitado.
- Imagens decorativas com `alt=""`; links que abrem em nova aba avisam no texto.

**Auditoria.** [axe-core](https://github.com/dequelabs/axe-core) 4.11 (o mesmo motor do Lighthouse),
regras WCAG 2.0/2.1/2.2 A e AA mais boas práticas, rodado por Playwright em todas as telas e estados —
catálogo, busca, filtros combinados, lista vazia, filtro inválido, detalhe, curso inexistente e 404 — em
1280 px e 390 px. Complementado com o que o axe não mede: rolagem horizontal a 320 px (reflow, WCAG
1.4.10), tamanho de todo alvo clicável, ordem de tabulação e foco após trocar de página.
Achados e correções:
- Títulos pulavam de nível (`h1` da página → `h3` no card): o título do card passou a `h2`.
- Logo do topo e link da trilha tinham 19 px de altura clicável: passaram a 44 px sem mudar o visual.

Resultado: zero violações e nenhum alvo abaixo de 44 px.

## Processo de publicação

Dois serviços gratuitos, sem custo para o SESI-SP:

| Peça | Onde | Por quê |
|---|---|---|
| Aplicação (API + front) | Render, plano gratuito, deploy pelo `Dockerfile` | A mesma imagem do `docker compose` local: o que roda aqui é o que roda lá |
| Banco | Neon, PostgreSQL 16 gratuito, região AWS us-east-1 | O PostgreSQL gratuito do Render expira em 30 dias; o do Neon não |

A configuração do Render está versionada em [`render.yaml`](render.yaml) (Blueprint): serviço web na
Virgínia (mesma região do banco), checagem de saúde em `/health` e deploy automático a cada push na `main`.
A única configuração fora do repositório é a `DATABASE_URL`, que é segredo e entra pelo painel.

Passo a passo, partindo do zero:

1. **Neon:** criar o projeto (PostgreSQL 16, região *AWS US East 1*) e copiar a *connection string*
   (`postgresql://...?sslmode=require`).
2. **Render:** *New → Blueprint*, apontar para este repositório; o Render lê o `render.yaml` e pede a
   `DATABASE_URL`: colar a do Neon.
3. No primeiro boot, a aplicação aplica as migrations sozinha (schema + catálogo fictício). Não há passo
   manual no banco; quem preferir pode rodar [`database/catalogo.sql`](database/catalogo.sql), que é
   idempotente.
4. Conferir `https://<serviço>.onrender.com/health` → `{"status":"ok"}`.

**Arquivos de publicação (ZIP):** `./gerar-zip-publicacao.sh` gera
`publicacao/sesi-catalogo-cursos-publicacao.zip` com a aplicação compilada (`app/`: `dotnet publish` com
o build do front em `wwwroot`), o script SQL e os arquivos que montam a publicação (`Dockerfile`,
`.dockerignore`, `render.yaml`). A configuração de desenvolvimento fica de fora. Para rodar o pacote sem
Docker: `DATABASE_URL=... dotnet app/CatalogoCursos.Api.dll` (requer o runtime ASP.NET Core 10).

## Limitações conhecidas

- **Hospedagem gratuita "dorme" sem uso:** a primeira visita pode levar cerca de 50 segundos. O front
  espera até 60 s; se passar disso, mostra a mensagem de demora e o botão "Tentar novamente". O edital
  aceita indisponibilidade temporária porque há instruções para rodar localmente (acima).
- **Sem integração com o Moodle**, que o edital deixa como diferencial opcional. O caminho seria trocar a
  origem dos dados pela API de web services do Moodle (`core_course_get_courses`); o contrato da nossa API
  (`/api/cursos`) não precisaria mudar.
- **Sem paginação:** o catálogo fictício é pequeno e a lista volta inteira. Com centenas de cursos, a API
  ganharia `pagina` e `tamanho`, e o total já está separado dos itens para isso.
- **Testes automatizados só no back-end.** O front é verificado por compilação estrita (`vue-tsc`) e
  pela auditoria de acessibilidade descrita acima, não por testes de componente.
- **Somente leitura:** não há cadastro, edição nem exclusão de cursos, como o edital dispensa; os dados
  mudam por migration.
- **Diferenças em relação ao Figma:** *(a completar junto com o layout)*
