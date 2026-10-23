<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
  <!-- Provisória: o login do aluno entra depois dos requisitos obrigatórios. -->
  <div class="container aviso">
    <h1>Área do aluno</h1>
    <p>Em breve você vai acompanhar aqui o seu progresso nos cursos.</p>
    <RouterLink :to="{ name: 'catalogo' }" class="botao">Ver os cursos</RouterLink>
  </div>
</template>
