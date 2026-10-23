/*
 * Texto institucional do Figma: não é dado do catálogo, então não vem da API.
 * Os cursos em destaque são a curadoria do protótipo (os 6 cursos que ele mostra); num
 * sistema real seria um campo "destaque" no cadastro. Ficam aqui por slug + nome para o
 * rodapé linkar sem precisar buscar a lista inteira em toda página.
 */

export const CURSOS_DESTAQUE = [
  { slug: 'matematica-precisao-medidas-escalas-metalurgia', nome: 'Matemática para a Precisão' },
  { slug: 'geometria-modelagem-design-textil', nome: 'Geometria e Modelagem no Design Têxtil' },
  { slug: 'estatistica-controle-qualidade-producao', nome: 'Estatística e Controle de Qualidade' },
  { slug: 'fisica-movimento-energia-metalurgia', nome: 'Física do Movimento e da Energia' },
  { slug: 'quimica-sustentabilidade', nome: 'Química e Sustentabilidade' },
  { slug: 'ciencia-materiais-metalurgia', nome: 'Ciência dos Materiais na Metalurgia' },
] as const

export const NUMEROS = [
  { valor: '25.000+', rotulo: 'Profissionais capacitados' },
  { valor: '500+', rotulo: 'Empresas parceiras' },
  { valor: '4.8/5', rotulo: 'Avaliação média' },
  { valor: '2.5M+', rotulo: 'Horas de capacitação' },
]

// A home mostra os três primeiros com ícone vermelho; a página do curso, os quatro com ícone bege.
// O ícone é o nome base do arquivo: o pacote tem "-red.svg" e "-bege.svg" (o 4º, só bege).
export const DIFERENCIAIS = [
  {
    icone: '01-certificacao',
    titulo: 'Certificação SESI',
    texto: 'Cursos com a melhor certificação do mercado industrial reconhecidos pelo SESI.',
  },
  {
    icone: '02-professores',
    titulo: 'Professores especialistas',
    texto: 'Contamos com professores reconhecidos por sua excelência no mercado.',
  },
  {
    icone: '03-suporte',
    titulo: 'Suporte contínuo',
    texto: 'Você não está sozinho! Acompanhamento online durante todo o curso.',
  },
  {
    icone: '04-flexibilidade',
    titulo: 'Flexibilidade',
    texto: 'Turmas presenciais e online com horários que se adaptam à sua rotina.',
  },
]

export const DEPOIMENTOS = [
  {
    texto:
      'A experiência do curso foi incrível! As aulas foram muito bem estruturadas e o cronograma foi cuidadosamente planejado, facilitando muito o aprendizado.',
    nome: 'Lucas Pereira',
    cargo: 'Analista na Empresa X',
  },
  {
    texto:
      'Concluir o curso foi uma jornada enriquecedora. As aulas eram dinâmicas e o horário foi organizado para maximizar nosso tempo de estudo.',
    nome: 'Mariana Costa',
    cargo: 'Gestora na Empresa X',
  },
  {
    texto:
      'O curso superou minhas expectativas! As aulas eram bem elaboradas e o planejamento do horário ajudou a manter o foco e a produtividade.',
    nome: 'Thiago Almeida',
    cargo: 'Técnico na Empresa X',
  },
  {
    texto:
      'Finalizar o curso foi uma experiência motivadora. As aulas foram muito bem preparadas e o cronograma foi ideal para o aprendizado.',
    nome: 'Fernanda Silva',
    cargo: 'Analista na Empresa X',
  },
  {
    // O print corta este card; o fim do texto e o sobrenome foram completados.
    texto:
      'A conclusão do curso foi uma etapa gratificante. O conteúdo era bem organizado e o horário foi pensado para facilitar o aprendizado.',
    nome: 'Gabriel Santos',
    cargo: 'Analista na Empresa X',
  },
]

export const CONTATO = { email: 'contato@sesindustria.com.br', endereco: 'Av. Paulista, 1313' }

// Acordeão da página do curso. O Figma só mostra os títulos fechados; o texto é nosso,
// genérico de propósito (vale para qualquer curso) e coerente com o que o catálogo informa.
export const AULAS_E_AVALIACOES = [
  'As aulas ficam no ambiente virtual de aprendizagem e podem ser feitas no seu ritmo, no computador ou no celular. Cada aula marca o seu progresso automaticamente.',
  'Ao fim de cada módulo há uma atividade avaliativa. Para receber o certificado é preciso concluir todas as aulas e atingir 70% de aproveitamento nas atividades.',
]

// Painel de ajuda (botão "Ajuda"): as dúvidas do curso mais as de uso da plataforma.
export const AJUDA_PLATAFORMA = [
  {
    pergunta: 'Como entro na minha conta?',
    resposta: 'Use o botão "Entrar", no alto da página, com o e-mail e a senha da sua matrícula. Professores entram pelo mesmo botão.',
  },
  {
    pergunta: 'Onde envio meus documentos?',
    resposta: 'Depois de entrar, em "Meu painel", no bloco Documentos. Aceitamos PDF, JPG ou PNG de até 2 MB.',
  },
  {
    pergunta: 'Onde ficam os materiais das aulas?',
    resposta: 'Os materiais que o professor publica aparecem em "Meu painel", no bloco Materiais das turmas.',
  },
]

export const DUVIDAS_FREQUENTES = [
  {
    pergunta: 'Preciso pagar pelo curso?',
    resposta: 'Não. Os cursos do catálogo são gratuitos para trabalhadores da indústria e seus dependentes.',
  },
  {
    pergunta: 'O certificado tem validade?',
    resposta: 'Sim. O certificado é emitido pelo SESI, com a carga horária do curso, e pode ser usado em todo o território nacional.',
  },
  {
    pergunta: 'Posso fazer mais de um curso ao mesmo tempo?',
    resposta: 'Pode. Recomendamos no máximo dois cursos simultâneos para manter o ritmo de estudo.',
  },
]
