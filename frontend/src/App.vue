<script setup lang="ts">
import { nextTick, watch } from 'vue'
import { RouterView, useRoute } from 'vue-router'
import CabecalhoSite from './components/CabecalhoSite.vue'
import RodapeSite from './components/RodapeSite.vue'

// Troca de página numa SPA não move o foco: o leitor de tela continuaria no link
// clicado. Levamos o foco ao <main> (só na troca de caminho, não a cada filtro).
// preventScroll: quem decide a rolagem é o router (topo, posição salva ou âncora).
const route = useRoute()
watch(
  () => route.path,
  async (_novo, anterior) => {
    if (anterior === undefined) return
    await nextTick()
    document.getElementById('conteudo')?.focus({ preventScroll: true })
  },
)
</script>

<template>
  <a class="pular" href="#conteudo">Pular para o conteúdo</a>
  <!-- A tela de aula é a do ambiente de estudo (Figma): sem o cabeçalho e o rodapé do site. -->
  <CabecalhoSite v-if="!route.meta.semCasca" />
  <main id="conteudo" tabindex="-1">
    <RouterView />
  </main>
  <RodapeSite v-if="!route.meta.semCasca" />
</template>
