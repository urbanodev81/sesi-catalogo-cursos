<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'

// Aviso informativo, não pedido de consentimento: o site só guarda o necessário para funcionar (ver
// /privacidade), então não há "recusar". Aparece até o primeiro "Entendi". Sem localStorage
// (aba anônima), aparece a cada visita, que é o lado seguro do erro.
const CHAVE = 'sesi-aviso-privacidade'
const visivel = ref(lerVisto() !== '1')

function lerVisto() {
  try {
    return localStorage.getItem(CHAVE)
  } catch {
    return null
  }
}

function entendi() {
  visivel.value = false
  try {
    localStorage.setItem(CHAVE, '1')
  } catch {
    // sem armazenamento: o aviso some só nesta visita
  }
}
</script>

<template>
  <section v-if="visivel" class="aviso-privacidade" aria-label="Aviso de privacidade">
    <p>
      Usamos só o necessário para o site funcionar: a sessão de login e as suas preferências.
      Sem rastreamento nem publicidade.
    </p>
    <div class="aviso-privacidade__acoes">
      <RouterLink :to="{ name: 'privacidade' }">Privacidade</RouterLink>
      <button type="button" class="botao botao--claro" @click="entendi">Entendi</button>
    </div>
  </section>
</template>
