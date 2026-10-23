import { readonly, ref } from 'vue'
import * as api from '../api/cliente'
import type { Usuario } from '../api/tipos'

// Estado global da sessão (um módulo é um singleton; não precisa de Pinia para um valor só).
// undefined = ainda não perguntou à API; null = ninguém logado.
const usuario = ref<Usuario | null | undefined>(undefined)

let consulta: Promise<Usuario | null> | undefined

/** Pergunta à API uma vez por carregamento da página; chamadas seguintes reaproveitam a resposta. */
export function carregarSessao() {
  consulta ??= api
    .obterSessao()
    .catch(() => null) // API fora do ar: segue como visitante
    .then((u) => (usuario.value = u))
  return consulta
}

export async function entrar(email: string, senha: string) {
  usuario.value = await api.entrar(email, senha)
}

export async function sair() {
  await api.sair()
  usuario.value = null
}

export function useSessao() {
  return { usuario: readonly(usuario), carregarSessao, entrar, sair }
}
