import { onBeforeUnmount, ref, shallowRef, type Ref } from 'vue'
import { ErroApi } from '../api/cliente'

/**
 * Carregamento de um bloco da tela com os três estados (carregando, pronto, erro). Cada bloco do
 * painel do aluno carrega sozinho: se um falhar, os outros continuam na tela.
 */
export function useCarga<T>(buscar: (sinal: AbortSignal) => Promise<T>, inicial: T) {
  const dados = shallowRef(inicial) as Ref<T>
  const estado = ref<'carregando' | 'pronto' | 'erro'>('carregando')
  const erro = ref('')
  let controle: AbortController | undefined

  async function carregar(silencioso = false) {
    controle?.abort()
    controle = new AbortController()
    const sinal = controle.signal
    if (!silencioso) estado.value = 'carregando'
    try {
      dados.value = await buscar(sinal)
      estado.value = 'pronto'
    } catch (e) {
      if (sinal.aborted) return
      erro.value = e instanceof ErroApi ? e.message : 'Ocorreu um erro inesperado.'
      estado.value = 'erro'
    }
  }

  carregar()
  onBeforeUnmount(() => controle?.abort())
  return { dados, estado, erro, carregar }
}
