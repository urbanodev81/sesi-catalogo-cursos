<script setup lang="ts">
// Ícones de traço das linhas de informação (início, certificação...). O pacote do SESI não
// traz esses ícones pequenos, então são desenhados aqui no mesmo estilo: traço de 2px, 24×24.
defineProps<{ nome: 'calendario' | 'medalha' | 'relogio' | 'tela' | 'pessoas' | 'busca' | 'voltar' }>()
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
    <template v-else>
      <path d="M20 12H4M10 6l-6 6 6 6" />
    </template>
  </svg>
</template>
