import type {
  AulaConteudo,
  AulaMarcada,
  ConsultaCursos,
  CursoDetalhe,
  Filtros,
  ListaCursos,
  MeuCurso,
  Progresso,
  Turma,
  Usuario,
} from './tipos'

/**
 * Erro com mensagem já pronta para a tela. `naoEncontrado` separa 404 de falha;
 * `invalido` marca 400 (filtro que não existe): tentar de novo não resolve, limpar resolve;
 * `status` guarda o código HTTP para quem precisa distinguir 401, 409, 429.
 */
export class ErroApi extends Error {
  readonly naoEncontrado: boolean
  readonly invalido: boolean
  readonly status: number

  constructor(message: string, naoEncontrado = false, invalido = false, status = 0) {
    super(message)
    this.naoEncontrado = naoEncontrado
    this.invalido = invalido
    this.status = status
  }
}

/** Mensagem do ProblemDetails da API ("E-mail ou senha incorretos."), com texto próprio para o 429. */
async function mensagemDoProblema(resposta: Response) {
  if (resposta.status === 429) return 'Muitas tentativas seguidas. Espere um minuto e tente de novo.'
  try {
    const corpo = (await resposta.json()) as { detail?: string }
    if (corpo.detail) return corpo.detail
  } catch {
    /* sem corpo (ex.: 401 de rota protegida) */
  }
  return resposta.status === 401 ? 'Sua sessão terminou. Entre de novo.' : 'Não foi possível concluir a ação.'
}

/** POST com corpo JSON. O cookie da sessão vai junto porque a API tem a mesma origem. */
const enviar = <T = void>(caminho: string, corpo?: unknown) =>
  obter<T>(caminho, undefined, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: corpo === undefined ? undefined : JSON.stringify(corpo),
  })

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

async function obter<T>(caminho: string, sinal?: AbortSignal, init?: RequestInit): Promise<T> {
  const limite = AbortSignal.timeout(TEMPO_LIMITE_MS)
  const signal = sinal ? AbortSignal.any([sinal, limite]) : limite

  let resposta: Response
  try {
    resposta = await fetch(caminho, { ...init, signal, headers: { Accept: 'application/json', ...init?.headers } })
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
  if (resposta.status === 401 || resposta.status === 409 || resposta.status === 429) {
    throw new ErroApi(await mensagemDoProblema(resposta), false, false, resposta.status)
  }
  if (!resposta.ok) throw new ErroApi('Não foi possível carregar os cursos agora. Tente novamente.', false, false, resposta.status)
  return (resposta.status === 204 ? undefined : await resposta.json()) as T
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

// ---------- Conta ----------

/** Quem está logado, ou null. */
export async function obterSessao(): Promise<Usuario | null> {
  try {
    return await obter<Usuario>('/api/conta')
  } catch (e) {
    if (e instanceof ErroApi && e.status === 401) return null
    throw e
  }
}

export const entrar = (email: string, senha: string) => enviar<Usuario>('/api/conta/entrar', { email, senha })
export const sair = () => enviar('/api/conta/sair')
export const obterMeusCursos = (sinal?: AbortSignal) => obter<MeuCurso[]>('/api/conta/cursos', sinal)
export const obterTurmas = (sinal?: AbortSignal) => obter<Turma[]>('/api/professor/turmas', sinal)

const base = (slug: string) => `/api/cursos/${encodeURIComponent(slug)}`
export const obterProgresso = (slug: string) => obter<Progresso>(`${base(slug)}/progresso`)
export const matricular = (slug: string) => enviar(`${base(slug)}/matricula`)
export const concluirAula = (slug: string, aula: AulaMarcada) => enviar(`${base(slug)}/progresso`, aula)
