import type { ConsultaCursos, CursoDetalhe, Filtros, ListaCursos } from './tipos'

/** Erro com mensagem já pronta para a tela; `naoEncontrado` separa 404 de falha. */
export class ErroApi extends Error {
  readonly naoEncontrado: boolean

  constructor(message: string, naoEncontrado = false) {
    super(message)
    this.naoEncontrado = naoEncontrado
  }
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
