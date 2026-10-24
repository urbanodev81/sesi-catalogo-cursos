<script setup lang="ts">
import Carregando from '../Carregando.vue'
import { computed, onBeforeUnmount } from 'vue'
import { obterColegas } from '../../api/cliente'
import { useCarga } from '../../composables/useCarga'
import IconeLinha from '../IconeLinha.vue'

const { dados: colegas, estado, erro, carregar } = useCarga(obterColegas, [])
const online = computed(() => colegas.value.filter((c) => c.online).length)

// A lista se atualiza sozinha a cada minuto, sem trocar para "Carregando…" (o sinal de presença
// também é por minuto). O resumo em role=status anuncia só a contagem, não a lista inteira.
const relogio = setInterval(() => document.visibilityState === 'visible' && carregar(true), 60_000)
onBeforeUnmount(() => clearInterval(relogio))

const iniciais = (nome: string) =>
  nome.split(' ').filter(Boolean).map((p) => p[0]).slice(0, 2).join('').toUpperCase()
</script>

<template>
  <section class="cartao painel-bloco" aria-labelledby="titulo-colegas">
    <h2 id="titulo-colegas" class="cartao__titulo"><IconeLinha nome="pessoas" /> Colegas da turma</h2>
    <Carregando v-if="estado === 'carregando'" />
    <div v-else-if="estado === 'erro'" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao botao--contorno" @click="carregar()">Tentar novamente</button>
    </div>
    <p v-else-if="colegas.length === 0">Ninguém mais nos seus cursos por enquanto.</p>
    <template v-else>
      <p class="painel-bloco__resumo" role="status">
        {{ online === 0 ? 'Nenhum colega online agora' : `${online} ${online === 1 ? 'colega online' : 'colegas online'} agora` }}
      </p>
      <ul class="colegas">
        <li v-for="c in colegas" :key="c.nome">
          <span class="colegas__avatar" aria-hidden="true">
            {{ iniciais(c.nome) }}
            <span :class="['colegas__ponto', { 'colegas__ponto--online': c.online }]" />
          </span>
          <span>
            <span class="colegas__nome">{{ c.nome }}</span>
            <span class="colegas__info">
              <!-- O estado vai em texto, não só na cor do ponto. -->
              <strong :class="{ 'colegas__online': c.online }">{{ c.online ? 'Online' : 'Offline' }}</strong>
              · {{ c.cursos.join(', ') }}
            </span>
          </span>
        </li>
      </ul>
    </template>
  </section>
</template>
