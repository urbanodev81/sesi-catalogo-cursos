import { afterEach, describe, expect, it, vi } from 'vitest'
import { ErroApi, listarCursos, obterCurso, obterSessao } from '../src/api/cliente'

const json = (status: number, corpo?: unknown) =>
  new Response(corpo === undefined ? null : JSON.stringify(corpo), {
    status,
    headers: { 'Content-Type': 'application/json' },
  })

function respondeCom(resposta: Response | Error) {
  const fetch = vi.fn(() => (resposta instanceof Error ? Promise.reject(resposta) : Promise.resolve(resposta)))
  vi.stubGlobal('fetch', fetch)
  return fetch
}

/** O erro que a promessa rejeitou, para olhar os campos. */
async function erroDe(promessa: Promise<unknown>) {
  try {
    await promessa
  } catch (e) {
    return e as ErroApi
  }
  throw new Error('a promessa deveria ter falhado')
}

afterEach(() => vi.unstubAllGlobals())

describe('listarCursos', () => {
  it('manda só os filtros preenchidos na query string', async () => {
    const fetch = respondeCom(json(200, { total: 0, itens: [] }))

    await listarCursos({ busca: 'robótica', categoria: '', modalidade: 'ead', status: '' })

    expect(fetch.mock.calls[0][0]).toBe('/api/cursos?busca=rob%C3%B3tica&modalidade=ead')
  })

  it('sem filtro nenhum, chama a rota sem query string', async () => {
    const fetch = respondeCom(json(200, { total: 0, itens: [] }))

    await listarCursos({})

    expect(fetch.mock.calls[0][0]).toBe('/api/cursos')
  })

  it('400 vira erro "inválido" com a mensagem da API e a dica de limpar', async () => {
    respondeCom(json(400, { errors: { categoria: ["Categoria 'xpto' não existe."] } }))

    const erro = await erroDe(listarCursos({ categoria: 'xpto' }))

    expect(erro.invalido).toBe(true)
    expect(erro.message).toBe("Categoria 'xpto' não existe. Limpe os filtros para ver todos os cursos.")
  })

  it('500 vira mensagem genérica, sem marcar inválido', async () => {
    respondeCom(json(500))

    const erro = await erroDe(listarCursos({}))

    expect(erro.invalido).toBe(false)
    expect(erro.status).toBe(500)
    expect(erro.message).toBe('Não foi possível carregar os cursos agora. Tente novamente.')
  })

  it('falha de rede vira mensagem de conexão', async () => {
    respondeCom(new TypeError('Failed to fetch'))

    const erro = await erroDe(listarCursos({}))

    expect(erro).toBeInstanceOf(ErroApi)
    expect(erro.message).toMatch(/Não foi possível conectar ao servidor/)
  })

  it('requisição cancelada por quem chamou não vira ErroApi', async () => {
    const controle = new AbortController()
    controle.abort()
    respondeCom(new DOMException('cancelada', 'AbortError'))

    const erro = await erroDe(listarCursos({}, controle.signal))

    expect(erro).not.toBeInstanceOf(ErroApi)
  })
})

describe('respostas de erro', () => {
  it('404 marca "não encontrado"', async () => {
    respondeCom(json(404))

    const erro = await erroDe(obterCurso('nao-existe'))

    expect(erro.naoEncontrado).toBe(true)
  })

  it('429 tem texto próprio, sem depender do corpo', async () => {
    respondeCom(json(429))

    const erro = await erroDe(obterCurso('qualquer'))

    expect(erro.status).toBe(429)
    expect(erro.message).toBe('Muitas tentativas seguidas. Espere um minuto e tente de novo.')
  })

  it('o slug vai codificado na URL', async () => {
    const fetch = respondeCom(json(200, {}))

    await obterCurso('a/b c')

    expect(fetch.mock.calls[0][0]).toBe('/api/cursos/a%2Fb%20c')
  })
})

describe('obterSessao', () => {
  it('401 significa visitante: devolve null em vez de falhar', async () => {
    respondeCom(json(401))

    expect(await obterSessao()).toBeNull()
  })

  it('outro erro continua sendo erro', async () => {
    respondeCom(json(500))

    await expect(obterSessao()).rejects.toBeInstanceOf(ErroApi)
  })
})
