<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import LinkAncora from './LinkAncora.vue'

const route = useRoute()
const menuAberto = ref(false)
const rolou = ref(false)

// No Figma o cabeçalho é transparente sobre o hero da home e fica branco ao rolar;
// nas outras páginas não há hero, então ele já nasce branco.
const transparente = computed(() => route.name === 'inicio' && !rolou.value && !menuAberto.value)

const aoRolar = () => (rolou.value = window.scrollY > 24)
onMounted(() => {
  window.addEventListener('scroll', aoRolar, { passive: true })
  aoRolar()
})
onBeforeUnmount(() => window.removeEventListener('scroll', aoRolar))

watch(() => route.fullPath, () => (menuAberto.value = false))

const botaoMenu = ref<HTMLButtonElement>()
function fecharComEsc(e: KeyboardEvent) {
  if (e.key !== 'Escape' || !menuAberto.value) return
  menuAberto.value = false
  botaoMenu.value?.focus()
}
</script>

<template>
  <header :class="['topo', { 'topo--transparente': transparente }]" @keydown="fecharComEsc">
    <div class="container topo__barra">
      <RouterLink to="/" class="topo__marca" aria-label="SESI Indústria, página inicial">
        <img v-if="transparente" src="/assets/logos/logo-sesi-industria-white.svg" alt="" width="188" height="19" />
        <img v-else src="/assets/logos/logo-sesi-industria-primary.svg" alt="" width="188" height="19" />
      </RouterLink>

      <button
        ref="botaoMenu"
        type="button"
        class="topo__menu"
        :aria-expanded="menuAberto"
        aria-controls="menu-principal"
        @click="menuAberto = !menuAberto"
      >
        <span class="topo__hamburguer" aria-hidden="true" />
        Menu
      </button>

      <nav id="menu-principal" :class="['topo__nav', { 'topo__nav--aberto': menuAberto }]" aria-label="Principal">
        <ul>
          <li><RouterLink to="/">Início</RouterLink></li>
          <li><RouterLink :to="{ name: 'catalogo' }">Cursos</RouterLink></li>
          <li><LinkAncora ancora="sobre">Sobre</LinkAncora></li>
          <li><RouterLink :to="{ name: 'area-do-aluno' }" class="topo__aluno">Área do aluno</RouterLink></li>
        </ul>
      </nav>
    </div>
  </header>
</template>
