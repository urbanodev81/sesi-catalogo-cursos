import { flushPromises, mount } from '@vue/test-utils'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { defineComponent, h } from 'vue'
import { createMemoryHistory, createRouter } from 'vue-router'
import { useCatalogo } from '../src/composables/useCatalogo'

const FILTROS = { categorias: [], modalidades: [], status: [] }

/** fetch falso: /api/filtros responde na hora; /api/cursos fica guardado para o teste decidir quando responder. */
function apiFalsa() {
  const pendentes: { url: string; signal: AbortSignal; responder: (corpo: unknown, status?: number) => void }[] = []
  vi.stubGlobal(
    'fetch',
    vi.fn((url: string, init: RequestInit) => {
      if (url === '/api/filtros') return Promise.resolve(new Response(JSON.stringify(FILTROS)))
      return new Promise<Response>((resolve, reject) => {
        init.signal!.addEventListener('abort', () => reject(new DOMException('cancelada', 'AbortError')))
        pendentes.push({
          url,
          signal: init.signal!,
          responder: (corpo, status = 200) => resolve(new Response(JSON.stringify(corpo), { status })),
        })
      })
    }),
  )
  return pendentes
}

/** Monta o composable dentro de um componente na rota do catálogo, como na tela real. */
async function montar(endereco = '/cursos') {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/cursos', name: 'catalogo', component: { render: () => null } },
      { path: '/cursos/:slug', name: 'curso', component: { render: () => null } },
    ],
  })
  await router.push(endereco)
  let catalogo!: ReturnType<typeof useCatalogo>
  const Tela = defineComponent({
    setup() {
      catalogo = useCatalogo()
      return () => h('div')
    },
  })
  const wrapper = mount(Tela, { global: { plugins: [router] } })
  await flushPromises()
  return { catalogo, router, wrapper }
}

const lista = (...nomes: string[]) => ({ total: nomes.length, itens: nomes.map((nome) => ({ nome })) })

let pendentes: ReturnType<typeof apiFalsa>
beforeEach(() => (pendentes = apiFalsa()))
afterEach(() => {
  vi.unstubAllGlobals()
  vi.useRealTimers()
})

describe('useCatalogo', () => {
  it('lê busca e filtros da URL ao abrir (link compartilhado funciona)', async () => {
    const { catalogo } = await montar('/cursos?busca=solda&status=aberto')

    expect(pendentes[0].url).toBe('/api/cursos?busca=solda&status=aberto')
    expect(catalogo.busca.value).toBe('solda')
    expect(catalogo.temFiltro.value).toBe(true)
  })

  it('passa por carregando e termina pronto com a lista e o total', async () => {
    const { catalogo } = await montar()
    expect(catalogo.estado.value).toBe('carregando')

    pendentes[0].responder(lista('Robótica', 'Solda'))
    await flushPromises()

    expect(catalogo.estado.value).toBe('pronto')
    expect(catalogo.total.value).toBe(2)
    expect(catalogo.cursos.value.map((c) => c.nome)).toEqual(['Robótica', 'Solda'])
  })

  it('escolher um filtro grava na URL e mantém os outros', async () => {
    const { catalogo, router } = await montar('/cursos?categoria=tecnologia')

    catalogo.definir('modalidade', 'ead')
    await flushPromises()

    expect(router.currentRoute.value.query).toEqual({ categoria: 'tecnologia', modalidade: 'ead' })
    expect(pendentes.at(-1)!.url).toBe('/api/cursos?categoria=tecnologia&modalidade=ead')
  })

  it('a busca digitada espera 300 ms antes de ir para a URL', async () => {
    vi.useFakeTimers()
    const { catalogo, router } = await montar()

    catalogo.busca.value = 'robo'
    await flushPromises()
    vi.advanceTimersByTime(299)
    await flushPromises()
    expect(router.currentRoute.value.query.busca).toBeUndefined()

    vi.advanceTimersByTime(1)
    await flushPromises()
    expect(router.currentRoute.value.query.busca).toBe('robo')
  })

  it('uma busca nova cancela a anterior, e a resposta velha não sobrescreve a nova', async () => {
    const { catalogo } = await montar()
    const primeira = pendentes[0]

    catalogo.definir('status', 'aberto')
    await flushPromises()
    expect(primeira.signal.aborted).toBe(true)

    pendentes.at(-1)!.responder(lista('Só abertos'))
    await flushPromises()

    expect(catalogo.estado.value).toBe('pronto')
    expect(catalogo.cursos.value.map((c) => c.nome)).toEqual(['Só abertos'])
  })

  it('limpar zera a URL e o campo de busca', async () => {
    const { catalogo, router } = await montar('/cursos?busca=solda&categoria=tecnologia')

    catalogo.limpar()
    await flushPromises()

    expect(router.currentRoute.value.query).toEqual({})
    expect(catalogo.busca.value).toBe('')
    expect(catalogo.temFiltro.value).toBe(false)
  })

  it('filtro inexistente (400) vira estado de erro marcado como inválido', async () => {
    const { catalogo } = await montar('/cursos?categoria=xpto')

    pendentes[0].responder({ errors: { categoria: ["Categoria 'xpto' não existe."] } }, 400)
    await flushPromises()

    expect(catalogo.estado.value).toBe('erro')
    expect(catalogo.erroInvalido.value).toBe(true)
    expect(catalogo.erro.value).toMatch(/Categoria 'xpto' não existe/)
  })

  it('sair para o detalhe de um curso não dispara nova busca', async () => {
    const { router } = await montar('/cursos?busca=solda')
    const antes = pendentes.length

    await router.push('/cursos/solda-basica')
    await flushPromises()

    expect(pendentes.length).toBe(antes)
  })

  it('desmontar a tela cancela a requisição em andamento', async () => {
    const { wrapper } = await montar()

    wrapper.unmount()

    expect(pendentes[0].signal.aborted).toBe(true)
  })
})
