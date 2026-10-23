<script setup lang="ts">
import { computed, onBeforeUnmount, ref, shallowRef, watch } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { ErroApi, obterAula, obterCurso } from '../api/cliente'
import type { AulaConteudo, CursoDetalhe } from '../api/tipos'
import IconeLinha from '../components/IconeLinha.vue'
import { useSessao } from '../conta/sessao'
import { useProgresso } from '../progresso/progresso'

const props = defineProps<{ slug: string; modulo: number; aula: number }>()
const router = useRouter()

// A seta de voltar sai do ambiente de estudo: quem está logado volta para a área do aluno, de onde
// abriu o curso; quem chegou pelo "Matricule-se" sem login volta para a página do curso.
const { usuario } = useSessao()
const voltar = computed(() =>
  usuario.value
    ? usuario.value.papel === 'professor'
      ? { para: { name: 'area-do-professor' }, rotulo: 'Voltar para a área do professor' }
      : { para: { name: 'area-do-aluno' }, rotulo: 'Voltar para a área do aluno' }
    : { para: { name: 'curso', params: { slug: props.slug } }, rotulo: 'Voltar para a página do curso' },
)

// O curso (módulos da barra lateral) só recarrega se o curso mudar; a aula, a cada navegação.
const curso = shallowRef<CursoDetalhe>()
const aulaAtual = shallowRef<AulaConteudo>()
const estado = ref<'carregando' | 'pronto' | 'erro' | 'nao-encontrado'>('carregando')
const erro = ref('')

let controle: AbortController | undefined
async function carregar(trocouCurso: boolean) {
  controle?.abort()
  controle = new AbortController()
  const sinal = controle.signal
  estado.value = 'carregando'
  try {
    const [c, a] = await Promise.all([
      trocouCurso || !curso.value ? obterCurso(props.slug, sinal) : curso.value,
      obterAula(props.slug, props.modulo, props.aula, sinal),
    ])
    curso.value = c
    aulaAtual.value = a
    document.title = `${a.titulo} — ${a.cursoNome} | SESI Indústria`
    abertos.value = new Set([...abertos.value, props.modulo])
    menuAberto.value = false
    estado.value = 'pronto'
  } catch (e) {
    if (sinal.aborted) return
    estado.value = e instanceof ErroApi && e.naoEncontrado ? 'nao-encontrado' : 'erro'
    erro.value = e instanceof ErroApi ? e.message : 'Ocorreu um erro inesperado.'
  }
}

watch(
  () => [props.slug, props.modulo, props.aula] as const,
  (novo, anterior) => carregar(!anterior || novo[0] !== anterior[0]),
  { immediate: true },
)
onBeforeUnmount(() => controle?.abort())

// ---------- Progresso ----------
const { concluidas, concluir, concluida, erro: erroProgresso } = useProgresso(() => props.slug)
const totalAulas = computed(() => curso.value?.modulos.reduce((soma, m) => soma + m.aulas.length, 0) ?? 0)
const percentual = computed(() =>
  totalAulas.value ? Math.round((concluidas.value.size / totalAulas.value) * 100) : 0,
)
const atualConcluida = computed(() => concluida(props.modulo, props.aula))

const erroConclusao = ref('')
async function concluirAula() {
  erroConclusao.value = ''
  try {
    await concluir(props.modulo, props.aula)
  } catch (e) {
    erroConclusao.value = e instanceof ErroApi ? e.message : 'Não foi possível salvar o progresso.'
    return
  }
  const proxima = aulaAtual.value?.proxima
  if (proxima) router.push(rotaAula(proxima.modulo, proxima.aula))
}

const rotaAula = (modulo: number, aula: number) => ({ name: 'aula', params: { slug: props.slug, modulo, aula } })

// ---------- Barra lateral: módulos abertos e busca ----------
const abertos = ref(new Set<number>())
function alternar(modulo: number) {
  const s = new Set(abertos.value)
  if (s.has(modulo)) s.delete(modulo)
  else s.add(modulo)
  abertos.value = s
}

const busca = ref('')
const normalizar = (t: string) => t.normalize('NFD').replace(/\p{Diacritic}/gu, '').toLowerCase()
const modulosVisiveis = computed(() => {
  const termo = normalizar(busca.value.trim())
  const modulos = curso.value?.modulos ?? []
  if (!termo) return modulos
  return modulos
    .map((m) => ({ ...m, aulas: m.aulas.filter((a) => normalizar(a.titulo).includes(termo)) }))
    .filter((m) => m.aulas.length > 0)
})
// Com busca, todo módulo com resultado aparece aberto.
const estaAberto = (modulo: number) => busca.value.trim() !== '' || abertos.value.has(modulo)

// No celular a barra lateral vira um painel que abre por botão.
const menuAberto = ref(false)

// ---------- Conteúdo: texto puro, "## " vira subtítulo (sem v-html, então nada de HTML injetado) ----------
const blocos = computed(() => {
  const a = aulaAtual.value
  if (!a) return []
  const partes = a.conteudo.split(/\n\s*\n/).map((p) => p.trim()).filter(Boolean)
  // O texto costuma abrir repetindo o título da aula, que já é o h1 da página.
  if (partes[0] === `## ${a.titulo}`) partes.shift()
  return partes.map((p) => (p.startsWith('## ') ? { tipo: 'subtitulo' as const, texto: p.slice(3) } : { tipo: 'paragrafo' as const, texto: p }))
})
</script>

<template>
  <div class="aula-pagina">
    <aside class="aula-lateral" aria-label="Navegação do curso">
      <div class="aula-lateral__topo">
        <RouterLink :to="voltar.para" class="aula-lateral__voltar">
          <IconeLinha nome="voltar" />
          <span class="visualmente-oculto">{{ voltar.rotulo }}</span>
        </RouterLink>
        <button
          type="button"
          class="aula-lateral__menu"
          :aria-expanded="menuAberto"
          aria-controls="aula-painel"
          @click="menuAberto = !menuAberto"
        >
          Conteúdo do curso
        </button>
      </div>

      <div class="aula-lateral__curso">
        <p class="aula-lateral__sobretitulo">{{ curso?.curso.categoria.rotulo ?? 'Curso' }}</p>
        <p class="aula-lateral__nome">{{ aulaAtual?.cursoNome ?? curso?.curso.nomeCompleto ?? '' }}</p>
      </div>

      <div class="aula-lateral__progresso">
        <div
          class="barra-progresso"
          role="progressbar"
          aria-label="Progresso no curso"
          :aria-valuenow="percentual"
          aria-valuemin="0"
          aria-valuemax="100"
          :aria-valuetext="`${percentual}% concluído, ${concluidas.size} de ${totalAulas} aulas`"
        >
          <span :style="{ width: `${percentual}%` }" />
        </div>
        <span aria-hidden="true">{{ percentual }}%</span>
      </div>

      <div id="aula-painel" :class="['aula-lateral__painel', { 'aula-lateral__painel--aberto': menuAberto }]">
        <div class="aula-lateral__busca busca">
          <label for="busca-aula" class="visualmente-oculto">Buscar aula</label>
          <input id="busca-aula" v-model="busca" type="search" placeholder="Buscar aula" autocomplete="off" maxlength="60" />
          <IconeLinha nome="busca" class="busca__lupa" />
        </div>

        <p v-if="busca.trim() && modulosVisiveis.length === 0" class="aula-lateral__vazio" role="status">
          Nenhuma aula encontrada.
        </p>

        <ol class="aula-modulos">
          <li v-for="m in modulosVisiveis" :key="m.ordem">
            <button
              type="button"
              class="aula-modulos__titulo"
              :aria-expanded="estaAberto(m.ordem)"
              :aria-controls="`aulas-modulo-${m.ordem}`"
              @click="alternar(m.ordem)"
            >
              {{ m.ordem }}. {{ m.titulo }}
            </button>
            <ol v-show="estaAberto(m.ordem)" :id="`aulas-modulo-${m.ordem}`" class="aula-modulos__aulas">
              <li v-for="a in m.aulas" :key="a.ordem">
                <RouterLink
                  :to="rotaAula(m.ordem, a.ordem)"
                  :class="['aula-item', { 'aula-item--concluida': concluida(m.ordem, a.ordem) }]"
                  :aria-current="m.ordem === modulo && a.ordem === aula ? 'page' : undefined"
                >
                  <span class="aula-item__status" aria-hidden="true" />
                  <span>{{ m.ordem }}.{{ a.ordem }} {{ a.titulo }}</span>
                  <span v-if="concluida(m.ordem, a.ordem)" class="visualmente-oculto">(concluída)</span>
                </RouterLink>
              </li>
            </ol>
          </li>
        </ol>
      </div>
    </aside>

    <div class="aula-conteudo">
      <p v-if="estado === 'carregando' && !aulaAtual" role="status">Carregando aula…</p>

      <div v-else-if="estado === 'nao-encontrado'" class="aviso">
        <h1>Aula não encontrada</h1>
        <p>Esta aula não existe neste curso.</p>
        <RouterLink :to="{ name: 'curso', params: { slug } }" class="botao">Voltar para o curso</RouterLink>
      </div>

      <div v-else-if="estado === 'erro'" class="aviso aviso--erro" role="alert">
        <p>{{ erro }}</p>
        <button type="button" class="botao" @click="carregar(true)">Tentar novamente</button>
      </div>

      <article v-else-if="aulaAtual" :aria-busy="estado === 'carregando'" aria-labelledby="titulo-aula">
        <img class="aula-conteudo__banner" src="/assets/imagens/banner-ava.webp" alt="" width="1200" height="200" />
        <p class="aula-conteudo__onde">Módulo {{ aulaAtual.modulo }} · {{ aulaAtual.moduloTitulo }} · Aula {{ aulaAtual.aula }}</p>
        <h1 id="titulo-aula">{{ aulaAtual.titulo }}</h1>

        <template v-for="(b, i) in blocos" :key="i">
          <h2 v-if="b.tipo === 'subtitulo'">{{ b.texto }}</h2>
          <p v-else>{{ b.texto }}</p>
        </template>

        <div class="aula-conteudo__acoes">
          <p v-if="erroProgresso || erroConclusao" class="aula-conteudo__erro" role="alert">
            {{ erroConclusao || erroProgresso }}
          </p>
          <button v-if="!atualConcluida" type="button" class="botao botao--escuro" @click="concluirAula">
            {{ aulaAtual.proxima ? 'Concluir e ir para a próxima' : 'Concluir aula' }}
          </button>
          <p v-else class="aula-conteudo__feita" role="status">
            {{ percentual === 100 ? 'Parabéns! Você concluiu todas as aulas do curso.' : 'Aula concluída.' }}
          </p>
        </div>

        <nav class="aula-conteudo__navegacao" aria-label="Aulas vizinhas">
          <RouterLink v-if="aulaAtual.anterior" :to="rotaAula(aulaAtual.anterior.modulo, aulaAtual.anterior.aula)" class="aula-vizinha">
            <small>Aula anterior</small>
            {{ aulaAtual.anterior.modulo }}.{{ aulaAtual.anterior.aula }} {{ aulaAtual.anterior.titulo }}
          </RouterLink>
          <RouterLink
            v-if="aulaAtual.proxima"
            :to="rotaAula(aulaAtual.proxima.modulo, aulaAtual.proxima.aula)"
            class="aula-vizinha aula-vizinha--proxima"
          >
            <small>Próxima aula</small>
            {{ aulaAtual.proxima.modulo }}.{{ aulaAtual.proxima.aula }} {{ aulaAtual.proxima.titulo }}
          </RouterLink>
        </nav>
      </article>
    </div>
  </div>
</template>
