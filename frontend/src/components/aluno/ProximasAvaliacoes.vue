<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { obterAvaliacoes } from '../../api/cliente'
import { useCarga } from '../../composables/useCarga'
import IconeLinha from '../IconeLinha.vue'

const { dados: avaliacoes, estado, erro, carregar } = useCarga(obterAvaliacoes, [])

const dia = (iso: string) => new Date(iso).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short' }).replace('.', '')
const quando = (iso: string) =>
  new Date(iso).toLocaleString('pt-BR', { weekday: 'long', day: '2-digit', month: 'long', hour: '2-digit', minute: '2-digit' })
</script>

<template>
  <section class="cartao painel-bloco" aria-labelledby="titulo-avaliacoes">
    <h2 id="titulo-avaliacoes" class="cartao__titulo">
      <IconeLinha nome="calendario" /> Próximas avaliações
    </h2>
    <p v-if="estado === 'carregando'" role="status">Carregando…</p>
    <div v-else-if="estado === 'erro'" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao botao--contorno" @click="carregar()">Tentar novamente</button>
    </div>
    <p v-else-if="avaliacoes.length === 0">Nenhuma avaliação marcada nos seus cursos.</p>
    <ol v-else class="avaliacoes">
      <li v-for="a in avaliacoes" :key="a.id">
        <time :datetime="a.em" class="avaliacoes__dia">{{ dia(a.em) }}</time>
        <div>
          <p class="avaliacoes__titulo">{{ a.titulo }}</p>
          <p class="avaliacoes__info">
            <span class="avaliacoes__tipo">{{ a.tipo.rotulo }}</span>
            <RouterLink :to="{ name: 'curso', params: { slug: a.cursoSlug } }">{{ a.cursoNome }}</RouterLink>
            · <span class="primeira-maiuscula">{{ quando(a.em) }}</span>
          </p>
        </div>
      </li>
    </ol>
  </section>
</template>
