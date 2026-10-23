<script setup lang="ts">
import { onBeforeUnmount, ref, shallowRef, watch } from 'vue'
import { RouterLink } from 'vue-router'
import { ErroApi, obterCurso } from '../api/cliente'
import type { CursoDetalhe } from '../api/tipos'

const props = defineProps<{ slug: string }>()

const detalhe = shallowRef<CursoDetalhe>()
const estado = ref<'carregando' | 'pronto' | 'erro' | 'nao-encontrado'>('carregando')
const erro = ref('')

let controle: AbortController | undefined
async function carregar() {
  controle?.abort()
  controle = new AbortController()
  const sinal = controle.signal
  estado.value = 'carregando'
  try {
    detalhe.value = await obterCurso(props.slug, sinal)
    document.title = `${detalhe.value.curso.nomeCompleto} | SESI Indústria`
    estado.value = 'pronto'
  } catch (e) {
    if (sinal.aborted) return
    if (e instanceof ErroApi && e.naoEncontrado) {
      estado.value = 'nao-encontrado'
      return
    }
    erro.value = e instanceof ErroApi ? e.message : 'Ocorreu um erro inesperado.'
    estado.value = 'erro'
  }
}

watch(() => props.slug, carregar, { immediate: true })
onBeforeUnmount(() => controle?.abort())

const formatarData = (iso: string) =>
  new Date(`${iso}T00:00:00`).toLocaleDateString('pt-BR', { day: '2-digit', month: 'long', year: 'numeric' })
</script>

<template>
  <div class="container detalhe">
    <nav aria-label="Trilha de navegação" class="trilha">
      <RouterLink to="/">Catálogo de cursos</RouterLink>
      <span aria-hidden="true"> / </span>
      <span aria-current="page">{{ detalhe?.curso.nomeBreve ?? 'Curso' }}</span>
    </nav>

    <p v-if="estado === 'carregando'" role="status">Carregando curso…</p>

    <div v-else-if="estado === 'nao-encontrado'" class="aviso">
      <h1>Curso não encontrado</h1>
      <p>O curso procurado não existe ou foi removido do catálogo.</p>
      <RouterLink to="/" class="botao">Ver todos os cursos</RouterLink>
    </div>

    <div v-else-if="estado === 'erro'" class="aviso aviso--erro" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao" @click="carregar">Tentar novamente</button>
    </div>

    <article v-else-if="detalhe" class="curso">
      <img class="curso__imagem" :src="detalhe.curso.imagemUrl" alt="" width="1200" height="675" />
      <p class="card__categoria">
        <img :src="`/assets/icones/${detalhe.curso.categoria.icone}`" alt="" width="20" height="20" />
        {{ detalhe.curso.categoria.rotulo }}
      </p>
      <h1>{{ detalhe.curso.nomeCompleto }}</h1>
      <p class="curso__descricao">{{ detalhe.curso.descricao }}</p>

      <dl class="curso__ficha">
        <div><dt>Nome breve</dt><dd>{{ detalhe.curso.nomeBreve }}</dd></div>
        <div><dt>Modalidade</dt><dd>{{ detalhe.curso.modalidade.rotulo }}</dd></div>
        <div><dt>Carga horária</dt><dd>{{ detalhe.curso.cargaHorariaHoras }} horas</dd></div>
        <div>
          <dt>Status</dt>
          <dd><span :class="['selo', `selo--${detalhe.curso.status.valor}`]">{{ detalhe.curso.status.rotulo }}</span></dd>
        </div>
        <div v-if="detalhe.curso.dataDisponibilizacao">
          <dt>Início do curso</dt>
          <dd><time :datetime="detalhe.curso.dataDisponibilizacao">{{ formatarData(detalhe.curso.dataDisponibilizacao) }}</time></dd>
        </div>
        <div><dt>Público-alvo</dt><dd>{{ detalhe.curso.publicoAlvo }}</dd></div>
      </dl>

      <section aria-labelledby="titulo-conteudo">
        <h2 id="titulo-conteudo">Conteúdo do curso</h2>
        <ol class="modulos">
          <li v-for="m in detalhe.modulos" :key="m.ordem">
            <details :open="m.ordem === 1">
              <summary>Módulo {{ m.ordem }}: {{ m.titulo }}</summary>
              <ol class="aulas">
                <li v-for="a in m.aulas" :key="a.ordem">
                  <strong>{{ a.titulo }}</strong>
                  <p>{{ a.descricao }}</p>
                </li>
              </ol>
            </details>
          </li>
        </ol>
      </section>
    </article>
  </div>
</template>
