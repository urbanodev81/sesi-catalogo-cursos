<script setup lang="ts">
import Carregando from '../Carregando.vue'
import { obterMateriais, urlDoMaterial } from '../../api/cliente'
import { useCarga } from '../../composables/useCarga'
import IconeLinha from '../IconeLinha.vue'

const { dados: materiais, estado, erro, carregar } = useCarga(obterMateriais, [])

const data = (iso: string) => new Date(iso).toLocaleDateString('pt-BR')
const tamanho = (bytes: number) =>
  bytes < 1024 * 1024 ? `${Math.max(1, Math.round(bytes / 1024))} KB` : `${(bytes / 1024 / 1024).toFixed(1)} MB`
</script>

<template>
  <section class="cartao painel-bloco" aria-labelledby="titulo-materiais">
    <h2 id="titulo-materiais" class="cartao__titulo"><IconeLinha nome="tela" /> Materiais das turmas</h2>
    <Carregando v-if="estado === 'carregando'" />
    <div v-else-if="estado === 'erro'" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao botao--contorno" @click="carregar()">Tentar novamente</button>
    </div>
    <p v-else-if="materiais.length === 0">Os professores ainda não publicaram materiais nos seus cursos.</p>
    <ul v-else class="materiais">
      <li v-for="m in materiais" :key="m.id">
        <span>
          <a :href="urlDoMaterial(m.id)" download>{{ m.titulo }}</a>
          <small>{{ m.cursoNome }} · {{ tamanho(m.tamanho) }} · {{ data(m.em) }}</small>
        </span>
      </li>
    </ul>
  </section>
</template>
