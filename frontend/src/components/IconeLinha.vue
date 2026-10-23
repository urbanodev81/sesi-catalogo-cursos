<script setup lang="ts">
// Ícones de traço das linhas de informação (início, certificação...). O pacote do SESI não
// traz esses ícones pequenos, então são desenhados aqui no mesmo estilo: traço de 2px, 24×24.
defineProps<{ nome: 'calendario' | 'medalha' | 'relogio' | 'tela' | 'pessoas' | 'busca' | 'subir' | 'voltar'
    | 'acessibilidade' | 'fechar' | 'mais' | 'menos' | 'pausa' | 'libras' | 'expandir' }>()
</script>

<template>
  <svg class="icone-linha" viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <template v-if="nome === 'calendario'">
      <rect x="3.5" y="5" width="17" height="15" rx="2" />
      <path d="M3.5 10h17M8 3v4M16 3v4" />
    </template>
    <template v-else-if="nome === 'medalha'">
      <circle cx="12" cy="9" r="5.5" />
      <path d="m8.5 13.5-1.5 7 5-2.5 5 2.5-1.5-7" />
    </template>
    <template v-else-if="nome === 'relogio'">
      <circle cx="12" cy="12" r="8.5" />
      <path d="M12 7.5V12l3 2" />
    </template>
    <template v-else-if="nome === 'tela'">
      <rect x="3" y="4.5" width="18" height="12" rx="1.5" />
      <path d="M8.5 20h7M12 16.5V20" />
    </template>
    <template v-else-if="nome === 'pessoas'">
      <circle cx="9" cy="8.5" r="3.5" />
      <path d="M2.5 20c.5-3.5 3-5.5 6.5-5.5s6 2 6.5 5.5M16 5.2a3.5 3.5 0 0 1 0 6.6M18 14.8c2 .7 3.2 2.4 3.5 5.2" />
    </template>
    <template v-else-if="nome === 'busca'">
      <circle cx="10.5" cy="10.5" r="6.5" />
      <path d="m15.5 15.5 5 5" />
    </template>
    <template v-else-if="nome === 'acessibilidade'">
      <circle cx="12" cy="4.5" r="1.8" />
      <path d="M5 8.5l7 1.5 7-1.5M12 10v4.5m0 0-3 6m3-6 3 6" />
    </template>
    <template v-else-if="nome === 'fechar'">
      <path d="M6 6l12 12M18 6 6 18" />
    </template>
    <template v-else-if="nome === 'mais'">
      <path d="M12 5v14M5 12h14" />
    </template>
    <template v-else-if="nome === 'menos'">
      <path d="M5 12h14" />
    </template>
    <template v-else-if="nome === 'pausa'">
      <path d="M9 5v14M15 5v14" />
    </template>
    <template v-else-if="nome === 'libras'">
      <path d="M8 13V5.5a1.5 1.5 0 0 1 3 0V11m0-1.5a1.5 1.5 0 0 1 3 0V11m0-1a1.5 1.5 0 0 1 3 0v4.5a6 6 0 0 1-6 6h-.5a6 6 0 0 1-4.6-2.2L4 15.5a1.5 1.5 0 0 1 2.3-1.9L8 15.5" />
    </template>
    <template v-else-if="nome === 'expandir'">
      <path d="m6 9 6 6 6-6" />
    </template>
    <template v-else-if="nome === 'subir'">
      <path d="M12 20V4M6 10l6-6 6 6" />
    </template>
    <template v-else>
      <path d="M20 12H4M10 6l-6 6 6 6" />
    </template>
  </svg>
</template>
