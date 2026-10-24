<script setup lang="ts">
import Carregando from '../Carregando.vue'
import { computed, ref } from 'vue'
import { ErroApi, enviarDocumento, obterDocumentos, urlDoArquivo } from '../../api/cliente'
import type { Documento } from '../../api/tipos'
import { useCarga } from '../../composables/useCarga'
import IconeLinha from '../IconeLinha.vue'

const { dados: documentos, estado, erro, carregar } = useCarga(obterDocumentos, [])
const pendentes = computed(() => documentos.value.filter((d) => !d.envio).length)

// Envio por documento: qual está subindo, e a mensagem (sucesso ou erro) de cada um.
const enviando = ref<number>()
const avisos = ref<Record<number, { texto: string; erro: boolean }>>({})
const TAMANHO_MAXIMO = 2 * 1024 * 1024

async function aoEscolher(documento: Documento, evento: Event) {
  const campo = evento.target as HTMLInputElement
  const arquivo = campo.files?.[0]
  campo.value = '' // escolher o mesmo arquivo de novo dispara o change outra vez
  if (!arquivo) return
  // Conferência rápida no navegador; quem decide de verdade é a API, pelos bytes do arquivo.
  if (arquivo.size > TAMANHO_MAXIMO) {
    avisos.value[documento.id] = { texto: 'O arquivo passa de 2 MB. Envie uma versão menor.', erro: true }
    return
  }
  enviando.value = documento.id
  avisos.value[documento.id] = { texto: 'Enviando…', erro: false }
  try {
    await enviarDocumento(documento.id, arquivo)
    avisos.value[documento.id] = { texto: `${documento.titulo}: arquivo recebido.`, erro: false }
    await carregar(true)
  } catch (e) {
    avisos.value[documento.id] = { texto: e instanceof ErroApi ? e.message : 'Não foi possível enviar agora.', erro: true }
  } finally {
    enviando.value = undefined
  }
}

const tamanho = (bytes: number) => (bytes < 1024 * 1024 ? `${Math.max(1, Math.round(bytes / 1024))} KB` : `${(bytes / 1024 / 1024).toFixed(1)} MB`)
const data = (iso: string) => new Date(iso).toLocaleDateString('pt-BR')
</script>

<template>
  <section class="cartao painel-bloco" aria-labelledby="titulo-documentos">
    <h2 id="titulo-documentos" class="cartao__titulo">
      <IconeLinha nome="medalha" /> Documentos
      <span v-if="estado === 'pronto'" class="painel-bloco__contagem">
        {{ pendentes === 0 ? 'tudo entregue' : `${pendentes} ${pendentes === 1 ? 'pendente' : 'pendentes'}` }}
      </span>
    </h2>
    <Carregando v-if="estado === 'carregando'" />
    <div v-else-if="estado === 'erro'" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao botao--contorno" @click="carregar()">Tentar novamente</button>
    </div>
    <template v-else>
      <p class="documentos__regra">PDF, JPG ou PNG, até 2 MB. Enviar de novo substitui o arquivo.</p>
      <ul class="documentos">
        <li v-for="d in documentos" :key="d.id" :class="{ 'documentos__item--ok': d.envio }">
          <div class="documentos__texto">
            <p class="documentos__titulo">
              {{ d.titulo }}
              <span :class="['selo', d.envio ? 'selo--ok' : 'selo--pendente']">{{ d.envio ? 'Enviado' : 'Pendente' }}</span>
            </p>
            <p v-if="d.envio" class="documentos__info">
              <a :href="urlDoArquivo(d.id)" download>{{ d.envio.nomeArquivo }}</a>
              · {{ tamanho(d.envio.tamanho) }} · {{ data(d.envio.em) }}
            </p>
            <p v-else class="documentos__info">{{ d.orientacao }}</p>
            <p
              v-if="avisos[d.id]"
              :class="['documentos__aviso', { 'documentos__aviso--erro': avisos[d.id]!.erro }]"
              :role="avisos[d.id]!.erro ? 'alert' : 'status'"
            >
              {{ avisos[d.id]!.texto }}
            </p>
          </div>
          <!-- O input fica visualmente oculto e o rótulo é o botão: clique e teclado abrem o seletor. -->
          <input
            :id="`arquivo-${d.id}`"
            type="file"
            class="visualmente-oculto"
            accept="application/pdf,image/jpeg,image/png"
            :disabled="enviando !== undefined"
            @change="aoEscolher(d, $event)"
          />
          <label :for="`arquivo-${d.id}`" :class="['botao', d.envio ? 'botao--contorno' : 'botao--escuro', { 'botao--ocupado': enviando === d.id }]">
            {{ enviando === d.id ? 'Enviando…' : d.envio ? 'Substituir' : 'Enviar' }}
            <span class="visualmente-oculto"> {{ d.titulo }}</span>
          </label>
        </li>
      </ul>
    </template>
  </section>
</template>
