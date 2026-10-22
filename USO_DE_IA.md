# Uso de Inteligência Artificial

> **Rascunho em construção.** Registro feito durante o desenvolvimento, etapa por etapa. Antes da
> entrega, revisar e completar com a voz do autor.

## Ferramenta utilizada

**Claude Code** (Anthropic, modelo Claude Opus 5.5), agente de linha de comando com acesso ao
repositório, ao terminal e ao Docker da máquina de desenvolvimento.

## Objetivo do uso

| Etapa | Uso da IA | Decisão humana |
|---|---|---|
| Leitura do edital | Transcrever PDF e e-mail para `docs/DESAFIO.md` com checklist | Conferir a transcrição contra o original |
| Arquitetura | Propor stack e hospedagem gratuita | Escolha de .NET 10 + PostgreSQL + Vue 3, front servido pela API |
| Back-end | Gerar entidades, endpoints, seed fictício e testes de integração | Revisar regras: filtro inválido = 400, ordenação por status, busca sem acento |
| Front-end | Gerar cliente da API, estado de busca/filtros e telas base | *(a completar na etapa do Figma)* |
| Otimização de assets | Converter PNG (3–4 MB) em WebP de 1200 px e TTF em woff2 com subset Latin, sem dependência nova no projeto (scripts avulsos via `uvx`) | Largura pelo maior uso real (detalhe, 1200 px); conferir visualmente a qualidade e que os eixos `wght`/`wdth` e os acentos sobreviveram ao subset |
| Auditoria de acessibilidade | Rodar axe-core por Playwright em todas as telas/estados (desktop e mobile) e medir o que o axe não cobre: reflow a 320 px, alvos de toque, ordem de Tab, foco após navegar. Ferramenta em pasta temporária, fora das dependências do projeto | Aceitar as duas correções (nível de título, alvo de 44 px) e conferir no print que o visual não mudou |
| Documentação | Esqueleto do README e deste arquivo | *(a completar)* |

## Prompts relevantes

*(a completar com os prompts que mais influenciaram o resultado — o que foi aproveitado e o que foi
descartado)*

- **Aproveitado:** semear os dados pela migration (`HasData`) em vez de um seeder em tempo de execução,
  para o script SQL exportado já sair com o catálogo — atende o entregável "scripts, migrations ou
  arquivo do banco" com um arquivo só.
- **Aproveitado:** busca contra uma coluna normalizada (`NomeBusca`) em vez da extensão `unaccent` do
  PostgreSQL, para funcionar em qualquer Postgres gerenciado gratuito sem permissão de criar extensão.
- **Descartado:** *(a completar)*

## Como o conteúdo foi validado

- **Testes de integração** (`tests/`): 22 testes contra um PostgreSQL real criado na hora
  (Testcontainers), cobrindo o mínimo exigido pelo edital (6 cursos, 3 categorias, 2 modalidades,
  3 status), busca, filtros combinados, 400/404 e o detalhe com módulos.
- **Compilação estrita**: `vue-tsc` no front e `dotnet build` no back.
- **Execução real** com `docker compose`, conferindo cada rota por HTTP (SPA, API, assets, OpenAPI)
  e os logs do contêiner.

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
   log, não apenas testar as rotas.
   **Correção:** `GssEncryptionMode=Disable` na montagem da conexão (`Dados/ConexaoBanco.cs`).
