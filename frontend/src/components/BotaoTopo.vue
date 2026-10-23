<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'
import IconeLinha from './IconeLinha.vue'

// "Voltar ao topo" depois de rolar uma tela e meia. Rolagem suave, exceto com movimento reduzido.
const visivel = ref(false)
const aoRolar = () => (visivel.value = window.scrollY > 600)

function subir() {
  const reduzir = window.matchMedia('(prefers-reduced-motion: reduce)').matches
  window.scrollTo({ top: 0, behavior: reduzir ? 'auto' : 'smooth' })
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
