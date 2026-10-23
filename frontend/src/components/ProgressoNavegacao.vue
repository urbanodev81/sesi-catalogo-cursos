<script setup lang="ts">
import { onBeforeUnmount, ref } from 'vue'
import { useRouter } from 'vue-router'

// Barra fina no topo durante a troca de página. As telas fora da home são carregadas sob demanda
// (import() no router): no primeiro acesso a cada uma, entre o clique e a tela nova não aparecia nada.
// Barra e não overlay: overlay cobre conteúdo ainda legível, prende o foco e pisca quando a página
// chega rápido. Só aparece depois de 180 ms, para não piscar em navegação instantânea (filtros, âncoras).
// É aria-hidden: o leitor de tela já ouve a troca pelo título da aba e pelo foco no <main>.
const ativa = ref(false)
const largura = ref(0)
let carencia: ReturnType<typeof setTimeout> | undefined
let avanco: ReturnType<typeof setInterval> | undefined

function comecar() {
  if (carencia || avanco) return
  carencia = setTimeout(() => {
    carencia = undefined
    ativa.value = true
    largura.value = 8
    // Nunca chega a 100% sozinha: quem completa é a navegação terminar.
    avanco = setInterval(() => (largura.value += (90 - largura.value) * 0.12), 220)
  }, 180)
}

function parar() {
  clearTimeout(carencia)
  clearInterval(avanco)
  carencia = avanco = undefined
  ativa.value = false
  largura.value = 0
}

const router = useRouter()
const remover = [router.beforeEach(() => comecar()), router.afterEach(parar), router.onError(parar)]
onBeforeUnmount(() => {
  remover.forEach((r) => r())
  parar()
})
</script>

<template>
  <div class="progresso-navegacao" :class="{ 'progresso-navegacao--ativa': ativa }" aria-hidden="true">
    <span :style="{ width: `${largura}%` }" />
  </div>
</template>
