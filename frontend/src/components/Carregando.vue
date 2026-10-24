<script setup lang="ts">
import { TEXTO_DEMORA, useDemora } from '../composables/useDemora'

// Aviso de carregamento das telas. Montado só enquanto carrega (v-if de quem usa), então a contagem
// da demora recomeça a cada carga. `oculto`: o texto base vai só para o leitor de tela (a home já tem
// os cards em esqueleto), e só a frase da demora aparece.
withDefaults(defineProps<{ texto?: string; oculto?: boolean }>(), { texto: 'Carregando…', oculto: false })
const demorou = useDemora(() => true)
</script>

<template>
  <!-- Oculto e ainda sem demora: sem margem, para não abrir um vão vazio na home. -->
  <p role="status" :class="['carregando', { 'carregando--vazio': oculto && !demorou }]">
    <span :class="{ 'visualmente-oculto': oculto }">{{ texto }}</span>
    <template v-if="demorou"> {{ TEXTO_DEMORA }}</template>
  </p>
</template>
