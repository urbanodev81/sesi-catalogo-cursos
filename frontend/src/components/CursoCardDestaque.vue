<script setup lang="ts">
import { RouterLink } from 'vue-router'
import type { CursoResumo } from '../api/tipos'

// `nivel`: h3 no carrossel da home (abaixo de "Principais cursos"), h2 na listagem (logo abaixo do h1).
// `ficha`: a listagem mostra modalidade, carga horária e status, que são os filtros da página.
withDefaults(defineProps<{ curso: CursoResumo; nivel?: 2 | 3; ficha?: boolean }>(), { nivel: 3, ficha: false })
</script>

<template>
  <article :class="['destaque', { 'destaque--ficha': ficha }]">
    <img class="destaque__imagem" :src="curso.imagemUrl" alt="" loading="lazy" width="400" height="560" />
    <p :class="['etiqueta', `etiqueta--${curso.categoria.valor}`]">
      <img :src="`/assets/icones/${curso.categoria.icone}`" alt="" width="16" height="16" />
      {{ curso.categoria.rotulo }}
    </p>
    <div class="destaque__texto">
      <component :is="`h${nivel}`" class="destaque__titulo">
        <!-- O link cobre o card inteiro (::after), mas o nome acessível é só o título. -->
        <RouterLink :to="{ name: 'curso', params: { slug: curso.slug } }" class="card__link">
          {{ curso.nomeCompleto }}
        </RouterLink>
      </component>
      <p class="destaque__resumo">{{ curso.resumo }}</p>
      <ul v-if="ficha" class="destaque__ficha">
        <li>{{ curso.modalidade.rotulo }}</li>
        <li>{{ curso.cargaHorariaHoras }} horas</li>
        <li :class="['destaque__status', `destaque__status--${curso.status.valor}`]">{{ curso.status.rotulo }}</li>
      </ul>
    </div>
  </article>
</template>
