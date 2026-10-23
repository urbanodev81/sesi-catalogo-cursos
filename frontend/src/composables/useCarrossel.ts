import { onBeforeUnmount, ref, watch, type Ref } from 'vue'

/**
 * Carrossel sobre uma lista com overflow horizontal e scroll-snap: quem rola é o navegador
 * (toque, trackpad, teclado), as setas só chamam scrollBy de um card. Sem biblioteca: são
 * ~40 linhas e o comportamento nativo já é acessível.
 */
export function useCarrossel(trilho: Ref<HTMLElement | null>) {
  const podeVoltar = ref(false)
  const podeAvancar = ref(false)

  function atualizar() {
    const el = trilho.value
    if (!el) return
    podeVoltar.value = el.scrollLeft > 1
    podeAvancar.value = el.scrollLeft + el.clientWidth < el.scrollWidth - 1
  }

  function passo(direcao: 1 | -1) {
    const el = trilho.value
    const card = el?.firstElementChild as HTMLElement | null
    if (!el || !card) return
    const gap = parseFloat(getComputedStyle(el).columnGap) || 0
    const suave = !matchMedia('(prefers-reduced-motion: reduce)').matches
    el.scrollBy({ left: direcao * (card.offsetWidth + gap), behavior: suave ? 'smooth' : 'auto' })
  }

  // watch no elemento, não onMounted: a lista pode só aparecer depois (ex.: erro → tentar de novo).
  const observador = new ResizeObserver(atualizar)
  watch(
    trilho,
    (el, antigo) => {
      antigo?.removeEventListener('scroll', atualizar)
      observador.disconnect()
      if (!el) return
      el.addEventListener('scroll', atualizar, { passive: true })
      observador.observe(el)
      atualizar()
    },
    { flush: 'post' },
  )
  onBeforeUnmount(() => {
    trilho.value?.removeEventListener('scroll', atualizar)
    observador.disconnect()
  })

  return { podeVoltar, podeAvancar, atualizar, voltar: () => passo(-1), avancar: () => passo(1) }
}
