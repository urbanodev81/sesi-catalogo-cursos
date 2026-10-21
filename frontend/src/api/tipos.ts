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
  descricao: string
  categoria: Categoria
  modalidade: Opcao
  cargaHorariaHoras: number
  publicoAlvo: string
  status: Opcao
  imagemUrl: string
  /** ISO (AAAA-MM-DD), só em curso que ainda não abriu. */
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

export interface ConsultaCursos {
  busca?: string
  categoria?: string
  modalidade?: string
  status?: string
}
