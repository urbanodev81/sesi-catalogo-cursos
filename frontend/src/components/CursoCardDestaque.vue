<script setup lang="ts">
import { RouterLink } from 'vue-router'
import type { CursoResumo } from '../api/tipos'

defineProps<{ curso: CursoResumo }>()
</script>

<template>
  <article class="destaque">
    <img class="destaque__imagem" :src="curso.imagemUrl" alt="" loading="lazy" width="400" height="560" />
    <p :class="['etiqueta', `etiqueta--${curso.categoria.valor}`]">
      <img :src="`/assets/icones/${curso.categoria.icone}`" alt="" width="16" height="16" />
      {{ curso.categoria.rotulo }}
    </p>
    <div class="destaque__texto">
      <h3 class="destaque__titulo">
        <!-- O link cobre o card inteiro (::after), mas o nome acessível é só o título. -->
        <RouterLink :to="{ name: 'curso', params: { slug: curso.slug } }" class="card__link">
          {{ curso.nomeCompleto }}
        </RouterLink>
      </h3>
      <p class="destaque__resumo">{{ curso.resumo }}</p>
    </div>
  </article>
</template>
