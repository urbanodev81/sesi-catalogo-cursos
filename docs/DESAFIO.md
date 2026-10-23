# Desafio — Processo seletivo SESI-SP 519/2026, Desenvolvedor de Sistemas I

Transcrição fiel das instruções recebidas em 21/10/2026 (PDF "Instruções_Atividade prática_519_2026_DEV"
e e-mail). **Fonte da verdade do requisito.** Prazo: **26/10/2026, 8h.** Apresentação técnica na
entrevista presencial. Entrega: repositório no GitHub + aplicação publicada.

Figma (protótipo): <https://www.figma.com/proto/YB0o91nnOEB5bXXCtmy5s6/-TEST-DEV--SESI-Ind%C3%BAstria?node-id=6017-342&p=f&starting-point-node-id=6017%3A342&page-id=3%3A2>

## 1. Contexto

O Centro de Tecnologias e Educação Digital (CTED) necessita de uma aplicação web para apresentar um
catálogo de cursos disponíveis em um ambiente educacional Moodle. A aplicação deverá permitir que o
usuário consulte, pesquise e filtre os cursos, além de visualizar informações detalhadas de cada oferta.
O protótipo no Figma é a referência para o front-end e, consequentemente, para o back-end.

Não é necessário acessar, instalar, configurar ou integrar a um Moodle (seria um diferencial).

## 2. Materiais fornecidos

Link do Figma; imagens e ícones (em `frontend/public/assets/`); informações fictícias para o catálogo
(no Figma). O Figma é a referência visual inclusive para páginas adicionais; não criar design novo.

## 3. Desafio

- [x] Front-end baseado no Figma (responsividade e acessibilidade)
- [x] Back-end em C#/.NET
- [x] API para consulta dos cursos
- [x] Banco de dados de código aberto (PostgreSQL)
- [x] Integração front-end ↔ API ↔ banco
- [x] Publicação da aplicação
- [x] Documentação técnica
- [x] Registro do uso de IA (`USO_DE_IA.md`)

## 4. Informações para o desenvolvimento

**a) Cada curso:** nome completo; nome breve; descrição; categoria; modalidade; carga horária;
público-alvo; status; imagem de apresentação; data de disponibilização, quando aplicável.

**b) Mínimo:** 6 cursos; 3 categorias; 2 modalidades; 3 status; cargas horárias, módulos, descrições de
aulas e públicos-alvo diferentes. Todos os dados fictícios. *(Hoje: 12 cursos, 4 categorias,
3 modalidades, 3 status — coberto por teste.)*

**c) Funcionalidades obrigatórias:**

- [x] Listar os cursos por meio da API
- [x] Pesquisar cursos pelo nome
- [x] Filtrar por categoria, modalidade ou status
- [x] Combinar pesquisa e filtros
- [x] Apresentar a quantidade de resultados
- [x] Limpar os filtros aplicados
- [x] Abrir os detalhes de um curso
- [x] Mensagem quando nenhum curso for encontrado
- [x] Estado de carregamento
- [x] Mensagem em caso de erro
- [x] Interface funcional em diferentes tamanhos de tela *(1440, 390 e 320 px; mobile desenhado com as peças do Figma)*
- Não é necessário cadastro, edição ou exclusão de cursos.

**d) `USO_DE_IA.md`:** ferramenta; objetivo; prompts relevantes (aproveitado/descartado); como foi
validado; um exemplo de erro ou inadequação; correção realizada.

**e) `README.md`:** objetivo; tecnologias; arquitetura; estrutura de pastas; execução; configuração do
banco; endpoints; modelo de dados; estratégia de responsividade; decisões de acessibilidade; processo de
publicação; limitações conhecidas.

## 5. Entregáveis

1. Link do repositório no GitHub
2. Link da aplicação publicada (plataforma gratuita; sem custo para o SESI-SP)
3. Código-fonte do front-end
4. Código-fonte do back-end
5. Scripts, migrations ou arquivo do banco
6. `README.md`
7. `USO_DE_IA.md`
8. Relação dos arquivos de publicação, em ZIP
9. Relação das limitações ou itens não concluídos — **diferenças em relação ao Figma devem ser
   identificadas e justificadas**

Indisponibilidade temporária da hospedagem não elimina, desde que haja instruções para execução local.
