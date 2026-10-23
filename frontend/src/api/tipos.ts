// Espelho dos DTOs da API (src/CatalogoCursos.Api/Cursos/CursosEndpoints.cs).

export interface Opcao {
  valor: string
  rotulo: string
}

export interface Categoria extends Opcao {
  icone: string
}

export interface Filtros {
  categorias: Categoria[]
  modalidades: Opcao[]
  status: Opcao[]
}

export interface CursoResumo {
  id: number
  slug: string
  nomeCompleto: string
  nomeBreve: string
  /** Texto curto do card. */
  resumo: string
  /** "Sobre o curso", na página de detalhe. */
  descricao: string
  categoria: Categoria
  modalidade: Opcao
  cargaHorariaHoras: number
  publicoAlvo: string
  status: Opcao
  imagemUrl: string
  /** Início do curso, ISO (AAAA-MM-DD); null quando já encerrou. */
  dataDisponibilizacao: string | null
}

export interface ListaCursos {
  total: number
  itens: CursoResumo[]
}

export interface Aula {
  ordem: number
  titulo: string
  descricao: string
}

export interface Modulo {
  ordem: number
  titulo: string
  aulas: Aula[]
}

export interface CursoDetalhe {
  curso: CursoResumo
  modulos: Modulo[]
}

export interface AulaRef {
  modulo: number
  aula: number
  titulo: string
}

export interface AulaConteudo {
  cursoSlug: string
  cursoNome: string
  modulo: number
  moduloTitulo: string
  aula: number
  titulo: string
  descricao: string
  /** Parágrafos separados por linha em branco; linha com "## " é subtítulo. */
  conteudo: string
  anterior: AulaRef | null
  proxima: AulaRef | null
}

export interface ConsultaCursos {
  busca?: string
  categoria?: string
  modalidade?: string
  status?: string
}

// ---------- Conta (login de demonstração) ----------

export interface Usuario {
  nome: string
  email: string
  papel: 'aluno' | 'professor'
}

export interface AulaMarcada {
  modulo: number
  aula: number
}

export interface Progresso {
  matriculado: boolean
  /** "modulo.aula", ex.: "1.2". */
  concluidas: string[]
}

export interface MeuCurso {
  curso: CursoResumo
  totalAulas: number
  concluidas: number
  /** Primeira aula ainda não concluída; null quando terminou o curso. */
  proxima: AulaMarcada | null
  horarios: Horario[]
}

/** Encontro semanal da turma, já formatado ("Terça-feira", "19:00"). */
export interface Horario {
  dia: string
  inicio: string
  fim: string
  descricao: string
}

export interface Material {
  id: number
  titulo: string
  nomeArquivo: string
  tamanho: number
  em: string
  cursoSlug: string
  cursoNome: string
}

export interface AulaFeita {
  modulo: number
  aula: number
  titulo: string
  em: string
}

export interface AlunoProgresso {
  nome: string
  concluidas: number
  ultimaAtividade: string | null
  /** Aulas concluídas, da mais recente para a mais antiga. */
  historico: AulaFeita[]
  documentosEntregues: number
  documentosExigidos: number
}

export interface Turma {
  slug: string
  nome: string
  totalAulas: number
  alunos: AlunoProgresso[]
  horarios: Horario[]
  materiais: Material[]
}

// ---------- Painel do aluno ----------

export interface Avaliacao {
  id: number
  titulo: string
  tipo: Opcao
  /** ISO 8601 com fuso. */
  em: string
  cursoSlug: string
  cursoNome: string
}

export interface Envio {
  nomeArquivo: string
  tamanho: number
  em: string
}

export interface Documento {
  id: number
  titulo: string
  orientacao: string
  /** null enquanto o aluno não enviou. */
  envio: Envio | null
}

export interface Colega {
  nome: string
  online: boolean
  /** Nome breve dos cursos em comum. */
  cursos: string[]
}

export interface Perfil {
  nome: string
  email: string
  alunoDesde: string | null
  cursos: number
  aulasConcluidas: number
  documentosPendentes: number
  proximaAvaliacao: Avaliacao | null
}
