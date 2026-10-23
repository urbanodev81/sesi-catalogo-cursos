import { ref, watch, type Ref } from 'vue'

// Preferências do painel de acessibilidade, guardadas no navegador. O estado fica no escopo do módulo:
// o painel e o "voltar ao topo" leem o mesmo valor sem passar props. A leitura do localStorage é
// protegida porque ele pode estar bloqueado (aba anônima, cookies desligados); aí vale o padrão.
const PREFIXO = 'sesi-a11y-'

function ler(chave: string): string | null {
  try {
    return localStorage.getItem(PREFIXO + chave)
  } catch {
    return null
  }
}

function guardar(chave: string, valor: number | boolean) {
  try {
    localStorage.setItem(PREFIXO + chave, typeof valor === 'boolean' ? (valor ? '1' : '0') : String(valor))
  } catch {
    // sem armazenamento, a preferência vale só até recarregar
  }
}

const numero = (chave: string) => ref(Number(ler(chave) ?? 0) || 0)
const booleano = (chave: string) => ref(ler(chave) === '1')

export const fonteNivel = numero('fonte') // 0 a 2
export const contrasteNivel = numero('contraste') // -2 a 2
export const escalaCinza = booleano('escala-cinza')
export const sublinharLinks = booleano('sublinhar-links')
export const espacamentoTexto = booleano('espacamento-texto')
export const pausarAnimacoes = booleano('pausar-animacoes')
export const cursorGrande = booleano('cursor-grande')
export const guiaLeitura = booleano('guia-leitura')

const CURSOR_GRANDE = `url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='40' height='40' viewBox='0 0 24 24'%3E%3Cpath d='M3 2l7.07 18 2.51-7.39L20 10.1z' fill='%23000' stroke='%23fff' stroke-width='1.5' stroke-linejoin='round'/%3E%3C/svg%3E") 2 2, auto`

/**
 * Aplica tudo no <html>. Contraste e escala de cinza saem num único `filter`: a propriedade não
 * acumula entre regras CSS, e quem ligasse os dois veria só um.
 */
export function aplicarPreferencias() {
  const html = document.documentElement
  html.classList.toggle('a11y-fonte-1', fonteNivel.value === 1)
  html.classList.toggle('a11y-fonte-2', fonteNivel.value === 2)
  html.classList.toggle('a11y-sublinhar-links', sublinharLinks.value)
  html.classList.toggle('a11y-espacamento-texto', espacamentoTexto.value)
  html.classList.toggle('a11y-pausar-animacoes', pausarAnimacoes.value)
  html.style.cursor = cursorGrande.value ? CURSOR_GRANDE : ''

  const filtro: string[] = []
  if (contrasteNivel.value === 2) filtro.push('contrast(1.4)', 'saturate(1.25)')
  else if (contrasteNivel.value === 1) filtro.push('contrast(1.2)', 'saturate(1.1)')
  else if (contrasteNivel.value === -1) filtro.push('contrast(0.9)', 'brightness(1.03)')
  else if (contrasteNivel.value === -2) filtro.push('contrast(0.78)', 'brightness(1.06)')
  if (escalaCinza.value) filtro.push('grayscale(1)')
  html.style.filter = filtro.join(' ')
}

const persistir = (chave: string, r: Ref<number | boolean>, aplica = true) =>
  watch(r, (valor) => {
    guardar(chave, valor)
    if (aplica) aplicarPreferencias()
  })

persistir('fonte', fonteNivel)
persistir('contraste', contrasteNivel)
persistir('escala-cinza', escalaCinza)
persistir('sublinhar-links', sublinharLinks)
persistir('espacamento-texto', espacamentoTexto)
persistir('pausar-animacoes', pausarAnimacoes)
persistir('cursor-grande', cursorGrande)
persistir('guia-leitura', guiaLeitura, false) // desenhado pelo painel, não por classe no <html>

export function restaurarPadrao() {
  fonteNivel.value = 0
  contrasteNivel.value = 0
  for (const r of [escalaCinza, sublinharLinks, espacamentoTexto, pausarAnimacoes, cursorGrande, guiaLeitura]) r.value = false
}
