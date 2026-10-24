<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { reservarTela } from '../composables/usePainelFlutuante'
import { useTour } from './useTour'

// O holofote e o balão do tour. Acha o alvo pelo `data-tour` (o primeiro VISÍVEL: o "Entrar" do
// cabeçalho existe no DOM do celular, escondido no menu, e lá o alvo é o botão "Menu"), rola até ele,
// escurece o resto com uma sombra gigante e põe o balão do lado que tiver espaço. Alvo que não
// aparece em ~1 s não trava nada: o balão vai para o centro, sem holofote.
//
// Acessibilidade: role=dialog com aria-modal, foco no balão a cada passo, Tab preso dentro dele,
// Esc encerra, e ao fim o foco volta para onde estava. Clicar no véu pergunta antes de sair.
const { estado, passos, passoAtual, primeiro, ultimo, iniciarSeNuncaViu, encerrar, proximo, anterior } = useTour()
const route = useRoute()
const router = useRouter()

const balao = ref<HTMLElement>()
const confirmandoSaida = ref(false)
const alvo = reactive({ top: 0, left: 0, width: 0, height: 0, achado: false })
const menosMovimento = window.matchMedia('(prefers-reduced-motion: reduce)').matches
let tentativas = 0
let espera: ReturnType<typeof setTimeout> | undefined
let focoAnterior: HTMLElement | null = null

function visivel(el: Element) {
  const r = el.getBoundingClientRect()
  return r.width > 0 && r.height > 0 && r.right > 0 && r.left < window.innerWidth && getComputedStyle(el).visibility !== 'hidden'
}

function medir(rolar = false) {
  clearTimeout(espera)
  const nome = passoAtual.value?.alvo
  const el = nome ? [...document.querySelectorAll(`[data-tour="${nome}"]`)].find(visivel) : undefined
  if (!el) {
    if (nome && tentativas-- > 0) espera = setTimeout(() => medir(rolar), 50)
    else alvo.achado = false
    return
  }
  let r = el.getBoundingClientRect()
  if (rolar && (r.top < 80 || r.bottom > window.innerHeight)) {
    el.scrollIntoView({ block: r.height > window.innerHeight * 0.6 ? 'start' : 'center', behavior: menosMovimento ? 'auto' : 'smooth' })
    r = el.getBoundingClientRect()
  }
  Object.assign(alvo, { top: r.top, left: r.left, width: r.width, height: r.height, achado: true })
}

function remedir(rolar = false) {
  tentativas = 20
  medir(rolar === true)
}

const FOLGA = 8
const holofote = computed(() => ({
  top: `${alvo.top - FOLGA}px`,
  left: `${alvo.left - FOLGA}px`,
  width: `${alvo.width + FOLGA * 2}px`,
  height: `${alvo.height + FOLGA * 2}px`,
}))

// Espaço para o balão inteiro (título, texto de 3 a 5 linhas e botões) sem rolar por dentro.
const ESPACO_MINIMO = 260

// Abaixo do alvo se couber, senão acima, senão no pé da tela, por cima de um alvo alto. Nunca ancorado com translateY(-100%),
// que penduraria o balão sem saber a altura do texto.
const posicao = computed(() => {
  if (!alvo.achado) return { top: '50%', left: '50%', transform: 'translate(-50%, -50%)' }
  const largura = Math.min(320, window.innerWidth - 32)
  const left = `${Math.min(Math.max(alvo.left, 16), window.innerWidth - largura - 16)}px`
  const abaixo = window.innerHeight - (alvo.top + alvo.height + FOLGA) - 24
  const acima = alvo.top - FOLGA - 24
  if (abaixo >= ESPACO_MINIMO) return { top: `${alvo.top + alvo.height + FOLGA + 12}px`, left, maxHeight: `${abaixo}px` }
  if (acima >= ESPACO_MINIMO) return { bottom: `${window.innerHeight - alvo.top + FOLGA + 12}px`, left, maxHeight: `${acima}px` }
  return { bottom: '12px', left, maxHeight: '45vh' }
})

watch(
  () => [estado.ativo, estado.passo] as const,
  ([ativo], antes) => {
    confirmandoSaida.value = false
    if (!ativo) {
      clearTimeout(espera)
      reservarTela(undefined)
      if (antes?.[0]) (focoAnterior?.isConnected ? focoAnterior : document.getElementById('conteudo'))?.focus({ preventScroll: true })
      return
    }
    if (!antes?.[0]) {
      focoAnterior = document.activeElement as HTMLElement | null
      reservarTela('tour') // fecha os painéis de acessibilidade e de ajuda
    }
    alvo.achado = false // o holofote não fica no alvo do passo anterior enquanto procura o novo
    nextTick(() => {
      remedir(true)
      balao.value?.focus({ preventScroll: true })
    })
  },
)

function aoTeclar(e: KeyboardEvent) {
  if (!estado.ativo) return
  if (e.key === 'Escape') {
    e.preventDefault()
    encerrar()
  } else if (e.key === 'Tab' && balao.value) {
    // Tab preso no balão: aria-modal promete que o resto da página está fora de alcance.
    const focaveis = [...balao.value.querySelectorAll<HTMLElement>('button')]
    const [primeiroBotao, ultimoBotao] = [focaveis[0], focaveis[focaveis.length - 1]]
    if (e.shiftKey && (document.activeElement === primeiroBotao || document.activeElement === balao.value)) {
      e.preventDefault()
      ultimoBotao?.focus()
    } else if (!e.shiftKey && document.activeElement === ultimoBotao) {
      e.preventDefault()
      primeiroBotao?.focus()
    }
  }
}

const aoMudarTela = () => remedir()
let pararRota: (() => void) | undefined

onMounted(async () => {
  window.addEventListener('keydown', aoTeclar)
  window.addEventListener('resize', aoMudarTela)
  window.addEventListener('scroll', aoMudarTela, true)
  await router.isReady()
  iniciarSeNuncaViu(String(route.name))
  // O tour é de uma tela: trocar de página fecha o aberto, e a primeira tela com tour abre o dela.
  pararRota = router.afterEach((para) => {
    if (estado.ativo && para.name !== estado.tela) encerrar()
    iniciarSeNuncaViu(String(para.name))
  })
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', aoTeclar)
  window.removeEventListener('resize', aoMudarTela)
  window.removeEventListener('scroll', aoMudarTela, true)
  pararRota?.()
  clearTimeout(espera)
})
</script>

<template>
  <Teleport to="body">
    <div v-if="estado.ativo && passoAtual" class="tour">
      <div :class="['tour__fundo', { 'tour__fundo--veu': !alvo.achado }]" aria-hidden="true" @click="confirmandoSaida = true" />
      <div v-if="alvo.achado" class="tour__holofote" :style="holofote" aria-hidden="true" />

      <div
        ref="balao"
        class="tour__balao"
        role="dialog"
        aria-modal="true"
        aria-labelledby="tour-titulo"
        aria-describedby="tour-texto"
        tabindex="-1"
        :style="posicao"
      >
        <template v-if="!confirmandoSaida">
          <p class="tour__passo">Passo {{ estado.passo + 1 }} de {{ passos.length }}</p>
          <h2 id="tour-titulo" class="tour__titulo">{{ passoAtual.titulo }}</h2>
          <p id="tour-texto" class="tour__texto">{{ passoAtual.texto }}</p>
          <div class="tour__rodape">
            <button type="button" class="tour__pular" @click="encerrar">Pular tour</button>
            <div class="tour__navegacao">
              <button v-if="!primeiro" type="button" class="botao botao--contorno" @click="anterior">Anterior</button>
              <button type="button" class="botao botao--escuro" @click="proximo">{{ ultimo ? 'Concluir' : 'Próximo' }}</button>
            </div>
          </div>
        </template>
        <template v-else>
          <h2 id="tour-titulo" class="tour__titulo">Sair do tour?</h2>
          <p id="tour-texto" class="tour__texto">Ele não abre sozinho de novo. Para ver depois, use o botão Ajuda.</p>
          <div class="tour__rodape tour__rodape--fim">
            <button type="button" class="botao botao--contorno" @click="confirmandoSaida = false">Continuar</button>
            <button type="button" class="botao botao--escuro" @click="encerrar">Sair do tour</button>
          </div>
        </template>
      </div>
    </div>
  </Teleport>
</template>
