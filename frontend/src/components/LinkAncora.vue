<script setup lang="ts">
import { RouterLink } from 'vue-router'

// Link para uma seção da home (/#sobre). Com RouterLink comum, o router ignora o hash e
// marcaria aria-current="page" nele e no "Início" ao mesmo tempo.
defineProps<{ ancora: string }>()
</script>

<template>
  <RouterLink v-slot="{ href, navigate }" :to="{ path: '/', hash: `#${ancora}` }" custom>
    <a :href="href" @click="navigate"><slot /></a>
  </RouterLink>
</template>
