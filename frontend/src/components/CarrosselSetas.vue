<script setup lang="ts">
defineProps<{ podeVoltar: boolean; podeAvancar: boolean; rotulo: string; claras?: boolean }>()
defineEmits<{ voltar: []; avancar: [] }>()
</script>

<template>
  <div :class="['setas', { 'setas--claras': claras }]">
    <button type="button" class="setas__botao" :disabled="!podeVoltar" :aria-label="`${rotulo}: anteriores`" @click="$emit('voltar')">
      <img src="/assets/icones/arrow-navigation.svg" alt="" width="48" height="48" />
    </button>
    <button type="button" class="setas__botao" :disabled="!podeAvancar" :aria-label="`${rotulo}: próximos`" @click="$emit('avancar')">
      <img src="/assets/icones/arrow-navigation-1.svg" alt="" width="48" height="48" />
    </button>
  </div>
</template>
