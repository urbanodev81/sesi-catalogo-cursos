/**
 * Os passos do tour, um conjunto por TELA (nome da rota). O alvo é o atributo `data-tour`, nunca uma
 * classe CSS: classe é estilo e muda quando a tela é revestida; `data-tour` é contrato, e procurar por
 * ele acha quem depende dele. Passo sem alvo aparece no centro da tela.
 */
export interface Passo {
  alvo?: string
  titulo: string
  texto: string
}

export const PASSOS: Record<string, Passo[]> = {
  inicio: [
    {
      titulo: 'Bem-vindo ao catálogo de cursos',
      texto:
        'Este site apresenta os cursos do ambiente virtual: busca, filtros, a página de cada curso com as aulas e, ' +
        'para quem entra, as áreas do aluno e do professor. Em poucos passos mostramos onde fica cada coisa.',
    },
    {
      alvo: 'catalogo',
      titulo: 'Todos os cursos',
      texto: 'Leva à lista completa, com busca pelo nome e filtros de categoria, modalidade e status, que se combinam.',
    },
    {
      alvo: 'destaques',
      titulo: 'Principais cursos',
      texto: 'Deslize para o lado ou use as setas. Cada card abre a página do curso, com os módulos, as aulas e a matrícula.',
    },
    {
      alvo: 'entrar',
      titulo: 'Entrar como aluno ou professor',
      texto:
        'Há contas de demonstração prontas (senha sesi2026). O aluno vê os cursos, as avaliações, os documentos e os ' +
        'colegas; o professor vê as turmas, os materiais e o histórico de cada aluno.',
    },
    {
      alvo: 'acessibilidade',
      titulo: 'Acessibilidade',
      texto: 'Tamanho do texto, contraste, guia de leitura, pausa das animações e tradução para Libras pelo VLibras.',
    },
    {
      alvo: 'ajuda',
      titulo: 'Ajuda',
      texto: 'Respostas para as dúvidas mais comuns. O tour de cada tela também fica aqui, para ver quando quiser.',
    },
  ],
  catalogo: [
    {
      alvo: 'busca',
      titulo: 'Busca pelo nome',
      texto: 'Não diferencia acentos nem maiúsculas. A busca e os filtros vão para o endereço da página: o link pode ser compartilhado.',
    },
    {
      alvo: 'filtros',
      titulo: 'Filtros combináveis',
      texto: 'Categoria, modalidade e status, juntos ou separados, e com a busca. "Limpar filtros" volta à lista completa.',
    },
    {
      alvo: 'resultado',
      titulo: 'Quantidade de resultados',
      texto: 'Muda a cada busca ou filtro, e o leitor de tela anuncia o número novo.',
    },
    {
      alvo: 'lista',
      titulo: 'Os cursos',
      texto: 'Primeiro os de inscrições abertas, depois os que vêm em breve e, por último, os encerrados.',
    },
  ],
  'area-do-aluno': [
    {
      alvo: 'meus-cursos',
      titulo: 'Meus cursos',
      texto: 'O progresso de cada curso, os horários da turma e o botão para continuar da próxima aula.',
    },
    { alvo: 'avaliacoes', titulo: 'Próximas avaliações', texto: 'Provas, trabalhos e questionários dos seus cursos, por data.' },
    {
      alvo: 'documentos',
      titulo: 'Documentos',
      texto:
        'Envie PDF, JPG ou PNG de até 2 MB. Nesta demonstração, o que for enviado é apagado quando o servidor reinicia.',
    },
    {
      alvo: 'colegas',
      titulo: 'Colegas de turma',
      texto: 'Quem faz algum curso com você e quem está online. Entre como Bruno em outra janela para vê-lo online aqui.',
    },
    { alvo: 'materiais', titulo: 'Materiais', texto: 'Arquivos que o professor publicou para as suas turmas.' },
    { alvo: 'perfil', titulo: 'Meu perfil', texto: 'Seus dados e um resumo: cursos, aulas concluídas e documentos pendentes.' },
  ],
  'area-do-professor': [
    {
      alvo: 'turmas',
      titulo: 'Suas turmas',
      texto: 'Cada turma abre e fecha como um acordeão. A primeira já vem aberta.',
    },
    { alvo: 'horarios', titulo: 'Horários', texto: 'Os encontros semanais da turma, ao vivo ou de plantão.' },
    {
      alvo: 'materiais-prof',
      titulo: 'Materiais',
      texto: 'Publique PDF, JPG ou PNG de até 2 MB para a turma (até 10). Os alunos baixam pela área deles.',
    },
    {
      alvo: 'alunos',
      titulo: 'Alunos',
      texto:
        'Abra um aluno para ver as aulas que ele concluiu e quando, e quantos documentos já entregou. O arquivo em si só o aluno vê.',
    },
  ],
}
