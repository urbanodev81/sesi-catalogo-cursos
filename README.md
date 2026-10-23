# Catálogo de cursos — SESI Indústria

> **Pendente:** a publicação (URL abaixo). O resto descreve o código como ele está.

## Objetivo da aplicação

Aplicação web para o Centro de Tecnologias e Educação Digital (CTED) apresentar o catálogo de cursos do
ambiente virtual de aprendizagem. O usuário consulta, pesquisa pelo nome, filtra por categoria,
modalidade e status (combináveis) e abre os detalhes de cada curso, com módulos e aulas. Além do
requisito, a tela de aula do protótipo funciona: conteúdo de cada aula, navegação entre aulas e progresso
guardado no navegador.

Páginas: `/` (home), `/cursos` (listagem com busca e filtros), `/cursos/{slug}` (curso),
`/cursos/{slug}/modulos/{m}/aulas/{a}` (aula), `/area-do-aluno` (login de demonstração).

**Login de demonstração** (também fora do requisito): senha **`sesi2026`** para todas as contas.

| Conta | Papel | O que vê |
|---|---|---|
| `aluno@exemplo.com` (Ana) | Aluno | "Meus cursos": já cursa os dois cursos em andamento, com progresso; o progresso das aulas fica no banco |
| `bruno@exemplo.com`, `carla@exemplo.com` | Aluno | Outros progressos, para o painel do professor ter o que mostrar |
| `professor@exemplo.com` (Marina) | Professor | Progresso de cada aluno, por curso, só leitura |

Sem entrar, tudo funciona igual e o progresso das aulas fica no navegador.

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
- **A URL como estado:** busca e filtros ficam na query string (`/cursos?busca=matematica&modalidade=ead`).
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
│       ├── composables/        useCatalogo (busca, filtros, estados, URL) e useCarrossel
│       ├── conteudo/           texto institucional do Figma (não é dado do catálogo)
│       ├── progresso/          progresso nas aulas, atrás de uma interface (hoje no navegador)
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

**Testes:** `dotnet test` roda os 38 testes de integração (requer Docker: cada execução cria um
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
| GET | `/api/cursos/{slug}/modulos/{modulo}/aulas/{aula}` | Conteúdo da aula (endereçada pela ordem, como na tela), com a anterior e a próxima do curso inteiro. `404` se não existir |
| GET | `/api/filtros` | Valores válidos de categoria, modalidade e status (`valor` + `rotulo`) |
| POST | `/api/conta/entrar` | `{ email, senha }` → abre a sessão (cookie). `401` com a mesma mensagem para e-mail ou senha errados; `429` após 5 tentativas no minuto |
| POST | `/api/conta/sair` | Encerra a sessão |
| GET | `/api/conta` | Quem está logado; `401` se ninguém |
| GET | `/api/conta/cursos` | *Aluno:* cursos matriculados, com aulas concluídas e a próxima aula |
| GET | `/api/cursos/{slug}/progresso` | *Aluno:* aulas concluídas neste curso (`"1.2"`) e se está matriculado |
| POST | `/api/cursos/{slug}/matricula` | *Aluno:* matricula (idempotente). `409` se as inscrições não estão abertas |
| POST | `/api/cursos/{slug}/progresso` | *Aluno:* `{ modulo, aula }` → marca como concluída (idempotente). `409` sem matrícula |
| GET | `/api/professor/turmas` | *Professor:* por curso, cada aluno com aulas concluídas e última atividade |
| GET | `/health` | `200` se o banco responde, `503` se não |
| GET | `/openapi/v1.json` | Documento OpenAPI |

Regras:
- A busca ignora acento, maiúscula e espaços nas pontas, e procura no nome completo e no nome breve.
- Rotas de aluno e de professor respondem `401` sem login e `403` com o papel errado (nunca redirecionam:
  quem decide o que mostrar é o front).
- Valor de filtro desconhecido responde `400` (ProblemDetails) dizendo qual campo, em vez de uma lista
  vazia silenciosa.
- A ordem é inscrições abertas → em breve → encerrado e, dentro de cada status, por nome.

Exemplo: `GET /api/cursos?busca=matematica&modalidade=ead`

## Modelo de dados

```
categorias (id, slug único, nome, icone)
    1 ── N
cursos (id, slug único, nome_completo, nome_breve, resumo, descricao, categoria_id, modalidade, carga_horaria_horas,
        publico_alvo, status, imagem_url, data_disponibilizacao?, nome_busca)
    1 ── N
modulos (id, curso_id, ordem)          ── ordem única por curso
    1 ── N
aulas (id, modulo_id, ordem, titulo, descricao, conteudo)   ── ordem única por módulo

usuarios (id, email único, nome, senha_hash, papel)
matriculas (usuario_id, curso_id, em)            ── chave: (usuario, curso)
aulas_concluidas (usuario_id, aula_id, em)       ── chave: (usuario, aula)
```

- `modalidade` (`Ead`, `Presencial`, `Hibrido`) e `status` (`InscricoesAbertas`, `EmBreve`, `Encerrado`)
  são gravados como texto: o banco fica legível e reordenar o enum no código não corrompe dados.
- `resumo` é o texto curto do card; `descricao` é o "Sobre o curso" da página de detalhe. O Figma mostra
  os dois, com tamanhos diferentes.
- `data_disponibilizacao` é o "Início do curso" do Figma; fica vazia em curso encerrado ("quando
  aplicável", como pede o edital).
- `conteudo` da aula: parágrafos separados por linha em branco, e linha com `## ` vira subtítulo. É texto
  puro, renderizado pelo front sem `v-html`, para que nenhum conteúdo vire HTML executável.
- Catálogo fictício: 12 cursos, 4 categorias, 3 modalidades e 3 status, com cargas horárias, módulos,
  aulas e públicos-alvo diferentes. Os seis cursos do Figma (nome, categoria e resumo dos cards) estão
  todos lá; os outros seis cobrem Linguagens e Sustentabilidade, que o pacote de ícones prevê.
- **Progresso sem percentual gravado:** cada aula concluída é uma linha. O percentual é calculado na hora
  (concluídas ÷ aulas do curso), então nunca fica desatualizado se o curso ganhar uma aula.
- **Senhas:** só o hash do `PasswordHasher` do ASP.NET Core (PBKDF2 com sal). Os hashes das contas de
  demonstração foram gerados uma vez e gravados no seed, porque o `HasData` precisa de valor fixo.
- **Troca de seed com índice único:** a migration `CatalogoDoFigma` remove e recria os índices de ordem
  dentro da própria transação, porque o EF atualiza o seed linha a linha e, no meio do caminho, duas aulas
  ocupariam a mesma posição.

## Estratégia de responsividade

O protótipo só tem telas de desktop (1440 px); as versões menores foram desenhadas aqui com as mesmas
peças, sem criar componente visual novo.

- **Mobile-first em CSS puro**, sem framework: uma folha (`estilos/base.css`) com tokens de cor e espaço.
  Os pontos de quebra são onde o conteúdo pede, não por aparelho: 640 px (filtros em duas colunas),
  900 px (hero e "Sobre o curso" em duas colunas, barra lateral da aula fixa) e 1024 px (filtros numa
  linha só).
- **Grades que se ajustam sozinhas:** cards da listagem com `repeat(auto-fill, minmax(272px, 1fr))` (uma
  coluna no celular, quatro em 1440 px); diferenciais com `auto-fit`.
- **Tipografia fluida com `clamp()`:** o título do hero vai de 40 px a 80 px sem degraus.
- **Carrosséis com `scroll-snap`:** no toque, o dedo arrasta; no desktop, as setas do Figma rolam um card
  por vez. Sem biblioteca.
- **Telas que mudam de forma:** no celular, o menu vira um botão "Menu"; a barra lateral da aula vira um
  painel aberto por "Conteúdo do curso"; a barra de matrícula mostra o nome breve do curso.
- **Imagens com `aspect-ratio`** e `width`/`height` no HTML: o layout não pula quando elas carregam.
- Verificado em 1440, 390 e 320 px (reflow da WCAG 1.4.10: nenhuma página rola na horizontal).

## Decisões de acessibilidade

Implementado:
- HTML semântico: `header`/`main`/`footer`, `form role="search"`, lista de cursos em `ul`, ficha do curso
  em `dl`, acordeão em `details/summary` (abre por teclado e leitor de tela sem JavaScript).
- Hierarquia de títulos sem saltos em todas as páginas (o card é `h3` no carrossel da home e `h2` na
  listagem, pela posição na página).
- Tela de aula: progresso em `role="progressbar"` com texto ("11% concluído, 1 de 9 aulas"), aula atual
  com `aria-current="page"`, aula concluída anunciada como "(concluída)", módulos com `aria-expanded`.
- Botão de play e seta de voltar só com ícone têm nome acessível em texto oculto.
- Login: campos com `autocomplete` (`username`, `current-password`) para o gerenciador de senhas, erro em
  `role="alert"` e foco no conteúdo depois de entrar ou sair. A tabela do professor tem `caption` e
  cabeçalhos de linha e coluna, e no celular rola dentro da própria caixa, sem alargar a página.
- Todo campo tem `label` visível; a contagem de resultados fica em `role="status"` e o leitor de tela a
  anuncia a cada busca ou filtro; erro em `role="alert"`.
- Link "Pular para o conteúdo"; ao trocar de página, o foco vai para o conteúdo principal e o título da
  aba muda.
- Card clicável inteiro, mas com nome acessível só no título (o link cobre o card por `::after`).
- Foco visível em tudo, alvos de toque com pelo menos 44 px e `prefers-reduced-motion` respeitado.
- Imagens decorativas com `alt=""`; links que abrem em nova aba avisam no texto.

**Auditoria.** [axe-core](https://github.com/dequelabs/axe-core) 4.11 (o mesmo motor do Lighthouse),
regras WCAG 2.0/2.1/2.2 A e AA mais boas práticas, rodado por Playwright em 14 telas e estados — home,
listagem, busca, filtros combinados, lista vazia, filtro inválido, curso (aberto, em breve, encerrado e
inexistente, com os acordeões abertos), aula, aula inexistente, área do aluno e 404 — em 1440 px e 390 px. Complementado com o que o axe não mede: rolagem horizontal a 320 px (reflow, WCAG
1.4.10), tamanho de todo alvo clicável, ordem de tabulação e foco após trocar de página.
Achados e correções:
- Títulos pulavam de nível (`h1` da página → `h3` no card): o título do card passou a `h2`.
- Logo do topo e link da trilha tinham 19 px de altura clicável: passaram a 44 px sem mudar o visual.
- Abrir uma página direto pelo link (não a home) já levava o foco ao conteúdo, e o "Pular para o
  conteúdo" ficava para trás: a primeira resolução da rota contava como troca de página. Corrigido.

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
- **Login só de demonstração:** não há cadastro, troca de senha nem recuperação; as contas vêm do seed.
  O professor vê todos os cursos (não há vínculo professor ↔ turma).
- **Sessão perdida quando a hospedagem reinicia:** as chaves que assinam o cookie ficam na memória do
  contêiner, e o plano gratuito reinicia o serviço quando ele "dorme". Basta entrar de novo. A correção
  seria guardar as chaves no banco (`PersistKeysToDbContext`).
- **Testes automatizados só no back-end.** O front é verificado por compilação estrita (`vue-tsc`) e
  pela auditoria de acessibilidade descrita acima, não por testes de componente.
- **Somente leitura:** não há cadastro, edição nem exclusão de cursos, como o edital dispensa; os dados
  mudam por migration.
- **Diferenças em relação ao Figma:**
  - **Listagem de cursos montada com peças do protótipo.** O Figma não tem a página com busca e filtros
    (só "Ver todos os cursos" apontando para ela). Como o edital pede para não criar design novo, ela usa
    o hero da home em versão curta, os cards de "Principais cursos" e o campo de busca em pílula da tela
    de aula. O card da listagem ganha uma linha com modalidade, carga horária e status, porque são os
    filtros da página e o usuário precisa ver por que um curso apareceu.
  - **Página do curso com mais informações no hero.** O protótipo mostra só "Início do curso" e
    "Certificação"; entraram modalidade, carga horária, status e público-alvo, que o edital lista como
    informações do curso. Os ícones dessas linhas não vêm no pacote e foram desenhados no mesmo traço.
    A trilha "Cursos / nome breve" também é nossa, para quem chega por link direto.
  - **"Matricule-se" e o play levam à primeira aula.** Não há login nem Moodle; esse é o fluxo do
    protótipo (curso → aula). Curso em breve ou encerrado não mostra o botão nem a barra fixa, e sim um
    aviso (o encerrado aponta para os cursos com inscrições abertas).
  - **Textos do acordeão escritos aqui.** O protótipo só mostra os títulos fechados de "Módulos", "Aulas
    online e avaliações" e "Dúvidas frequentes". Módulos vêm da API; os outros dois são texto genérico.
  - **Tela de aula:** o status "atual" vale para a aula aberta, e o progresso conta as aulas concluídas
    pelo botão "Concluir". O banner é o mesmo em todas as aulas (o pacote traz um só). Acrescentamos o
    botão de concluir e os links para a aula anterior e a próxima, que o protótipo não mostra.
  - **Fotos repetidas.** O pacote traz só três fotos (hero, banner do AVA e capa do vídeo), então os
    cards reaproveitam essas três. Cada card do protótipo tem uma foto própria.
  - **Sem botão de play na chamada final da home.** O protótipo mostra um vídeo, mas nenhum vídeo foi
    fornecido, e ali não há aula para onde levar. Um play que não toca nada seria uma promessa falsa.
  - **Rodapé só com links que levam a algum lugar.** Blog, Termos de Uso, Política de Privacidade e
    Trabalhe Conosco não têm página e saíram. "Certificação" leva aos diferenciais da home, e "Fale
    conosco" e "Central de ajuda" abrem um e-mail. Os "Cursos populares" do protótipo (Excel, Gestão do
    Tempo etc.) não existem no catálogo, então a coluna lista os seis cursos em destaque.
  - **"Área do aluno"** leva a uma página provisória, porque o login não faz parte do requisito.
  - **Cores das categorias Linguagens e Sustentabilidade.** O protótipo só mostra as etiquetas de
    Matemática (bege) e Ciências da Natureza (lilás). As outras duas seguem a mesma lógica, em tom claro
    de azul e de verde.
  - **Quinto depoimento completado.** O print corta o card; o fim do texto e o sobrenome foram escritos
    no mesmo tom dos outros.
  - **Versões para celular desenhadas aqui.** O protótipo só tem desktop (ver "Estratégia de
    responsividade").
