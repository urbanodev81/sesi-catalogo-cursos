<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, ref, shallowRef, watch } from 'vue'
import { RouterLink } from 'vue-router'
import { ErroApi, obterMeusCursos, obterTurmas } from '../api/cliente'
import type { MeuCurso, Turma } from '../api/tipos'
import ColegasTurma from '../components/aluno/ColegasTurma.vue'
import DocumentosPendentes from '../components/aluno/DocumentosPendentes.vue'
import ProximasAvaliacoes from '../components/aluno/ProximasAvaliacoes.vue'
import { useSessao } from '../conta/sessao'

// Login de demonstração (não é requisito do edital): o aluno vê os cursos que está fazendo e o
// professor acompanha o progresso das turmas, só leitura. Contas e senha publicadas no README.
const { usuario, carregarSessao, entrar, sair } = useSessao()
carregarSessao()

const CONTAS_DEMO = [
  { email: 'aluno@exemplo.com', papel: 'Aluno' },
  { email: 'professor@exemplo.com', papel: 'Professor' },
]
const SENHA_DEMO = 'sesi2026'

// ---------- Login ----------
const email = ref('')
const senha = ref('')
const enviando = ref(false)
const erroLogin = ref('')

async function aoEntrar() {
  erroLogin.value = ''
  enviando.value = true
  try {
    await entrar(email.value, senha.value)
    senha.value = ''
    await nextTick()
    document.getElementById('conteudo')?.focus() // a página trocou de conteúdo: o leitor recomeça do título
  } catch (e) {
    erroLogin.value = e instanceof ErroApi ? e.message : 'Não foi possível entrar agora. Tente novamente.'
  } finally {
    enviando.value = false
  }
}

function preencher(conta: string) {
  email.value = conta
  senha.value = SENHA_DEMO
}

async function aoSair() {
  await sair().catch(() => undefined)
  await nextTick()
  document.getElementById('conteudo')?.focus()
}

// ---------- Dados do painel ----------
const meusCursos = shallowRef<MeuCurso[]>([])
const turmas = shallowRef<Turma[]>([])
const estado = ref<'carregando' | 'pronto' | 'erro'>('carregando')
const erro = ref('')

let controle: AbortController | undefined
async function carregarPainel() {
  controle?.abort()
  const papel = usuario.value?.papel
  if (!papel) return
  controle = new AbortController()
  const sinal = controle.signal
  estado.value = 'carregando'
  try {
    if (papel === 'aluno') meusCursos.value = await obterMeusCursos(sinal)
    else turmas.value = await obterTurmas(sinal)
    estado.value = 'pronto'
  } catch (e) {
    if (sinal.aborted) return
    erro.value = e instanceof ErroApi ? e.message : 'Ocorreu um erro inesperado.'
    estado.value = 'erro'
  }
}
watch(() => usuario.value?.papel, carregarPainel, { immediate: true })
onBeforeUnmount(() => controle?.abort())

const primeiroNome = computed(() => usuario.value?.nome.split(' ')[0] ?? '')
const pct = (feitas: number, total: number) => (total ? Math.round((feitas / total) * 100) : 0)
const formatarData = (iso: string) =>
  new Date(iso).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' })
</script>

<template>
  <section class="pagina-hero" aria-labelledby="titulo-area">
    <div class="container pagina-hero__linha">
      <div>
        <h1 id="titulo-area">{{ usuario ? `Olá, ${primeiroNome}` : 'Área do aluno' }}</h1>
        <p v-if="usuario?.papel === 'professor'">Acompanhe o progresso dos alunos em cada curso.</p>
        <p v-else-if="usuario">Continue de onde parou.</p>
        <p v-else>Entre para acompanhar o seu progresso nos cursos.</p>
      </div>
      <div v-if="usuario" class="pagina-hero__acoes">
        <RouterLink v-if="usuario.papel === 'aluno'" :to="{ name: 'perfil' }" class="botao botao--claro">Meu perfil</RouterLink>
        <button type="button" class="botao botao--claro" @click="aoSair">Sair</button>
      </div>
    </div>
  </section>

  <div class="container area">
    <p v-if="usuario === undefined" role="status">Carregando…</p>

    <!-- ---------- Visitante: login ---------- -->
    <div v-else-if="usuario === null" class="area__login">
      <form class="cartao" aria-labelledby="titulo-entrar" @submit.prevent="aoEntrar">
        <h2 id="titulo-entrar" class="cartao__titulo">Entrar</h2>
        <div class="campo">
          <label for="email">E-mail</label>
          <input id="email" v-model="email" type="email" autocomplete="username" required />
        </div>
        <div class="campo">
          <label for="senha">Senha</label>
          <input id="senha" v-model="senha" type="password" autocomplete="current-password" required />
        </div>
        <p v-if="erroLogin" class="area__erro" role="alert">{{ erroLogin }}</p>
        <button type="submit" class="botao botao--escuro" :disabled="enviando">
          {{ enviando ? 'Entrando…' : 'Entrar' }}
        </button>
      </form>

      <aside class="cartao cartao--suave" aria-labelledby="titulo-demo">
        <h2 id="titulo-demo" class="cartao__titulo">Contas de demonstração</h2>
        <p>Senha de todas: <strong>{{ SENHA_DEMO }}</strong>. Sem entrar, o progresso das aulas fica salvo só neste navegador.</p>
        <ul class="area__demo">
          <li v-for="c in CONTAS_DEMO" :key="c.email">
            <span><strong>{{ c.papel }}</strong> {{ c.email }}</span>
            <button type="button" class="botao botao--contorno" @click="preencher(c.email)">
              Usar<span class="visualmente-oculto"> a conta de {{ c.papel.toLowerCase() }}</span>
            </button>
          </li>
        </ul>
      </aside>
    </div>

    <p v-else-if="estado === 'carregando'" role="status">Carregando…</p>

    <div v-else-if="estado === 'erro'" class="aviso aviso--erro" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao" @click="carregarPainel">Tentar novamente</button>
    </div>

    <!-- ---------- Aluno: meus cursos ---------- -->
    <template v-else-if="usuario.papel === 'aluno'">
    <section aria-labelledby="titulo-meus">
      <h2 id="titulo-meus" class="secao__titulo">Meus cursos</h2>
      <div v-if="meusCursos.length === 0" class="aviso">
        <p>Você ainda não começou nenhum curso.</p>
        <RouterLink :to="{ name: 'catalogo', query: { status: 'inscricoes-abertas' } }" class="botao">
          Ver cursos com inscrições abertas
        </RouterLink>
      </div>
      <ul v-else class="meus-cursos">
        <li v-for="m in meusCursos" :key="m.curso.slug" class="cartao meu-curso">
          <p :class="['etiqueta', 'etiqueta--solta', `etiqueta--${m.curso.categoria.valor}`]">
            <img :src="`/assets/icones/${m.curso.categoria.icone}`" alt="" width="16" height="16" />
            {{ m.curso.categoria.rotulo }}
          </p>
          <h3 class="meu-curso__nome">
            <RouterLink :to="{ name: 'curso', params: { slug: m.curso.slug } }">{{ m.curso.nomeCompleto }}</RouterLink>
          </h3>
          <div class="meu-curso__progresso">
            <div
              class="barra-progresso barra-progresso--clara"
              role="progressbar"
              :aria-label="`Progresso em ${m.curso.nomeBreve}`"
              :aria-valuenow="pct(m.concluidas, m.totalAulas)"
              aria-valuemin="0"
              aria-valuemax="100"
            >
              <span :style="{ width: `${pct(m.concluidas, m.totalAulas)}%` }" />
            </div>
            <span>{{ m.concluidas }} de {{ m.totalAulas }} aulas</span>
          </div>
          <RouterLink
            v-if="m.proxima"
            :to="{ name: 'aula', params: { slug: m.curso.slug, modulo: m.proxima.modulo, aula: m.proxima.aula } }"
            class="botao botao--escuro"
          >
            Continuar<span class="visualmente-oculto"> {{ m.curso.nomeBreve }}</span>
          </RouterLink>
          <p v-else class="meu-curso__fim">Curso concluído</p>
        </li>
      </ul>
    </section>

    <!-- Cada bloco carrega sozinho: um que falhe não derruba os outros. -->
    <div class="painel-aluno">
      <ProximasAvaliacoes />
      <DocumentosPendentes />
      <ColegasTurma />
    </div>
    </template>

    <!-- ---------- Professor: turmas ---------- -->
    <section v-else aria-labelledby="titulo-turmas">
      <h2 id="titulo-turmas" class="secao__titulo">Acompanhamento das turmas</h2>
      <p v-if="turmas.length === 0" class="aviso">Nenhum aluno matriculado ainda.</p>
      <div v-for="t in turmas" :key="t.slug" class="turma">
        <h3 :id="`turma-${t.slug}`" class="turma__titulo">
          <RouterLink :to="{ name: 'curso', params: { slug: t.slug } }">{{ t.nome }}</RouterLink>
          <small>{{ t.alunos.length }} {{ t.alunos.length === 1 ? 'aluno' : 'alunos' }} · {{ t.totalAulas }} aulas</small>
        </h3>
        <!-- Tabela larga no celular rola dentro da própria caixa, não a página. -->
        <div class="turma__rolagem" role="region" :aria-labelledby="`turma-${t.slug}`" tabindex="0">
          <table>
            <caption class="visualmente-oculto">Progresso dos alunos em {{ t.nome }}</caption>
            <thead>
              <tr>
                <th scope="col">Aluno</th>
                <th scope="col">Aulas concluídas</th>
                <th scope="col">Progresso</th>
                <th scope="col">Última atividade</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="a in t.alunos" :key="a.nome">
                <th scope="row">{{ a.nome }}</th>
                <td>{{ a.concluidas }} de {{ t.totalAulas }}</td>
                <td>
                  <span class="turma__pct">
                    <span class="barra-progresso barra-progresso--clara" aria-hidden="true">
                      <span :style="{ width: `${pct(a.concluidas, t.totalAulas)}%` }" />
                    </span>
                    {{ pct(a.concluidas, t.totalAulas) }}%
                  </span>
                </td>
                <td>{{ a.ultimaAtividade ? formatarData(a.ultimaAtividade) : 'Ainda não começou' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </section>
  </div>
</template>
