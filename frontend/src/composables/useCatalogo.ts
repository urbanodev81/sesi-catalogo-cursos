import { computed, onBeforeUnmount, ref, shallowRef, watch } from 'vue'
import { useRoute, useRouter, type LocationQuery } from 'vue-router'
import { ErroApi, listarCursos, obterFiltros } from '../api/cliente'
import type { ConsultaCursos, CursoResumo, Filtros } from '../api/tipos'

export type Estado = 'carregando' | 'pronto' | 'erro'

const ATRASO_BUSCA_MS = 300

const texto = (v: LocationQuery[string]) => (typeof v === 'string' ? v : '')

/**
 * Estado do catálogo com a URL como fonte da verdade: busca e filtros vivem na
 * query string, então o link é compartilhável e o "voltar" do navegador desfaz
 * o último filtro. Cada mudança cancela a requisição anterior (AbortController),
 * para uma resposta velha nunca sobrescrever uma nova.
 */
export function useCatalogo() {
  const route = useRoute()
  const router = useRouter()

  const busca = ref(texto(route.query.busca))
  const consulta = computed<ConsultaCursos>(() => ({
    busca: texto(route.query.busca).trim(),
    categoria: texto(route.query.categoria),
    modalidade: texto(route.query.modalidade),
    status: texto(route.query.status),
  }))

  const cursos = shallowRef<CursoResumo[]>([])
  const total = ref(0)
  const estado = ref<Estado>('carregando')
  const erro = ref('')
  const filtros = shallowRef<Filtros>({ categorias: [], modalidades: [], status: [] })

  const temFiltro = computed(() => Object.values(consulta.value).some(Boolean))

  let controle: AbortController | undefined
  async function carregar() {
    controle?.abort()
    controle = new AbortController()
    const sinal = controle.signal
    estado.value = 'carregando'
    try {
      const lista = await listarCursos(consulta.value, sinal)
      cursos.value = lista.itens
      total.value = lista.total
      estado.value = 'pronto'
    } catch (e) {
      if (sinal.aborted) return
      erro.value = e instanceof ErroApi ? e.message : 'Ocorreu um erro inesperado.'
      estado.value = 'erro'
    }
  }

  function aplicar(mudanca: Partial<ConsultaCursos>) {
    const query: Record<string, string> = {}
    for (const [chave, valor] of Object.entries({ ...consulta.value, ...mudanca })) {
      if (valor) query[chave] = valor
    }
    router.replace({ query })
  }

  function definir(campo: 'categoria' | 'modalidade' | 'status', valor: string) {
    aplicar({ [campo]: valor })
  }

  function limpar() {
    busca.value = ''
    router.replace({ query: {} })
  }

  let espera: ReturnType<typeof setTimeout> | undefined
  watch(busca, (valor) => {
    clearTimeout(espera)
    espera = setTimeout(() => aplicar({ busca: valor.trim() }), ATRASO_BUSCA_MS)
  })

  // A URL mudou (filtro, busca, voltar do navegador): recarrega e alinha o campo.
  watch(
    consulta,
    (atual, anterior) => {
      if (route.name !== 'catalogo') return // saindo para o detalhe: a query some, não é filtro novo
      if (anterior && JSON.stringify(atual) === JSON.stringify(anterior)) return
      if (atual.busca !== busca.value.trim()) busca.value = atual.busca ?? ''
      carregar()
    },
    { immediate: true },
  )

  obterFiltros()
    .then((f) => (filtros.value = f))
    .catch(() => {
      /* sem as opções os selects ficam vazios; a lista segue funcionando */
    })

  onBeforeUnmount(() => {
    controle?.abort()
    clearTimeout(espera)
  })

  return { busca, consulta, cursos, total, estado, erro, filtros, temFiltro, definir, limpar, recarregar: carregar }
}
