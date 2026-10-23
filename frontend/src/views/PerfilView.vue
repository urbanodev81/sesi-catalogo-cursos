<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { obterPerfil } from '../api/cliente'
import type { Perfil } from '../api/tipos'
import { useCarga } from '../composables/useCarga'
import { useSessao } from '../conta/sessao'

// Perfil do aluno (login de demonstração): só leitura, porque o edital não pede cadastro nem edição.
const { usuario, carregarSessao } = useSessao()
carregarSessao()

const { dados: perfil, estado, erro, carregar } = useCarga<Perfil | null>(async (sinal) => {
  await carregarSessao()
  return usuario.value?.papel === 'aluno' ? obterPerfil(sinal) : null
}, null)

const iniciais = computed(() =>
  (perfil.value?.nome ?? '').split(' ').filter(Boolean).map((p) => p[0]).slice(0, 2).join('').toUpperCase(),
)
const data = (iso: string) => new Date(iso).toLocaleDateString('pt-BR', { day: '2-digit', month: 'long', year: 'numeric' })
const quando = (iso: string) =>
  new Date(iso).toLocaleString('pt-BR', { day: '2-digit', month: 'long', hour: '2-digit', minute: '2-digit' })
</script>

<template>
  <section class="pagina-hero" aria-labelledby="titulo-perfil">
    <div class="container pagina-hero__linha">
      <div>
        <h1 id="titulo-perfil">Meu perfil</h1>
        <p>Seus dados e um resumo dos seus estudos.</p>
      </div>
      <RouterLink :to="{ name: 'area-do-aluno' }" class="botao botao--claro">Voltar para a área do aluno</RouterLink>
    </div>
  </section>

  <div class="container area">
    <p v-if="estado === 'carregando'" role="status">Carregando…</p>

    <div v-else-if="estado === 'erro'" class="aviso aviso--erro" role="alert">
      <p>{{ erro }}</p>
      <button type="button" class="botao" @click="carregar()">Tentar novamente</button>
    </div>

    <div v-else-if="!perfil" class="aviso">
      <p>O perfil é dos alunos. Entre com uma conta de aluno para ver o seu.</p>
      <RouterLink :to="{ name: 'area-do-aluno' }" class="botao">Ir para a área do aluno</RouterLink>
    </div>

    <div v-else class="perfil">
      <section class="cartao perfil__dados" aria-labelledby="titulo-dados">
        <span class="perfil__avatar" aria-hidden="true">{{ iniciais }}</span>
        <h2 id="titulo-dados" class="cartao__titulo">{{ perfil.nome }}</h2>
        <dl class="perfil__lista">
          <div><dt>E-mail</dt><dd>{{ perfil.email }}</dd></div>
          <div><dt>Perfil</dt><dd>Aluno</dd></div>
          <div v-if="perfil.alunoDesde"><dt>Aluno desde</dt><dd>{{ data(perfil.alunoDesde) }}</dd></div>
        </dl>
        <p class="perfil__nota">Conta de demonstração: os dados não podem ser alterados.</p>
      </section>

      <section class="perfil__resumo" aria-labelledby="titulo-resumo">
        <h2 id="titulo-resumo" class="secao__titulo">Resumo</h2>
        <ul class="perfil__numeros">
          <li class="cartao">
            <strong>{{ perfil.cursos }}</strong>
            <span>{{ perfil.cursos === 1 ? 'curso em andamento' : 'cursos em andamento' }}</span>
          </li>
          <li class="cartao">
            <strong>{{ perfil.aulasConcluidas }}</strong>
            <span>{{ perfil.aulasConcluidas === 1 ? 'aula concluída' : 'aulas concluídas' }}</span>
          </li>
          <li :class="['cartao', { 'perfil__numero--alerta': perfil.documentosPendentes > 0 }]">
            <strong>{{ perfil.documentosPendentes }}</strong>
            <span>{{ perfil.documentosPendentes === 1 ? 'documento pendente' : 'documentos pendentes' }}</span>
          </li>
        </ul>
        <div v-if="perfil.proximaAvaliacao" class="cartao cartao--suave perfil__proxima">
          <p class="perfil__rotulo">Próxima avaliação</p>
          <p class="perfil__proxima-titulo">{{ perfil.proximaAvaliacao.titulo }}</p>
          <p>
            {{ perfil.proximaAvaliacao.tipo.rotulo }} de
            <RouterLink :to="{ name: 'curso', params: { slug: perfil.proximaAvaliacao.cursoSlug } }">
              {{ perfil.proximaAvaliacao.cursoNome }}</RouterLink>,
            em {{ quando(perfil.proximaAvaliacao.em) }}
          </p>
        </div>
        <RouterLink :to="{ name: 'area-do-aluno' }" class="botao botao--escuro perfil__acao">
          Ver cursos, avaliações e documentos
        </RouterLink>
      </section>
    </div>
  </div>
</template>
