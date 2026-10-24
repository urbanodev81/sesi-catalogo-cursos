import { onBeforeUnmount, ref, watch, type Ref } from 'vue'

/** Sem nenhum movimento, toque, tecla ou rolagem por este tempo, o painel aberto se fecha sozinho. */
export const FECHAR_APOS_MS = 60_000

// Um painel flutuante por vez: os dois (acessibilidade e ajuda) cobrem boa parte da tela no celular.
const painelAberto = ref<string>()

/** Outra coisa toma a tela (o tour): fecha o painel aberto. `undefined` devolve a vez aos painéis. */
export function reservarTela(dono: string | undefined) {
  painelAberto.value = dono
}

const EVENTOS = ['pointermove', 'pointerdown', 'keydown', 'wheel', 'scroll', 'touchstart'] as const

/**
 * Regras comuns aos painéis dos cantos: abrir um fecha o outro, e o aberto fecha sozinho depois de
 * um tempo sem uso. Fechar sozinho só devolve o foco ao botão se o foco estava dentro do painel
 * (senão ele cairia no <body>); fora disso, não mexe no foco de quem está em outra parte da página.
 */
export function usePainelFlutuante(
  nome: string,
  aberto: Ref<boolean>,
  botao: Ref<HTMLElement | undefined>,
  painel: Ref<HTMLElement | undefined>,
) {
  let espera: ReturnType<typeof setTimeout> | undefined

  function fecharPorInatividade() {
    if (painel.value?.contains(document.activeElement)) botao.value?.focus({ preventScroll: true })
    aberto.value = false
  }

  function rearmar() {
    clearTimeout(espera)
    espera = setTimeout(fecharPorInatividade, FECHAR_APOS_MS)
  }

  function parar() {
    clearTimeout(espera)
    EVENTOS.forEach((e) => window.removeEventListener(e, rearmar, true))
  }

  watch(aberto, (agora) => {
    if (!agora) {
      parar()
      if (painelAberto.value === nome) painelAberto.value = undefined
      return
    }
    painelAberto.value = nome
    EVENTOS.forEach((e) => window.addEventListener(e, rearmar, { capture: true, passive: true }))
    rearmar()
  })

  watch(painelAberto, (outro) => {
    if (aberto.value && outro !== nome) aberto.value = false
  })

  onBeforeUnmount(parar)
}
