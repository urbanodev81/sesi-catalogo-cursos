<script setup lang="ts">
import { reactive } from 'vue'
import { RouterLink } from 'vue-router'
import { ErroApi, obterTurmas, publicarMaterial, removerMaterial, urlDoMaterial } from '../../api/cliente'
import type { AlunoProgresso, Material, Turma } from '../../api/tipos'
import { useCarga } from '../../composables/useCarga'
import IconeLinha from '../IconeLinha.vue'

// Área do professor: cada turma é um acordeão (o mesmo <details> da página do curso, do Figma), com os
// horários, os materiais e os alunos; cada aluno é outro acordeão, com o histórico de aulas.
// Depois de publicar ou remover material, a lista recarrega sem "Carregando…", então os acordeões
// abertos continuam abertos.
const { dados: turmas, estado, erro, carregar } = useCarga<Turma[]>(obterTurmas, [])

// Formulário de material por turma: título, arquivo escolhido, envio em andamento e a mensagem.
type Formulario = { titulo: string; arquivo: File | null; enviando: boolean; aviso: string; erro: boolean }
const formularios = reactive<Record<string, Formulario>>({})
const form = (slug: string) =>
  (formularios[slug] ??= { titulo: '', arquivo: null, enviando: false, aviso: '', erro: false })

function aoEscolher(slug: string, evento: Event) {
  const f = form(slug)
  f.arquivo = (evento.target as HTMLInputElement).files?.[0] ?? null
  f.aviso = ''
}

async function publicar(turma: Turma, evento: Event) {
  const f = form(turma.slug)
  if (!f.arquivo) {
    Object.assign(f, { aviso: 'Escolha o arquivo do material.', erro: true })
    return
  }
  f.enviando = true
  try {
    const material = await publicarMaterial(turma.slug, f.titulo, f.arquivo)
    Object.assign(f, { titulo: '', arquivo: null, aviso: `"${material.titulo}" publicado para a turma.`, erro: false })
    ;(evento.target as HTMLFormElement).reset()
    await carregar(true)
  } catch (e) {
    Object.assign(f, { aviso: e instanceof ErroApi ? e.message : 'Não foi possível publicar agora.', erro: true })
  } finally {
    f.enviando = false
  }
}

async function remover(turma: Turma, material: Material) {
  if (!window.confirm(`Remover "${material.titulo}"? Os alunos deixam de ver o arquivo.`)) return
  const f = form(turma.slug)
  try {
    await removerMaterial(material.id)
    Object.assign(f, { aviso: `"${material.titulo}" removido.`, erro: false })
    await carregar(true)
  } catch (e) {
    Object.assign(f, { aviso: e instanceof ErroApi ? e.message : 'Não foi possível remover agora.', erro: true })
  }
}

const pct = (feitas: number, total: number) => (total ? Math.round((feitas / total) * 100) : 0)
const data = (iso: string) => new Date(iso).toLocaleDateString('pt-BR')
const tamanho = (bytes: number) =>
  bytes < 1024 * 1024 ? `${Math.max(1, Math.round(bytes / 1024))} KB` : `${(bytes / 1024 / 1024).toFixed(1)} MB`
const documentos = (a: AlunoProgresso) =>
  a.documentosEntregues === a.documentosExigidos ? 'Documentos em dia' : `${a.documentosEntregues} de ${a.documentosExigidos} documentos`
</script>

<template>
  <section aria-labelledby="titulo-turmas">
    <h2 id="titulo-turmas" class="secao__titulo">Suas turmas</h2>

    <p v-if="estado === 'carregando'" role="status">Carregando…</p>
    <div v-else-if="estado === 'erro'" class="aviso aviso--erro" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao" @click="carregar()">Tentar novamente</button>
    </div>
    <p v-else-if="turmas.length === 0" class="aviso">Nenhum aluno matriculado ainda.</p>

    <div v-else class="acordeao turmas-prof">
      <details v-for="(t, i) in turmas" :key="t.slug" :open="i === 0">
        <summary>
          <span>
            {{ t.nome }}
            <small class="turmas-prof__resumo">
              {{ t.alunos.length }} {{ t.alunos.length === 1 ? 'aluno' : 'alunos' }} · {{ t.totalAulas }} aulas ·
              {{ t.materiais.length }} {{ t.materiais.length === 1 ? 'material' : 'materiais' }}
            </small>
          </span>
        </summary>

        <div class="turma-prof">
          <!-- ---------- Horários ---------- -->
          <section class="turma-prof__bloco" :aria-labelledby="`horarios-${t.slug}`">
            <h3 :id="`horarios-${t.slug}`" class="turma-prof__titulo"><IconeLinha nome="relogio" /> Horários da turma</h3>
            <p v-if="t.horarios.length === 0">Sem encontros fixos: o curso é feito no ritmo do aluno.</p>
            <ul v-else class="horarios">
              <li v-for="h in t.horarios" :key="h.dia + h.inicio">
                <strong>{{ h.dia }}</strong>
                <span>{{ h.inicio }} às {{ h.fim }}</span>
                <span class="horarios__descricao">{{ h.descricao }}</span>
              </li>
            </ul>
          </section>

          <!-- ---------- Materiais ---------- -->
          <section class="turma-prof__bloco" :aria-labelledby="`materiais-${t.slug}`">
            <h3 :id="`materiais-${t.slug}`" class="turma-prof__titulo"><IconeLinha nome="tela" /> Materiais para os alunos</h3>
            <p v-if="t.materiais.length === 0">Nenhum material publicado nesta turma.</p>
            <ul v-else class="materiais">
              <li v-for="m in t.materiais" :key="m.id">
                <span>
                  <a :href="urlDoMaterial(m.id)" download>{{ m.titulo }}</a>
                  <small>{{ m.nomeArquivo }} · {{ tamanho(m.tamanho) }} · {{ data(m.em) }}</small>
                </span>
                <button type="button" class="botao botao--contorno" @click="remover(t, m)">
                  Remover<span class="visualmente-oculto"> {{ m.titulo }}</span>
                </button>
              </li>
            </ul>

            <form class="material-novo" :aria-labelledby="`publicar-${t.slug}`" @submit.prevent="publicar(t, $event)">
              <p :id="`publicar-${t.slug}`" class="material-novo__titulo">Publicar material</p>
              <div class="campo">
                <label :for="`titulo-${t.slug}`">Título</label>
                <input :id="`titulo-${t.slug}`" v-model="form(t.slug).titulo" type="text" minlength="3" maxlength="120" required
                  placeholder="Ex.: Apostila do módulo 2" />
              </div>
              <div class="campo">
                <label :for="`arquivo-${t.slug}`">Arquivo (PDF, JPG ou PNG, até 2 MB)</label>
                <input :id="`arquivo-${t.slug}`" type="file" accept="application/pdf,image/jpeg,image/png" required
                  @change="aoEscolher(t.slug, $event)" />
              </div>
              <button type="submit" class="botao botao--escuro" :disabled="form(t.slug).enviando">
                {{ form(t.slug).enviando ? 'Publicando…' : 'Publicar' }}
              </button>
              <p v-if="form(t.slug).aviso" :class="['documentos__aviso', { 'documentos__aviso--erro': form(t.slug).erro }]"
                :role="form(t.slug).erro ? 'alert' : 'status'">
                {{ form(t.slug).aviso }}
              </p>
            </form>
          </section>

          <!-- ---------- Alunos ---------- -->
          <section class="turma-prof__bloco turma-prof__bloco--largo" :aria-labelledby="`alunos-${t.slug}`">
            <h3 :id="`alunos-${t.slug}`" class="turma-prof__titulo"><IconeLinha nome="pessoas" /> Alunos</h3>
            <div class="alunos-prof">
              <details v-for="a in t.alunos" :key="a.nome">
                <summary>
                  <span class="alunos-prof__nome">{{ a.nome }}</span>
                  <span class="alunos-prof__progresso">
                    <span class="barra-progresso barra-progresso--clara" aria-hidden="true">
                      <span :style="{ width: `${pct(a.concluidas, t.totalAulas)}%` }" />
                    </span>
                    {{ a.concluidas }} de {{ t.totalAulas }} aulas
                  </span>
                  <span :class="['selo', a.documentosEntregues === a.documentosExigidos ? 'selo--ok' : 'selo--pendente']">
                    {{ documentos(a) }}
                  </span>
                </summary>
                <div class="alunos-prof__historico">
                  <p class="alunos-prof__ultima">
                    {{ a.ultimaAtividade ? `Última atividade em ${data(a.ultimaAtividade)}` : 'Ainda não concluiu nenhuma aula' }}
                  </p>
                  <ol v-if="a.historico.length" :aria-label="`Histórico de ${a.nome}`">
                    <li v-for="h in a.historico" :key="`${h.modulo}.${h.aula}`">
                      <RouterLink :to="{ name: 'aula', params: { slug: t.slug, modulo: h.modulo, aula: h.aula } }">
                        Aula {{ h.modulo }}.{{ h.aula }}: {{ h.titulo }}
                      </RouterLink>
                      <span>concluída em {{ data(h.em) }}</span>
                    </li>
                  </ol>
                </div>
              </details>
            </div>
          </section>
        </div>
      </details>
    </div>
  </section>
</template>
