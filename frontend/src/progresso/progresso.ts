import { computed, ref, toValue, watch, type MaybeRefOrGetter } from 'vue'
import { concluirAula, matricular, obterProgresso } from '../api/cliente'
import { useSessao } from '../conta/sessao'

/**
 * Onde o progresso do aluno fica guardado. Visitante: no navegador. Aluno logado: na API.
 * As telas só conhecem esta interface. Cada aula é identificada por "modulo.aula" (ex.: "1.2").
 */
export interface RepositorioProgresso {
  /** Chamado ao abrir a tela de aula; devolve as aulas já concluídas do curso. */
  abrir(slug: string): Promise<string[]>
  concluir(slug: string, modulo: number, aula: number): Promise<void>
}

const chave = (slug: string) => `sesi:progresso:${slug}`

function lerDoNavegador(slug: string): string[] {
  try {
    const salvo = JSON.parse(localStorage.getItem(chave(slug)) ?? '[]')
    return Array.isArray(salvo) ? salvo.filter((x) => typeof x === 'string') : []
  } catch {
    return []
  }
}

/** localStorage pode não existir ou recusar (aba anônima, cota): aí o progresso vale só na sessão. */
export const progressoNoNavegador: RepositorioProgresso = {
  async abrir(slug) {
    return lerDoNavegador(slug)
  },
  async concluir(slug, modulo, aula) {
    const concluidas = new Set(lerDoNavegador(slug)).add(chaveAula(modulo, aula))
    try {
      localStorage.setItem(chave(slug), JSON.stringify([...concluidas]))
    } catch {
      /* sem armazenamento: segue só em memória */
    }
  },
}

/** Aluno logado: abrir a aula de um curso com inscrições abertas já matricula (é o "Matricule-se"). */
export const progressoNaApi: RepositorioProgresso = {
  async abrir(slug) {
    const progresso = await obterProgresso(slug)
    if (!progresso.matriculado) {
      // 409 = curso em breve ou encerrado: o aluno lê a aula, mas não acumula progresso.
      await matricular(slug).catch(() => undefined)
    }
    return progresso.concluidas
  },
  concluir: (slug, modulo, aula) => concluirAula(slug, { modulo, aula }),
}

export const chaveAula = (modulo: number, aula: number) => `${modulo}.${aula}`

/** Progresso reativo de um curso; recarrega se o curso mudar ou se a sessão mudar. */
export function useProgresso(slug: MaybeRefOrGetter<string>) {
  const { usuario } = useSessao()
  // undefined enquanto a sessão não respondeu: esperar evita ler do navegador e logo depois da API.
  const repositorio = computed(() =>
    usuario.value === undefined ? undefined : usuario.value?.papel === 'aluno' ? progressoNaApi : progressoNoNavegador,
  )
  const concluidas = ref(new Set<string>())
  const erro = ref('')

  watch(
    [() => toValue(slug), repositorio],
    async ([s, repo]) => {
      if (!repo) return
      erro.value = ''
      try {
        concluidas.value = new Set(await repo.abrir(s))
      } catch {
        concluidas.value = new Set()
        erro.value = 'Não foi possível carregar o seu progresso.'
      }
    },
    { immediate: true },
  )

  async function concluir(modulo: number, aula: number) {
    const k = chaveAula(modulo, aula)
    if (concluidas.value.has(k)) return
    await (repositorio.value ?? progressoNoNavegador).concluir(toValue(slug), modulo, aula)
    concluidas.value = new Set([...concluidas.value, k])
  }

  const concluida = (modulo: number, aula: number) => concluidas.value.has(chaveAula(modulo, aula))

  return { concluidas, concluir, concluida, erro }
}
