<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <div class="container aviso">
    <h1>Página não encontrada</h1>
    <p>O endereço acessado não existe.</p>
    <RouterLink to="/" class="botao">Ir para o catálogo</RouterLink>
  </div>
</template>
