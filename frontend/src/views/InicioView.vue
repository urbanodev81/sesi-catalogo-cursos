<script setup lang="ts">
import Carregando from '../components/Carregando.vue'
import { nextTick, onBeforeUnmount, onMounted, ref, shallowRef } from 'vue'
import { RouterLink } from 'vue-router'
import { ErroApi, listarCursos } from '../api/cliente'
import type { CursoResumo } from '../api/tipos'
import CarrosselSetas from '../components/CarrosselSetas.vue'
import CursoCardDestaque from '../components/CursoCardDestaque.vue'
import SecaoDepoimentos from '../components/SecaoDepoimentos.vue'
import { useCarrossel } from '../composables/useCarrossel'
import { CURSOS_DESTAQUE, DIFERENCIAIS, NUMEROS } from '../conteudo/institucional'

const destaques = shallowRef<CursoResumo[]>([])
const estado = ref<'carregando' | 'pronto' | 'erro'>('carregando')
const erro = ref('')

const trilhoCursos = ref<HTMLElement | null>(null)
const cursosCarrossel = useCarrossel(trilhoCursos)

let controle: AbortController | undefined
async function carregar() {
  controle?.abort()
  controle = new AbortController()
  const sinal = controle.signal
  estado.value = 'carregando'
  try {
    // 12 cursos: buscar a lista e escolher na tela sai mais simples que um endpoint só de destaques.
    const { itens } = await listarCursos({}, sinal)
    const porSlug = new Map(itens.map((c) => [c.slug, c]))
    destaques.value = CURSOS_DESTAQUE.flatMap((d) => porSlug.get(d.slug) ?? [])
    estado.value = 'pronto'
    await nextTick()
    cursosCarrossel.atualizar()
  } catch (e) {
    if (sinal.aborted) return
    erro.value = e instanceof ErroApi ? e.message : 'Ocorreu um erro inesperado.'
    estado.value = 'erro'
  }
}

onMounted(carregar)
onBeforeUnmount(() => controle?.abort())
</script>

<template>
  <section class="inicio-hero" aria-labelledby="titulo-inicio">
    <div class="container">
      <h1 id="titulo-inicio">Impulsione sua carreira na indústria</h1>
      <p>Amplie seus conhecimentos com excelência na melhor plataforma de estudos da indústria brasileira</p>
      <RouterLink :to="{ name: 'catalogo' }" class="botao botao--claro">
        Conhecer cursos
        <img src="/assets/icones/arrow-navigation-1.svg" alt="" width="24" height="24" />
      </RouterLink>
    </div>
  </section>

  <section class="secao" aria-labelledby="titulo-principais">
    <div class="container">
      <h2 id="titulo-principais" class="secao__titulo">Principais cursos</h2>
    </div>

    <Carregando v-if="estado === 'carregando'" class="container" texto="Carregando cursos…" oculto />
    <div v-if="estado === 'erro'" class="container aviso aviso--erro" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao" @click="carregar">Tentar novamente</button>
    </div>

    <ul v-else ref="trilhoCursos" class="trilho" :aria-busy="estado === 'carregando'" aria-label="Cursos em destaque">
      <template v-if="estado === 'carregando'">
        <li v-for="n in 5" :key="n" class="destaque destaque--esqueleto" aria-hidden="true" />
      </template>
      <template v-else>
        <li v-for="c in destaques" :key="c.id"><CursoCardDestaque :curso="c" /></li>
        <li>
          <RouterLink :to="{ name: 'catalogo' }" class="destaque destaque--todos">
            Todos os cursos
            <img src="/assets/icones/arrow-navigation-1.svg" alt="" width="32" height="32" />
          </RouterLink>
        </li>
      </template>
    </ul>

    <div class="container secao__rodape">
      <RouterLink :to="{ name: 'catalogo' }" class="botao botao--contorno">Ver todos os cursos</RouterLink>
      <CarrosselSetas
        rotulo="Cursos"
        :pode-voltar="cursosCarrossel.podeVoltar.value"
        :pode-avancar="cursosCarrossel.podeAvancar.value"
        @voltar="cursosCarrossel.voltar"
        @avancar="cursosCarrossel.avancar"
      />
    </div>
  </section>

  <section id="sobre" class="secao container impacto" aria-labelledby="titulo-impacto">
    <div class="impacto__chamada">
      <h2 id="titulo-impacto" class="secao__titulo">Impacto transformador</h2>
      <p>Resultados que demonstram o sucesso da nossa metodologia de capacitação contínua para a indústria</p>
    </div>

    <div>
      <!-- dt antes do dd no HTML (o leitor lê "Empresas parceiras: 500+"); o CSS inverte na tela. -->
      <dl class="impacto__numeros">
        <div v-for="n in NUMEROS" :key="n.rotulo">
          <dt>{{ n.rotulo }}</dt>
          <dd>{{ n.valor }}</dd>
        </div>
      </dl>

      <ul id="diferenciais" class="diferenciais">
        <li v-for="d in DIFERENCIAIS.slice(0, 3)" :key="d.titulo">
          <img :src="`/assets/icones/${d.icone}-red.svg`" alt="" width="48" height="48" />
          <h3>{{ d.titulo }}</h3>
          <p>{{ d.texto }}</p>
        </li>
      </ul>
    </div>
  </section>

  <div class="faixa-vinho">
    <SecaoDepoimentos tema="vinho" />

    <section class="secao container chamada" aria-labelledby="titulo-chamada">
      <div>
        <h2 id="titulo-chamada" class="secao__titulo">Pronto para transformar sua carreira?</h2>
        <p>
          Junte-se a milhares de profissionais que já estão transformando a indústria através dos nossos cursos.
          Invista no seu futuro hoje mesmo!
        </p>
        <RouterLink :to="{ name: 'catalogo' }" class="botao botao--claro">
          Matricule-se
          <img src="/assets/icones/arrow-navigation-1.svg" alt="" width="24" height="24" />
        </RouterLink>
      </div>
      <img class="chamada__imagem" src="/assets/imagens/video-thumb.webp" alt="" loading="lazy" width="1200" height="675" />
    </section>
  </div>
</template>
