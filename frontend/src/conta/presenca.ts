import { watch } from 'vue'
import { enviarPresenca } from '../api/cliente'
import { useSessao } from './sessao'

const INTERVALO_MS = 60_000 // a API considera online quem deu sinal nos últimos 2 minutos

/**
 * Sinal de presença para o "online" da lista de colegas: um POST por minuto enquanto há login e a
 * aba está visível. Aba em segundo plano não manda nada, então o aluno sai do "online" sozinho;
 * ao voltar para a aba, o sinal vai na hora. Falha de rede é ignorada: presença não é crítica.
 */
export function manterPresenca() {
  const { usuario } = useSessao()
  let relogio: ReturnType<typeof setInterval> | undefined

  const sinal = () => {
    if (usuario.value && document.visibilityState === 'visible') enviarPresenca().catch(() => undefined)
  }

  watch(
    () => usuario.value?.email,
    (logado) => {
      clearInterval(relogio)
      relogio = logado ? setInterval(sinal, INTERVALO_MS) : undefined
    },
    { immediate: true },
  )
  document.addEventListener('visibilitychange', sinal)
}
