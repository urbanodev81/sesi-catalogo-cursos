<script setup lang="ts">
import { RouterLink } from 'vue-router'
import type { CursoResumo } from '../api/tipos'

defineProps<{ curso: CursoResumo }>()
</script>

<template>
  <article class="card">
    <img class="card__imagem" :src="curso.imagemUrl" alt="" loading="lazy" width="400" height="225" />
    <div class="card__corpo">
      <p class="card__categoria">
        <img :src="`/assets/icones/${curso.categoria.icone}`" alt="" width="20" height="20" />
        {{ curso.categoria.rotulo }}
      </p>
      <h3 class="card__titulo">
        <!-- O link cobre o card inteiro (::after), mas o nome acessível é só o título. -->
        <RouterLink :to="{ name: 'curso', params: { slug: curso.slug } }" class="card__link">
          {{ curso.nomeCompleto }}
        </RouterLink>
      </h3>
      <p class="card__descricao">{{ curso.descricao }}</p>
      <ul class="card__meta">
        <li>{{ curso.modalidade.rotulo }}</li>
        <li>{{ curso.cargaHorariaHoras }} horas</li>
        <li :class="['selo', `selo--${curso.status.valor}`]">{{ curso.status.rotulo }}</li>
      </ul>
    </div>
  </article>
</template>
