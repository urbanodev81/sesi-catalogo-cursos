import type { AulaConteudo, ConsultaCursos, CursoDetalhe, Filtros, ListaCursos } from './tipos'

/**
 * Erro com mensagem já pronta para a tela. `naoEncontrado` separa 404 de falha;
 * `invalido` marca 400 (filtro que não existe): tentar de novo não resolve, limpar resolve.
 */
export class ErroApi extends Error {
  readonly naoEncontrado: boolean
  readonly invalido: boolean

  constructor(message: string, naoEncontrado = false, invalido = false) {
    super(message)
    this.naoEncontrado = naoEncontrado
    this.invalido = invalido
  }
}

/** Primeira mensagem do ValidationProblem da API (ex.: "Categoria 'xpto' não existe."). */
async function mensagemDeValidacao(resposta: Response) {
  try {
    const corpo = (await resposta.json()) as { errors?: Record<string, string[]> }
    const primeira = Object.values(corpo.errors ?? {}).flat()[0]
    if (primeira) return `${primeira} Limpe os filtros para ver todos os cursos.`
  } catch {
    /* corpo fora do formato: cai na mensagem genérica */
  }
  return 'A busca tem um filtro inválido. Limpe os filtros para ver todos os cursos.'
}

const TEMPO_LIMITE_MS = 60_000 // a hospedagem gratuita "acorda" em até ~50 s

async function obter<T>(caminho: string, sinal?: AbortSignal): Promise<T> {
  const limite = AbortSignal.timeout(TEMPO_LIMITE_MS)
  const signal = sinal ? AbortSignal.any([sinal, limite]) : limite

  let resposta: Response
  try {
    resposta = await fetch(caminho, { signal, headers: { Accept: 'application/json' } })
  } catch (e) {
    if (sinal?.aborted) throw e // cancelada por uma busca mais nova: quem chamou ignora
    throw new ErroApi(
      limite.aborted
        ? 'O servidor demorou demais para responder. Tente novamente em instantes.'
        : 'Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente.',
    )
  }

  if (resposta.status === 404) throw new ErroApi('Conteúdo não encontrado.', true)
  if (resposta.status === 400) throw new ErroApi(await mensagemDeValidacao(resposta), false, true)
  if (!resposta.ok) throw new ErroApi('Não foi possível carregar os cursos agora. Tente novamente.')
  return (await resposta.json()) as T
}

export function listarCursos(consulta: ConsultaCursos, sinal?: AbortSignal) {
  const params = new URLSearchParams()
  for (const [chave, valor] of Object.entries(consulta)) {
    if (valor) params.set(chave, valor)
  }
  const qs = params.toString()
  return obter<ListaCursos>(`/api/cursos${qs ? `?${qs}` : ''}`, sinal)
}

export const obterCurso = (slug: string, sinal?: AbortSignal) =>
  obter<CursoDetalhe>(`/api/cursos/${encodeURIComponent(slug)}`, sinal)

export const obterFiltros = (sinal?: AbortSignal) => obter<Filtros>('/api/filtros', sinal)

export const obterAula = (slug: string, modulo: number, aula: number, sinal?: AbortSignal) =>
  obter<AulaConteudo>(`/api/cursos/${encodeURIComponent(slug)}/modulos/${modulo}/aulas/${aula}`, sinal)
