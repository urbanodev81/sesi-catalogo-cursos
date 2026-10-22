# Uso de Inteligência Artificial

> Registro feito durante o desenvolvimento, etapa por etapa. Falta a etapa do layout do Figma, que
> ainda não foi feita. As linhas marcadas com *(a completar)* entram junto com ela.

## Ferramenta utilizada

**Claude Code** (Anthropic, modelo Claude Opus 5.5), agente de linha de comando com acesso ao
repositório, ao terminal e ao Docker da máquina de desenvolvimento. A IA escreveu a maior parte do código
e dos textos. As decisões e a revisão foram minhas: o que entrava, o que voltava e o que era aceito.

## Objetivo do uso

| Etapa | Uso da IA | Decisão humana |
|---|---|---|
| Leitura do edital | Ler o PDF, o e-mail e o pacote de arquivos; transcrever tudo para `docs/DESAFIO.md`, com um checklist | Conferir a transcrição contra o original |
| Arquitetura | Avaliar o edital e propor a stack e a hospedagem gratuita | Aceitar a proposta (.NET 10 + PostgreSQL + Vue 3, front servido pela API, Render + Neon) e pedir para começar |
| Back-end | Gerar entidades, endpoints, seed fictício e testes de integração | Revisar as regras: filtro inválido = 400, ordenação por status, busca sem acento |
| Front-end | Gerar cliente da API, estado de busca/filtros e telas base | Layout provisório até ter as telas do Figma; decidir mandar prints do protótipo, que a IA não consegue abrir. *(a completar na etapa do Figma)* |
| Ambiente | Escrever `preparar.sh`, que deixa uma máquina sem nada pronta com um comando | Usá-lo para continuar o trabalho em outra máquina |
| Otimização de assets | Converter PNG (3–4 MB) em WebP de 1200 px e TTF em woff2 com subset Latin, sem dependência nova no projeto (scripts avulsos via `uvx`) | Largura pelo maior uso real (detalhe, 1200 px); conferir visualmente a qualidade e que os eixos `wght`/`wdth` e os acentos sobreviveram ao subset |
| Auditoria de acessibilidade | Rodar axe-core por Playwright em todas as telas/estados (desktop e mobile) e medir o que o axe não cobre: reflow a 320 px, alvos de toque, ordem de Tab, foco após navegar. Ferramenta em pasta temporária, fora das dependências do projeto | Aceitar as duas correções (nível de título, alvo de 44 px) e conferir no print que o visual não mudou |
| Documentação | Redigir o README e este arquivo a partir do código e do histórico das sessões | Revisar o texto e cortar o que eu não saberia explicar na entrevista |

## Prompts relevantes

Os prompts foram curtos e em linguagem natural; o contexto vinha dos arquivos do edital e do próprio
repositório. Transcritos com a grafia corrigida e o sentido intacto:

1. **"RECEBI ISSO, AVALIE"**, com o PDF de instruções, o e-mail e o `.rar` de arquivos anexados.
   A IA resumiu o pedido em tabela, apontou o que pesaria na avaliação (fidelidade ao Figma,
   acessibilidade, os três estados, `USO_DE_IA.md` honesto) e recomendou a stack.
   **Aproveitado:** a stack e a hospedagem (Render + Neon), e a ideia de a API servir o front: um
   endereço só, sem CORS, um único Dockerfile.
   **Descartado:** o nome e o lugar sugeridos para o projeto (`~/dev/senai-catalogo`). O projeto ficou
   em `sesi-catalogo-cursos`, ao lado do outro teste técnico em andamento.
2. **"comece na nossa maquina, crie uma pasta proximo da pasta do outro teste que estamos, comece o
   projeto de montar"**. A IA montou o back-end, o banco, o seed, os testes, o front provisório e o
   Docker, e fez o primeiro commit.
   **Aproveitado:** semear os dados pela migration (`HasData`) em vez de um seeder em tempo de execução.
   Assim o script SQL exportado já sai com o catálogo, e o entregável "scripts, migrations ou arquivo do
   banco" fica num arquivo só.
   **Aproveitado:** busca contra uma coluna normalizada (`NomeBusca`) em vez da extensão `unaccent` do
   PostgreSQL. Funciona em qualquer Postgres gerenciado gratuito, sem permissão de criar extensão.
   **Descartado:** os textos dos cursos, que a IA inventou porque não tinha acesso ao Figma. Eles serão
   trocados pelos dados fictícios do protótipo *(a completar)*.
3. **Escolha "Mando prints das telas"**, numa pergunta da IA sobre como seguir com o visual. O link do
   Figma é de protótipo e não abre fora do navegador, então a IA não montou telas "no escuro". Ela
   esperou os prints, porque o edital cobra fidelidade e justificativa para cada diferença.
4. **"Não consegue baixar e preparar tudo para mim?"**, na troca de máquina. Virou o `preparar.sh`.
5. **Escolha das quatro frentes finais** (publicar, README/USO_DE_IA, auditoria de acessibilidade,
   layout do Figma). A auditoria veio antes do layout de propósito: o que ela corrigiu (estrutura de
   títulos, área clicável) continua valendo com qualquer visual. Ela será rodada de novo depois.

## Como o conteúdo foi validado

- **Testes de integração** (`tests/`): 22 testes contra um PostgreSQL real criado na hora
  (Testcontainers). Cobrem o mínimo exigido pelo edital (6 cursos, 3 categorias, 2 modalidades,
  3 status), a busca, os filtros combinados, as respostas 400/404 e o detalhe com módulos.
- **Compilação estrita**: `vue-tsc` no front e `dotnet build` no back.
- **Execução real** com `docker compose`: cada rota conferida por HTTP (SPA, API, assets, OpenAPI),
  com leitura dos logs do contêiner.
- **Auditoria de acessibilidade** com axe-core e medições complementares (ver README, "Decisões de
  acessibilidade").
- **Conferência contra o material original**: dimensões dos SVGs do pacote, qualidade visual das
  imagens convertidas, acentos da fonte após o subset.

## Exemplos de erro ou inadequação identificados — e a correção

1. **Sintaxe TypeScript incompatível com a configuração do projeto.** A IA escreveu a classe de erro
   da API com *parameter properties* (`constructor(readonly naoEncontrado = false)`). O template do Vite
   liga `erasableSyntaxOnly`, que proíbe essa sintaxe, e o build quebrou (`TS1294`).
   **Correção:** campo declarado explicitamente e atribuído no construtor.
2. **Nome de arquivo presumido.** O Dockerfile gerado copiava `CatalogoCursos.sln`, mas o .NET 10 cria
   a solução no formato novo `.slnx`; o build da imagem falhou com "not found".
   **Correção:** conferido o arquivo real e ajustado o `COPY`.
3. **Erro silencioso no log do contêiner.** A aplicação respondia normalmente, mas o log mostrava
   `libgssapi_krb5.so.2: cannot open shared object file` a cada conexão: o driver Npgsql tenta
   autenticação Kerberos, que a imagem `aspnet` não traz. Só apareceu porque a validação incluiu ler o
   log, e não apenas testar as rotas.
   **Correção:** `GssEncryptionMode=Disable` na montagem da conexão (`Dados/ConexaoBanco.cs`).
4. **Logo com proporção inventada.** A IA pôs o logo do SESI Indústria em 160 × 40 no topo e no
   rodapé, sem olhar o arquivo. O SVG do pacote mede 188 × 19, e o logo saía achatado.
   **Correção:** dimensões tiradas do próprio SVG (commit `734e476`).
5. **Acessibilidade que só a auditoria pegou.** O código gerado pela IA já tinha rótulos, `role=status`
   e foco visível, mas o axe e as medições acharam dois problemas: os títulos pulavam de nível (`h1` da
   página → `h3` no card), e o logo e o link da trilha tinham só 19 px de altura clicável, abaixo dos
   44 px. **Correção:** título do card como `h2` e área clicável de 44 px sem mudar o visual
   (commit `215ffed`).
