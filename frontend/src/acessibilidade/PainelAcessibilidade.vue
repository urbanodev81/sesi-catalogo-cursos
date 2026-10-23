<script setup lang="ts">
import { nextTick, onBeforeUnmount, onMounted, ref } from 'vue'
import IconeLinha from '../components/IconeLinha.vue'
import {
  aplicarPreferencias,
  contrasteNivel,
  cursorGrande,
  escalaCinza,
  espacamentoTexto,
  fonteNivel,
  guiaLeitura,
  pausarAnimacoes,
  restaurarPadrao,
  sublinharLinks,
} from './preferencias'
import { usePainelFlutuante } from '../composables/usePainelFlutuante'

// Painel de preferências de acessibilidade, com botão próprio no canto inferior esquerdo: quem precisa
// de texto maior ou mais contraste tem de achar o recurso de cara, não dentro de um menu. Não substitui
// a acessibilidade da página (que passa no axe sem ele); dá ao usuário ajustes que o navegador não
// oferece num clique. Montado uma vez no App, fora do <RouterView>, para o VLibras e o guia de leitura
// sobreviverem à troca de página.
const aberto = ref(false)
const botao = ref<HTMLButtonElement>()
const painel = ref<HTMLElement>()
usePainelFlutuante('acessibilidade', aberto, botao, painel)
const leituraAberta = ref(false)

async function alternar() {
  aberto.value = !aberto.value
  if (aberto.value) {
    await nextTick()
    painel.value?.querySelector<HTMLElement>('button, input')?.focus()
  }
}

function fechar() {
  aberto.value = false
  botao.value?.focus()
}

// Esc fecha e devolve o foco ao botão que abriu (WCAG 2.1.2, sem armadilha de teclado).
const aoTeclar = (e: KeyboardEvent) => e.key === 'Escape' && aberto.value && fechar()

// ---------- VLibras ----------
// Tradutor oficial do governo (vlibras.gov.br). O acionamento é pelo painel, então o botão azul nativo
// nasce escondido (classe dock-vlibras), sem piscar na tela enquanto o plugin carrega. O item do painel
// só aparece depois de confirmar que o plugin criou o botão. Se em 20 s isso não acontecer, a classe sai
// e o botão nativo, se existir, volta: o Libras nunca fica sem caminho. Script fora do ar: nada quebra.
const vlibrasPronto = ref(false)
let esperaVLibras: ReturnType<typeof setInterval> | undefined

type JanelaComVLibras = Window & { VLibras?: { Widget: new (url: string) => unknown } }

function botaoDoVLibras(): HTMLElement | null {
  // O plugin atual cria o botão num shadow DOM próprio; versões antigas preenchiam a marcação [vw].
  const novo = document.getElementById('vlibras-access-wrapper')?.shadowRoot?.getElementById('vlibras-button')
  if (novo) return novo
  const antigo = document.querySelector<HTMLElement>('[vw-access-button]')
  return antigo?.firstElementChild ? antigo : null
}

function checarVLibras() {
  if (!(window as JanelaComVLibras).VLibras || !botaoDoVLibras()) return false
  vlibrasPronto.value = true
  return true
}

function carregarVLibras() {
  if (document.getElementById('vlibras-script')) return
  const script = document.createElement('script')
  script.id = 'vlibras-script'
  script.src = 'https://vlibras.gov.br/app/vlibras-plugin.js'
  script.async = true
  script.onload = () => {
    const VLibras = (window as JanelaComVLibras).VLibras
    if (VLibras) new VLibras.Widget('https://vlibras.gov.br/app')
  }
  document.body.appendChild(script)
}

function abrirLibras() {
  botaoDoVLibras()?.click()
  aberto.value = false
}

// ---------- Guia de leitura ----------
// Faixa de 120 px que segue o mouse, com o resto da tela escurecido. Só é desenhada depois do primeiro
// movimento: com a preferência já salva, nasceria em y=0 cobrindo a tela.
const guiaY = ref(0)
const mouseMoveu = ref(false)
function aoMoverMouse(e: MouseEvent) {
  guiaY.value = e.clientY
  mouseMoveu.value = true
}

onMounted(() => {
  aplicarPreferencias()
  window.addEventListener('keydown', aoTeclar)
  window.addEventListener('mousemove', aoMoverMouse, { passive: true })
  document.documentElement.classList.add('dock-vlibras')
  carregarVLibras()
  let tentativas = 0
  if (!checarVLibras())
    esperaVLibras = setInterval(() => {
      if (checarVLibras()) return clearInterval(esperaVLibras)
      if (++tentativas < 20) return
      clearInterval(esperaVLibras)
      document.documentElement.classList.remove('dock-vlibras')
    }, 1000)
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', aoTeclar)
  window.removeEventListener('mousemove', aoMoverMouse)
  clearInterval(esperaVLibras)
})
</script>

<template>
  <!-- Marcação que versões antigas do plugin preenchem; a atual cria a própria no <body>. -->
  <div vw class="enabled">
    <div vw-access-button class="active" />
    <div vw-plugin-wrapper><div class="vw-plugin-top-wrapper" /></div>
  </div>

  <button
    ref="botao"
    type="button"
    class="a11y-botao"
    :aria-expanded="aberto"
    aria-controls="painel-acessibilidade"
    aria-label="Opções de acessibilidade"
    title="Opções de acessibilidade"
    @click="alternar"
  >
    <IconeLinha nome="acessibilidade" />
  </button>

  <section
    v-if="aberto"
    id="painel-acessibilidade"
    ref="painel"
    class="a11y-painel"
    role="dialog"
    aria-modal="false"
    aria-labelledby="a11y-titulo"
  >
    <div class="a11y-painel__topo">
      <h2 id="a11y-titulo">Acessibilidade</h2>
      <button type="button" class="a11y-painel__texto" @click="restaurarPadrao">Restaurar padrão</button>
      <button type="button" class="a11y-painel__icone" aria-label="Fechar opções de acessibilidade" @click="fechar">
        <IconeLinha nome="fechar" />
      </button>
    </div>

    <label class="a11y-destaque">
      <span class="a11y-destaque__icone"><IconeLinha nome="pausa" /></span>
      <span>
        <strong>Parar animações</strong>
        <small>{{ pausarAnimacoes ? 'Ligado: nada se mexe na tela' : 'Para transições e rolagem animada' }}</small>
      </span>
      <input v-model="pausarAnimacoes" type="checkbox" />
    </label>

    <div class="a11y-linha">
      <span id="a11y-fonte">Tamanho do texto</span>
      <div class="a11y-passos" role="group" aria-labelledby="a11y-fonte">
        <button type="button" :disabled="fonteNivel === 0" aria-label="Diminuir o texto" @click="fonteNivel--">A-</button>
        <button type="button" :disabled="fonteNivel === 2" aria-label="Aumentar o texto" @click="fonteNivel++">A+</button>
      </div>
    </div>

    <div class="a11y-linha">
      <span id="a11y-contraste">Contraste</span>
      <div class="a11y-passos" role="group" aria-labelledby="a11y-contraste">
        <button type="button" :disabled="contrasteNivel === -2" aria-label="Reduzir o contraste" @click="contrasteNivel--">
          <IconeLinha nome="menos" />
        </button>
        <output aria-live="polite">{{ contrasteNivel > 0 ? `+${contrasteNivel}` : contrasteNivel }}</output>
        <button type="button" :disabled="contrasteNivel === 2" aria-label="Aumentar o contraste" @click="contrasteNivel++">
          <IconeLinha nome="mais" />
        </button>
      </div>
    </div>

    <button
      type="button"
      class="a11y-secao"
      :aria-expanded="leituraAberta"
      aria-controls="a11y-leitura"
      @click="leituraAberta = !leituraAberta"
    >
      Leitura e navegação
      <IconeLinha nome="expandir" :class="{ 'a11y-secao__seta--aberta': leituraAberta }" />
    </button>
    <div v-show="leituraAberta" id="a11y-leitura" class="a11y-opcoes">
      <label><span>Escala de cinza</span><input v-model="escalaCinza" type="checkbox" /></label>
      <label><span>Sublinhar links</span><input v-model="sublinharLinks" type="checkbox" /></label>
      <label><span>Espaçamento de texto</span><input v-model="espacamentoTexto" type="checkbox" /></label>
      <label><span>Cursor grande</span><input v-model="cursorGrande" type="checkbox" /></label>
      <label><span>Guia de leitura</span><input v-model="guiaLeitura" type="checkbox" /></label>
    </div>

    <button v-if="vlibrasPronto" type="button" class="a11y-libras" @click="abrirLibras">
      <IconeLinha nome="libras" />
      <span>
        <strong>Traduzir para Libras</strong>
        <small>VLibras, tradutor do governo federal</small>
      </span>
    </button>
  </section>

  <template v-if="guiaLeitura && mouseMoveu">
    <div class="a11y-guia a11y-guia--cima" :style="{ height: `${Math.max(0, guiaY - 60)}px` }" />
    <div class="a11y-guia a11y-guia--baixo" :style="{ top: `${guiaY + 60}px` }" />
  </template>
</template>
