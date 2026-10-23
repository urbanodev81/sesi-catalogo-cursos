<script setup lang="ts">
import { nextTick, watch } from 'vue'
import { RouterView, useRoute, useRouter } from 'vue-router'
import PainelAcessibilidade from './acessibilidade/PainelAcessibilidade.vue'
import BotaoTopo from './components/BotaoTopo.vue'
import CabecalhoSite from './components/CabecalhoSite.vue'
import ProgressoNavegacao from './components/ProgressoNavegacao.vue'
import RodapeSite from './components/RodapeSite.vue'
import { manterPresenca } from './conta/presenca'
import { carregarSessao } from './conta/sessao'

// Troca de página numa SPA não move o foco: o leitor de tela continuaria no link
// clicado. Levamos o foco ao <main> (só na troca de caminho, não a cada filtro).
// preventScroll: quem decide a rolagem é o router (topo, posição salva ou âncora).
// Só depois de isReady: a primeira resolução da rota (de "/" para a URL aberta) não é uma
// troca de página, e mover o foco ali pularia o "Pular para o conteúdo".
const route = useRoute()
carregarSessao()
manterPresenca()
useRouter()
  .isReady()
  .then(() =>
    watch(
      () => route.path,
      async () => {
        await nextTick()
        document.getElementById('conteudo')?.focus({ preventScroll: true })
      },
    ),
  )
</script>

<template>
  <ProgressoNavegacao />
  <a class="pular" href="#conteudo">Pular para o conteúdo</a>
  <!-- A tela de aula é a do ambiente de estudo (Figma): sem o cabeçalho e o rodapé do site. -->
  <CabecalhoSite v-if="!route.meta.semCasca" />
  <main id="conteudo" tabindex="-1">
    <RouterView />
  </main>
  <RodapeSite v-if="!route.meta.semCasca" />
  <BotaoTopo />
  <PainelAcessibilidade />
</template>
