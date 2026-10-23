<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { pausarAnimacoes } from '../acessibilidade/preferencias'
import IconeLinha from './IconeLinha.vue'

// "Voltar ao topo" depois de rolar uma tela e meia.
const visivel = ref(false)
const aoRolar = () => (visivel.value = window.scrollY > 600)

// A rolagem é animada aqui, e não por behavior: 'smooth': com "efeitos de animação" desligado no
// Windows o Chrome responde prefers-reduced-motion e desliga o smooth nativo, e o clique virava salto.
// Anima mesmo assim de propósito: é um deslocamento que a pessoa pediu ao clicar, não tela que se mexe
// sozinha (o que a preferência do sistema protege continua desligado no CSS). Duração proporcional à distância.
function rolarAoTopo() {
  const inicio = window.scrollY
  // "Parar animações" do painel de acessibilidade é pedido explícito: aí a subida é instantânea.
  if (pausarAnimacoes.value) return window.scrollTo({ top: 0, behavior: 'instant' })
  const duracao = Math.min(1300, Math.max(450, inicio * 0.45))
  const suavizar = (t: number) => (t < 0.5 ? 2 * t * t : 1 - (-2 * t + 2) ** 2 / 2)
  let comeco: number | undefined
  const passo = (agora: number) => {
    comeco ??= agora
    const parte = Math.min(1, (agora - comeco) / duracao)
    window.scrollTo({ top: inicio * (1 - suavizar(parte)), behavior: 'instant' })
    if (parte < 1) requestAnimationFrame(passo)
  }
  requestAnimationFrame(passo)
}

function subir() {
  rolarAoTopo()
  // O botão some ao chegar no topo; o foco do teclado não pode sumir com ele. Vai para o início
  // do conteúdo, como na troca de página (preventScroll: a rolagem acima continua valendo).
  document.getElementById('conteudo')?.focus({ preventScroll: true })
}

onMounted(() => {
  aoRolar()
  window.addEventListener('scroll', aoRolar, { passive: true })
})
onBeforeUnmount(() => window.removeEventListener('scroll', aoRolar))
</script>

<template>
  <Transition name="aparecer">
    <button v-if="visivel" type="button" class="botao-topo" aria-label="Voltar ao topo" title="Voltar ao topo" @click="subir">
      <IconeLinha nome="subir" />
    </button>
  </Transition>
</template>
