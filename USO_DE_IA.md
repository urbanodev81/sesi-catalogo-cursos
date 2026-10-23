# Uso de Inteligência Artificial

> Registro feito durante o desenvolvimento, etapa por etapa.

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
| Front-end | Gerar cliente da API, estado de busca/filtros e telas base; depois, montar home, listagem, página do curso e tela de aula a partir dos prints do Figma, e propor como fazer a listagem (que o protótipo não tem) só com peças dele | Mandar os prints do protótipo, que a IA não consegue abrir; escolher fazer a tela de aula completa, com progresso no navegador e sem login; revisar cada diferença em relação ao Figma listada no README |
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
   **Descartado:** os textos dos cursos, que a IA inventou porque não tinha acesso ao Figma. Foram
   trocados pelos dados fictícios do protótipo quando os prints chegaram (migration `CatalogoDoFigma`).
3. **Escolha "Mando prints das telas"**, numa pergunta da IA sobre como seguir com o visual. O link do
   Figma é de protótipo e não abre fora do navegador, então a IA não montou telas "no escuro". Ela
   esperou os prints, porque o edital cobra fidelidade e justificativa para cada diferença.
4. **"Não consegue baixar e preparar tudo para mim?"**, na troca de máquina. Virou o `preparar.sh`.
5. **Escolha das quatro frentes finais** (publicar, README/USO_DE_IA, auditoria de acessibilidade,
   layout do Figma). A auditoria veio antes do layout de propósito: o que ela corrigiu (estrutura de
   títulos, área clicável) continua valendo com qualquer visual. Ela foi rodada de novo depois do layout
   (achado nº 10 abaixo).
6. **Prints do Figma, com "a página do curso fiel ao Figma e a tela de aula completa"**. A IA percebeu
   que os cursos dos prints eram os "dados fictícios do catálogo" que o edital diz estarem no Figma, e
   propôs alinhar o seed a eles antes do visual.
   **Aproveitado:** o protótipo não tem a listagem com busca e filtros, e o edital pede para não criar
   design novo. A listagem foi montada com o hero, os cards e o campo de busca que o protótipo já tem.
   **Aproveitado:** o progresso das aulas atrás de uma interface (`RepositorioProgresso`), guardado no
   navegador por enquanto. O login, se entrar, troca só a implementação.
   **Decidido por mim:** login de aluno e de professor, mas só depois de tudo o que o edital exige. A IA
   o fez numa branch separada (`login`), para eu decidir se entra na entrega.
   **Decidido por mim:** "Matricule-se" leva à primeira aula, que é o fluxo do protótipo. Num sistema
   real, levaria à matrícula no Moodle.
7. **"Olhe nossos outros projetos, estou pensando em aplicar algumas coisas nossas, como acessibilidade,
   light/dark mode, botão de redes sociais e subir e descer suavemente, além do loading quando muda de
   tela. O que acha?"** A IA levantou como esses recursos foram feitos em outros sites meus e pesou cada
   um contra a regra do edital de não criar design novo.
   **Aproveitado:** a barra de carregamento entre páginas e o "voltar ao topo", que são pequenos e
   resolvem problemas reais (rotas carregadas sob demanda; páginas longas no celular).
   **Descartado:** o modo escuro, que seria uma segunda paleta fora do Figma; o botão flutuante de redes
   sociais, que repetiria o rodapé do protótipo sem nenhum destino real; o botão de "descer", que a
   rolagem do navegador já resolve. **Decidido por mim:** o painel de acessibilidade com VLibras ficou
   de fora, mesmo recomendado pela IA (e entrou na rodada seguinte).
8. **Revisão rodando o projeto local:** "a seta da aula não pode voltar para a página do curso, saiu da
   área do aluno; o botão de subir não sobe fluindo, só salta; cadê a acessibilidade?". A IA mediu a
   rolagem no navegador e achou a causa (erro nº 14 abaixo).
   **Aproveitado:** a seta da aula volta para a área do aluno quando há login, e para o curso quando
   não há (quem chegou pelo "Matricule-se"). O painel de acessibilidade com VLibras, portado de outro
   projeto meu, reescrito sem Tailwind e sem biblioteca de ícones.
   **Decidido por mim:** os quatro extras da área do aluno (próximas avaliações, documentos pendentes com
   envio de arquivo, colegas da turma online e perfil), mesmo com o alerta da IA de que nada disso está no
   Figma e de que o "online" e o envio de arquivo pedem infraestrutura. A IA fez o online por sinal a cada
   minuto (sem WebSocket) e guardou os arquivos no banco, e documentou as duas escolhas como limitação.
   **Decidido por mim:** a subida animada mesmo com movimento reduzido no sistema. A IA recomendou
   manter o salto, que é o que a preferência pede. Escolhi animar porque é um deslocamento pedido pelo
   clique, e o painel dá o "Parar animações" para quem quiser o salto.

## Como o conteúdo foi validado

- **Teste de interação** (Playwright, fora do projeto): busca, filtros combinados, limpar, lista vazia,
  filtro inválido, abrir o curso, matricular-se, concluir aula, progresso após recarregar, busca de aula,
  painel da aula no celular, acordeão por teclado, ordem de Tab. Barra de carregamento e "voltar ao topo"
  em 1440 e 390 px: a barra aparece com a tela atrasada e some ao chegar, o botão leva ao topo com o foco
  no conteúdo, e na página do curso não cobre a barra de matrícula enquanto ela está fixa no pé da tela.
  Painel de acessibilidade: foco ao abrir, Esc devolvendo o foco, fonte e filtro aplicados e mantidos
  depois de recarregar, "Restaurar padrão", VLibras acionável pelo painel com o botão nativo escondido.
  Painel do aluno com dois navegadores (Ana e Bruno): Bruno aparece online para a Ana, envio de arquivo
  falso recusado com a mensagem da API, envio de PDF atualizando a contagem de pendentes, perfil, axe e
  ausência de rolagem horizontal em 1440 e 390 px.

- **Testes de integração** (`tests/`): 48 testes contra um PostgreSQL real criado na hora
  (Testcontainers). Cobrem o mínimo exigido pelo edital (6 cursos, 3 categorias, 2 modalidades,
  3 status), a busca, os filtros combinados, as respostas 400/404, o detalhe com módulos e o login (sessão, papéis, limite de tentativas, progresso).
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
6. **Migration que não aplicava num banco existente.** Ao trocar o catálogo pelos cursos do Figma, a IA
   gerou a migration com `dotnet ef migrations add` e seguiu em frente. Dez testes quebraram com
   `23505: duplicate key ... IX_aulas_ModuloId_Ordem`: o EF atualiza o seed linha a linha, e no meio do
   caminho duas aulas ficavam na mesma posição do mesmo módulo. Os testes pegaram porque cada execução
   aplica todas as migrations, em sequência, num banco novo. **Correção:** a migration `CatalogoDoFigma`
   remove os dois índices de ordem no começo e os recria no fim, dentro da mesma transação. Depois, o
   `catalogo.sql` regerado foi aplicado num banco vazio para conferir.
7. **Página mais larga que a tela no celular.** No carrossel de depoimentos, a IA escreveu a nota
   "5 de 5" num texto só para leitor de tela (`position: absolute`), sem um ancestral posicionado dentro
   do carrossel. O texto escapou do recorte da rolagem e esticou a página para 1193 px numa tela de
   390 px. O axe não acusa isso; quem pegou foi o print em 390 px, e depois a medição eliminando cada
   carrossel. **Correção:** `position: relative` no card do depoimento.
8. **Botões com a cor errada por ordem no CSS.** Os modificadores `.botao--claro` e `.botao--contorno`
   foram escritos antes da regra `.botao` no arquivo. Com a mesma especificidade, a última regra ganha, e
   os três botões da home saíram vermelhos em vez de brancos. O build passou; o print mostrou o erro.
   **Correção:** seletor `.botao.botao--claro`, que não depende da ordem.
9. **Âncora escondida atrás do cabeçalho fixo.** O link "Sobre" (`/#sobre`) contava com o
   `scroll-margin-top` do CSS, mas o vue-router rola com `scrollTo` e ignora essa propriedade. O teste
   de interação mediu a seção em 0 px do topo, embaixo do cabeçalho. **Correção:** o recuo de 72 px
   passado no `scrollBehavior` do router.
10. **Foco que pulava o "Pular para o conteúdo".** Para o leitor de tela acompanhar a troca de página, a
    IA move o foco para o `<main>` quando a rota muda. Mas, ao abrir uma página direto pelo link, a
    primeira resolução da rota (de `/` para a URL aberta) também contava como troca. O foco já começava
    no conteúdo, e o primeiro Tab ia para o meio da página. O axe não acusa isso; quem pegou foi o teste
    de ordem de Tab na reauditoria depois do layout. **Correção:** o foco só se move em navegações
    depois de `router.isReady()` (commit `12bfee4`).
11. **Botão que nunca resolveria o erro.** Com um filtro inválido na URL (ex.: link antigo com uma
    categoria que não existe), a tela mostrava "Não foi possível carregar os cursos" com "Tentar
    novamente". Tentar de novo dá o mesmo 400, para sempre. O teste de interação mostrou o problema.
    **Correção:** o cliente lê a mensagem do campo que a API já devolvia ("Categoria 'xpto' não
    existe.") e a tela oferece "Limpar filtros" no lugar de "Tentar novamente".
12. **Consulta que o EF não traduz.** Para listar os cursos do aluno, a IA escreveu
    `Matriculas.Where(...).Select(m => m.Curso).Include(c => c.Modulos)...` com consulta dividida
    (`AsSplitQuery`). Compilou, mas a rota respondia 500. O teste de integração "aluno de demonstração já
    está cursando os dois cursos" pegou o erro antes de qualquer tela. **Correção:** partir de `Cursos`,
    com `Where(c => Matriculas.Any(...))`, que é também a forma mais simples de ler.
13. **Resposta que se perderia em silêncio.** O endpoint de sair recebia só o `HttpContext`. Com essa
    assinatura, o método casa com `RequestDelegate`, e o ASP.NET descarta o `204` retornado. Não quebra nada
    visível; quem avisou foi o analisador do build (`ASP0016`). **Correção:** registrar como `Delegate`, com
    um comentário explicando o porquê.
14. **Rolagem "suave" que saltava.** O "voltar ao topo" usava `scrollTo({ behavior: 'smooth' })`. No meu
    Windows, com "Efeitos de animação" desligado, o Chrome responde `prefers-reduced-motion: reduce` e
    ignora o smooth nativo; no teste automatizado, com a configuração padrão, a rolagem era suave, então o
    defeito só apareceu quando eu abri o site. A IA mediu a posição a cada 80 ms nos dois modos e achou a
    configuração do sistema. **Correção:** a subida é animada pelo próprio script (`requestAnimationFrame`,
    450 a 1300 ms conforme a distância), e o "Parar animações" do painel a torna instantânea.
