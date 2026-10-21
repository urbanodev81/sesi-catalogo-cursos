<script setup lang="ts">
import { nextTick, watch } from 'vue'
import { RouterLink, RouterView, useRoute } from 'vue-router'

// Troca de página numa SPA não move o foco: o leitor de tela continuaria no link
// clicado. Levamos o foco ao <main> (só na troca de caminho, não a cada filtro).
const route = useRoute()
watch(
  () => route.path,
  async (_novo, anterior) => {
    if (anterior === undefined) return
    await nextTick()
    document.getElementById('conteudo')?.focus()
  },
)
</script>

<template>
  <a class="pular" href="#conteudo">Pular para o conteúdo</a>

  <header class="topo">
    <div class="container topo__barra">
      <RouterLink to="/" class="topo__marca" aria-label="SESI Indústria, ir para o catálogo de cursos">
        <img src="/assets/logos/logo-sesi-industria-primary.svg" alt="" width="160" height="40" />
      </RouterLink>
    </div>
  </header>

  <main id="conteudo" tabindex="-1">
    <RouterView />
  </main>

  <footer class="rodape">
    <div class="container rodape__barra">
      <img src="/assets/logos/logo-sesi-industria-white.svg" alt="SESI Indústria" width="140" height="35" />
      <ul class="rodape__redes" aria-label="Redes sociais">
        <li><a href="https://www.facebook.com/" rel="noopener" target="_blank"><img src="/assets/icones/icon-facebook-24px.svg" alt="Facebook (abre em nova aba)" width="24" height="24" /></a></li>
        <li><a href="https://www.instagram.com/" rel="noopener" target="_blank"><img src="/assets/icones/icon-instagram-24px.svg" alt="Instagram (abre em nova aba)" width="24" height="24" /></a></li>
        <li><a href="https://www.linkedin.com/" rel="noopener" target="_blank"><img src="/assets/icones/icon-linkedIn-24px.svg" alt="LinkedIn (abre em nova aba)" width="24" height="24" /></a></li>
      </ul>
    </div>
  </footer>
</template>
