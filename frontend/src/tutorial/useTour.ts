import { computed, reactive } from 'vue'
import { PASSOS } from './passos'

/**
 * Tour guiado, um por TELA. Adaptado do tutorial dos outros sistemas da Urbano Dev (holofote +
 * balão, sem biblioteca de terceiro): um tour que atravessa páginas quebra quando a página troca e
 * o alvo ainda não existe, então cada tela tem o seu, e trocar de página fecha o aberto.
 *
 * Estado de módulo (um só para o app inteiro), como a sessão. Abre sozinho uma vez, na primeira tela
 * com tour que a pessoa visitar; depois, só pelo painel de Ajuda. Pular e concluir marcam visto do
 * mesmo jeito: tour que volta porque a pessoa pulou é o mesmo que não ter botão de pular.
 */
const CHAVE_VISTO = 'sesi-tour-visto'

const estado = reactive({ ativo: false, tela: '', passo: 0 })

export const temTour = (tela: string) => (PASSOS[tela]?.length ?? 0) > 0

function jaViu() {
  try {
    return localStorage.getItem(CHAVE_VISTO) === '1'
  } catch {
    return true // sem armazenamento, o tour não insiste a cada página
  }
}

function marcarVisto() {
  try {
    localStorage.setItem(CHAVE_VISTO, '1')
  } catch {
    // sem armazenamento: só deixa de lembrar
  }
}

export function useTour() {
  const passos = computed(() => PASSOS[estado.tela] ?? [])
  const passoAtual = computed(() => (estado.ativo ? passos.value[estado.passo] : undefined))
  const primeiro = computed(() => estado.passo === 0)
  const ultimo = computed(() => estado.passo === passos.value.length - 1)

  function iniciar(tela: string) {
    if (!temTour(tela)) return
    estado.tela = tela
    estado.passo = 0
    estado.ativo = true
  }

  function iniciarSeNuncaViu(tela: string) {
    if (!estado.ativo && !jaViu()) iniciar(tela)
  }

  function encerrar() {
    if (!estado.ativo) return
    estado.ativo = false
    marcarVisto()
  }

  const proximo = () => (ultimo.value ? encerrar() : estado.passo++)
  const anterior = () => primeiro.value || estado.passo--

  return { estado, passos, passoAtual, primeiro, ultimo, iniciar, iniciarSeNuncaViu, encerrar, proximo, anterior }
}
