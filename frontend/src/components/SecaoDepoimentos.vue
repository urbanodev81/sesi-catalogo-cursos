<script setup lang="ts">
import { ref } from 'vue'
import { useCarrossel } from '../composables/useCarrossel'
import { DEPOIMENTOS } from '../conteudo/institucional'
import CarrosselSetas from './CarrosselSetas.vue'

// Mesma seção na home (cards vinho sobre faixa vinho) e na página do curso (cards brancos
// sobre faixa bege). A cor da faixa é de quem usa; aqui só muda o card e as setas.
const props = defineProps<{ tema: 'vinho' | 'bege' }>()

const trilho = ref<HTMLElement | null>(null)
const carrossel = useCarrossel(trilho)
const id = `titulo-depoimentos-${props.tema}`
</script>

<template>
  <section :class="['secao', `depoimentos--${tema}`]" :aria-labelledby="id">
    <div class="container">
      <p class="secao__sobretitulo">Depoimentos</p>
      <h2 :id="id" class="secao__titulo">Quem fez recomenda</h2>
    </div>

    <!-- Sem link dentro: tabindex para o teclado conseguir rolar a lista. -->
    <ul ref="trilho" class="trilho" tabindex="0" aria-label="Depoimentos de alunos">
      <li v-for="d in DEPOIMENTOS" :key="d.nome" class="depoimento">
        <blockquote>
          <p>“{{ d.texto }}”</p>
        </blockquote>
        <p class="depoimento__autor">
          <strong>{{ d.nome }}</strong>
          {{ d.cargo }}
        </p>
        <p class="depoimento__nota">
          <span class="visualmente-oculto">Nota 5 de 5</span>
          <img v-for="n in 5" :key="n" src="/assets/icones/star.svg" alt="" width="20" height="20" />
        </p>
      </li>
    </ul>

    <div class="container secao__rodape secao__rodape--fim">
      <CarrosselSetas
        rotulo="Depoimentos"
        :claras="tema === 'vinho'"
        :pode-voltar="carrossel.podeVoltar.value"
        :pode-avancar="carrossel.podeAvancar.value"
        @voltar="carrossel.voltar"
        @avancar="carrossel.avancar"
      />
    </div>
  </section>
</template>
