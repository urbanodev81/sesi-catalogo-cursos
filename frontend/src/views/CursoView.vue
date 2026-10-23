<script setup lang="ts">
import { computed, onBeforeUnmount, ref, shallowRef, watch } from 'vue'
import { RouterLink } from 'vue-router'
import { ErroApi, obterCurso } from '../api/cliente'
import type { CursoDetalhe } from '../api/tipos'
import IconeLinha from '../components/IconeLinha.vue'
import SecaoDepoimentos from '../components/SecaoDepoimentos.vue'
import { AULAS_E_AVALIACOES, CONTATO, DIFERENCIAIS, DUVIDAS_FREQUENTES } from '../conteudo/institucional'

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

// Data como no Figma ("05/01/2026"). T00:00 local: sem isso o fuso de SP mostraria o dia anterior.
const formatarData = (iso: string) => new Date(`${iso}T00:00:00`).toLocaleDateString('pt-BR')

// Sem login nem integração com o Moodle, "Matricule-se" leva à primeira aula (é o fluxo do
// protótipo: curso → aula). Só existe para curso com inscrições abertas.
const abertas = computed(() => detalhe.value?.curso.status.valor === 'inscricoes-abertas')
const primeiraAula = computed(() => ({ name: 'aula', params: { slug: props.slug, modulo: 1, aula: 1 } }))
const totalAulas = computed(() => detalhe.value?.modulos.reduce((soma, m) => soma + m.aulas.length, 0) ?? 0)
</script>

<template>
  <div v-if="estado === 'carregando'" class="container detalhe-estado">
    <p role="status">Carregando curso…</p>
  </div>

  <div v-else-if="estado === 'nao-encontrado'" class="container detalhe-estado">
    <div class="aviso">
      <h1>Curso não encontrado</h1>
      <p>O curso procurado não existe ou foi removido do catálogo.</p>
      <RouterLink :to="{ name: 'catalogo' }" class="botao">Ver todos os cursos</RouterLink>
    </div>
  </div>

  <div v-else-if="estado === 'erro'" class="container detalhe-estado">
    <div class="aviso aviso--erro" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao" @click="carregar">Tentar novamente</button>
    </div>
  </div>

  <article v-else-if="detalhe" class="curso" aria-labelledby="titulo-curso">
    <section class="container curso-hero">
      <div class="curso-hero__texto">
        <!-- A trilha não está no Figma: entra para quem chega por link direto saber onde está e voltar. -->
        <nav aria-label="Trilha de navegação" class="trilha">
          <RouterLink :to="{ name: 'catalogo' }">Cursos</RouterLink>
          <span aria-hidden="true">/</span>
          <span aria-current="page">{{ detalhe.curso.nomeBreve }}</span>
        </nav>

        <div class="curso-hero__etiquetas">
          <p :class="['etiqueta', 'etiqueta--solta', `etiqueta--${detalhe.curso.categoria.valor}`]">
            <img :src="`/assets/icones/${detalhe.curso.categoria.icone}`" alt="" width="16" height="16" />
            {{ detalhe.curso.categoria.rotulo }}
          </p>
          <p :class="['selo', `selo--${detalhe.curso.status.valor}`]">{{ detalhe.curso.status.rotulo }}</p>
        </div>

        <h1 id="titulo-curso">{{ detalhe.curso.nomeCompleto }}</h1>

        <dl class="curso-hero__ficha">
          <div v-if="detalhe.curso.dataDisponibilizacao">
            <dt><IconeLinha nome="calendario" />Início do curso:</dt>
            <dd>
              <time :datetime="detalhe.curso.dataDisponibilizacao">{{ formatarData(detalhe.curso.dataDisponibilizacao) }}</time>
            </dd>
          </div>
          <div>
            <dt><IconeLinha nome="tela" />Modalidade:</dt>
            <dd>{{ detalhe.curso.modalidade.rotulo }}</dd>
          </div>
          <div>
            <dt><IconeLinha nome="relogio" />Carga horária:</dt>
            <dd>{{ detalhe.curso.cargaHorariaHoras }} horas</dd>
          </div>
          <div>
            <dt><IconeLinha nome="medalha" />Certificação:</dt>
            <dd>SESI</dd>
          </div>
        </dl>

        <RouterLink v-if="abertas" :to="primeiraAula" class="botao botao--escuro">
          Matricule-se
          <img src="/assets/icones/arrow-navigation-1.svg" alt="" width="20" height="20" />
        </RouterLink>
        <p v-else-if="detalhe.curso.status.valor === 'em-breve'" class="curso-hero__aviso">
          As inscrições abrem em breve. Volte para se matricular.
        </p>
        <p v-else class="curso-hero__aviso">
          Esta turma está encerrada.
          <RouterLink :to="{ name: 'catalogo', query: { status: 'inscricoes-abertas' } }">Ver cursos com inscrições abertas</RouterLink>
        </p>
      </div>

      <div class="curso-hero__midia">
        <img :src="detalhe.curso.imagemUrl" alt="" width="1200" height="675" />
        <!-- O pacote não traz vídeo: o play leva à aula de apresentação (a primeira). -->
        <RouterLink v-if="abertas" :to="primeiraAula" class="curso-hero__play">
          <span class="visualmente-oculto">Assistir à aula de apresentação</span>
        </RouterLink>
      </div>
    </section>

    <section class="container" aria-labelledby="titulo-diferenciais">
      <h2 id="titulo-diferenciais" class="visualmente-oculto">Diferenciais</h2>
      <ul class="diferenciais diferenciais--quatro">
        <li v-for="d in DIFERENCIAIS" :key="d.titulo">
          <img :src="`/assets/icones/${d.icone}-bege.svg`" alt="" width="48" height="48" />
          <h3>{{ d.titulo }}</h3>
          <p>{{ d.texto }}</p>
        </li>
      </ul>
    </section>

    <div class="container curso-sobre">
      <section aria-labelledby="titulo-sobre">
        <h2 id="titulo-sobre" class="secao__titulo">Sobre o curso</h2>
        <p class="curso-sobre__texto">{{ detalhe.curso.descricao }}</p>
        <h3 class="curso-sobre__subtitulo"><IconeLinha nome="pessoas" />Para quem é</h3>
        <p>{{ detalhe.curso.publicoAlvo }}</p>
      </section>

      <!-- Acordeão com <details>: abre e fecha por teclado e leitor de tela sem uma linha de JS. -->
      <div class="acordeao">
        <details>
          <summary>Módulos</summary>
          <p class="acordeao__resumo">{{ detalhe.modulos.length }} módulos, {{ totalAulas }} aulas.</p>
          <ol class="modulos">
            <li v-for="m in detalhe.modulos" :key="m.ordem">
              <strong>{{ m.ordem }}. {{ m.titulo }}</strong>
              <ol class="modulos__aulas">
                <li v-for="a in m.aulas" :key="a.ordem">
                  <span>{{ m.ordem }}.{{ a.ordem }} {{ a.titulo }}</span>
                  <small>{{ a.descricao }}</small>
                </li>
              </ol>
            </li>
          </ol>
        </details>
        <details>
          <summary>Aulas online e avaliações</summary>
          <p v-for="(p, i) in AULAS_E_AVALIACOES" :key="i">{{ p }}</p>
        </details>
        <details>
          <summary>Dúvidas frequentes</summary>
          <dl class="duvidas">
            <div v-for="d in DUVIDAS_FREQUENTES" :key="d.pergunta">
              <dt>{{ d.pergunta }}</dt>
              <dd>{{ d.resposta }}</dd>
            </div>
          </dl>
        </details>
      </div>
    </div>

    <div class="faixa-bege">
      <SecaoDepoimentos tema="bege" />
    </div>

    <section class="faixa-vinho fale" aria-labelledby="titulo-fale">
      <div class="container">
        <p class="secao__sobretitulo">Precisa de ajuda para decidir?</p>
        <h2 id="titulo-fale" class="secao__titulo">Fale com a gente</h2>
        <a :href="`mailto:${CONTATO.email}?subject=${encodeURIComponent(`Dúvida sobre o curso ${detalhe.curso.nomeBreve}`)}`" class="botao botao--claro">
          Mandar uma mensagem
          <img src="/assets/icones/arrow-navigation-1.svg" alt="" width="20" height="20" />
        </a>
      </div>
    </section>

    <!-- Barra do Figma: sempre presa no pé da tela, do topo ao rodapé da página do curso. -->
    <div v-if="abertas" class="barra-matricula">
      <div class="container barra-matricula__conteudo">
        <p class="barra-matricula__nome">
          <span class="barra-matricula__completo">{{ detalhe.curso.nomeCompleto }}</span>
          <span class="barra-matricula__breve">{{ detalhe.curso.nomeBreve }}</span>
        </p>
        <RouterLink :to="primeiraAula" class="botao botao--claro">
          Matricule-se
          <img src="/assets/icones/arrow-navigation-1.svg" alt="" width="20" height="20" />
        </RouterLink>
      </div>
    </div>
  </article>
</template>
