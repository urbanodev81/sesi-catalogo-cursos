import { onBeforeUnmount, ref, watch, type WatchSource } from 'vue'

/** Esperando mais que isto, a tela explica a demora em vez de deixar só o "Carregando…". */
export const DEMORA_MS = 5_000

/**
 * A hospedagem gratuita dorme sem uso, e a primeira resposta depois disso leva de 20 a 50 s. Sem
 * explicação, parece que o site travou. Não é tela de carregamento por cima do conteúdo: é uma frase
 * a mais no mesmo lugar do aviso de carregamento.
 */
export const TEXTO_DEMORA = 'O servidor gratuito está acordando: na primeira visita, isso leva até 50 segundos.'

/** true quando `esperando` ficou verdadeiro por mais de DEMORA_MS seguidos. */
export function useDemora(esperando: WatchSource<boolean>) {
  const demorou = ref(false)
  let relogio: ReturnType<typeof setTimeout> | undefined

  watch(
    esperando,
    (sim) => {
      clearTimeout(relogio)
      demorou.value = false
      if (sim) relogio = setTimeout(() => (demorou.value = true), DEMORA_MS)
    },
    { immediate: true },
  )
  onBeforeUnmount(() => clearTimeout(relogio))

  return demorou
}
