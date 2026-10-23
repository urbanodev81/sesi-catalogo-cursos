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
}

export interface AlunoProgresso {
  nome: string
  concluidas: number
  ultimaAtividade: string | null
}

export interface Turma {
  slug: string
  nome: string
  totalAulas: number
  alunos: AlunoProgresso[]
}
