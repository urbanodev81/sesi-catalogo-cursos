<script setup lang="ts">
import { nextTick, onBeforeUnmount, onMounted, ref } from 'vue'
import { AJUDA_PLATAFORMA, CONTATO, DUVIDAS_FREQUENTES } from '../conteudo/institucional'
import IconeLinha from './IconeLinha.vue'

// Painel de ajuda no canto inferior direito. Hoje responde às dúvidas frequentes (texto fixo, do site)
// em formato de conversa; o campo para escrever fica desabilitado, com o aviso de que o assistente
// com IA vem depois. Nada aqui finge ser IA: as respostas são as mesmas da página do curso.
const PERGUNTAS = [...AJUDA_PLATAFORMA, ...DUVIDAS_FREQUENTES]

type Mensagem = { de: 'voce' | 'sesi'; texto: string }
const aberto = ref(false)
const conversa = ref<Mensagem[]>([])
const botao = ref<HTMLButtonElement>()
const painel = ref<HTMLElement>()
const historico = ref<HTMLElement>()

async function alternar() {
  aberto.value = !aberto.value
  if (aberto.value) {
    await nextTick()
    painel.value?.querySelector<HTMLElement>('.ajuda__pergunta')?.focus()
  }
}

function fechar() {
  aberto.value = false
  botao.value?.focus()
}

async function perguntar(p: (typeof PERGUNTAS)[number]) {
  conversa.value.push({ de: 'voce', texto: p.pergunta }, { de: 'sesi', texto: p.resposta })
  await nextTick()
  historico.value?.scrollTo({ top: historico.value.scrollHeight })
}

const aoTeclar = (e: KeyboardEvent) => e.key === 'Escape' && aberto.value && fechar()
onMounted(() => window.addEventListener('keydown', aoTeclar))
onBeforeUnmount(() => window.removeEventListener('keydown', aoTeclar))
</script>

<template>
  <button
    ref="botao"
    type="button"
    class="ajuda-botao"
    :aria-expanded="aberto"
    aria-controls="painel-ajuda"
    @click="alternar"
  >
    <IconeLinha :nome="aberto ? 'fechar' : 'conversa'" />
    <span>Ajuda</span>
  </button>

  <section v-if="aberto" id="painel-ajuda" ref="painel" class="ajuda" role="dialog" aria-modal="false" aria-labelledby="ajuda-titulo">
    <div class="ajuda__topo">
      <h2 id="ajuda-titulo">Como podemos ajudar?</h2>
      <button type="button" class="a11y-painel__icone" aria-label="Fechar a ajuda" @click="fechar">
        <IconeLinha nome="fechar" />
      </button>
    </div>

    <!-- A conversa: cada resposta nova é anunciada pelo leitor de tela. -->
    <div ref="historico" class="ajuda__conversa" role="log" aria-live="polite" aria-label="Conversa">
      <p class="ajuda__balao ajuda__balao--sesi">
        Olá! Escolha uma das perguntas abaixo. Se não encontrar o que procura, escreva para
        <a :href="`mailto:${CONTATO.email}?subject=Central de ajuda`">{{ CONTATO.email }}</a>.
      </p>
      <p v-for="(m, i) in conversa" :key="i" :class="['ajuda__balao', `ajuda__balao--${m.de}`]">
        <span class="visualmente-oculto">{{ m.de === 'voce' ? 'Você perguntou:' : 'Resposta:' }}</span>
        {{ m.texto }}
      </p>
    </div>

    <ul class="ajuda__perguntas" aria-label="Perguntas frequentes">
      <li v-for="p in PERGUNTAS" :key="p.pergunta">
        <button type="button" class="ajuda__pergunta" @click="perguntar(p)">{{ p.pergunta }}</button>
      </li>
    </ul>

    <div class="ajuda__ia">
      <label for="ajuda-mensagem" class="visualmente-oculto">Mensagem para o assistente (em breve)</label>
      <input id="ajuda-mensagem" type="text" disabled placeholder="Escreva sua dúvida…" aria-describedby="ajuda-em-breve" />
      <p id="ajuda-em-breve"><span class="selo selo--em-breve">Em breve</span> Assistente com IA para responder qualquer dúvida sobre os cursos.</p>
    </div>
  </section>
</template>
