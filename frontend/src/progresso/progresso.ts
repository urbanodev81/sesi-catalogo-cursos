import { ref, toValue, watch, type MaybeRefOrGetter } from 'vue'

/**
 * Onde o progresso do aluno fica guardado. Hoje é o navegador (sem login, o edital não pede);
 * quando o login entrar, uma implementação que chama a API substitui esta sem mexer nas telas.
 * Cada aula é identificada por "modulo.aula" (ex.: "1.2").
 */
export interface RepositorioProgresso {
  concluidas(slug: string): string[]
  salvar(slug: string, concluidas: string[]): void
}

const chave = (slug: string) => `sesi:progresso:${slug}`

/** localStorage pode não existir ou recusar (aba anônima, cota): aí o progresso vale só na sessão. */
export const progressoNoNavegador: RepositorioProgresso = {
  concluidas(slug) {
    try {
      const salvo = JSON.parse(localStorage.getItem(chave(slug)) ?? '[]')
      return Array.isArray(salvo) ? salvo.filter((x) => typeof x === 'string') : []
    } catch {
      return []
    }
  },
  salvar(slug, concluidas) {
    try {
      localStorage.setItem(chave(slug), JSON.stringify(concluidas))
    } catch {
      /* sem armazenamento: segue só em memória */
    }
  },
}

export const chaveAula = (modulo: number, aula: number) => `${modulo}.${aula}`

/** Progresso reativo de um curso; recarrega se o curso mudar (de uma aula para outra de outro curso). */
export function useProgresso(slug: MaybeRefOrGetter<string>, repositorio: RepositorioProgresso = progressoNoNavegador) {
  const concluidas = ref(new Set<string>())
  watch(() => toValue(slug), (s) => (concluidas.value = new Set(repositorio.concluidas(s))), { immediate: true })

  function concluir(modulo: number, aula: number) {
    const k = chaveAula(modulo, aula)
    if (concluidas.value.has(k)) return
    concluidas.value = new Set([...concluidas.value, k])
    repositorio.salvar(toValue(slug), [...concluidas.value])
  }

  const concluida = (modulo: number, aula: number) => concluidas.value.has(chaveAula(modulo, aula))

  return { concluidas, concluir, concluida }
}
