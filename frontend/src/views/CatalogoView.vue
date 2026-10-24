<script setup lang="ts">
import { computed } from 'vue'
import CursoCardDestaque from '../components/CursoCardDestaque.vue'
import IconeLinha from '../components/IconeLinha.vue'
import { useCatalogo } from '../composables/useCatalogo'
import { TEXTO_DEMORA, useDemora } from '../composables/useDemora'

const { busca, consulta, cursos, total, estado, erro, erroInvalido, filtros, temFiltro, definir, limpar, recarregar } = useCatalogo()

const demorou = useDemora(() => estado.value === 'carregando')
const resumo = computed(() => {
  if (estado.value === 'carregando') return demorou.value ? `Carregando cursos… ${TEXTO_DEMORA}` : 'Carregando cursos…'
  if (estado.value === 'erro') return ''
  if (total.value === 0) return 'Nenhum curso encontrado'
  return total.value === 1 ? '1 curso encontrado' : `${total.value} cursos encontrados`
})

const ao = (campo: 'categoria' | 'modalidade' | 'status') => (e: Event) =>
  definir(campo, (e.target as HTMLSelectElement).value)
</script>

<template>
  <!-- O protótipo não tem esta página: o hero é o da home em versão curta, e os cards são os de "Principais cursos". -->
  <section class="pagina-hero" aria-labelledby="titulo-catalogo">
    <div class="container">
      <h1 id="titulo-catalogo">Cursos</h1>
      <p>Encontre o curso certo para o seu desenvolvimento na indústria.</p>
    </div>
  </section>

  <div class="container catalogo">
    <form class="filtros" role="search" aria-label="Buscar e filtrar cursos" @submit.prevent>
      <div class="campo campo--busca" data-tour="busca">
        <label for="busca">Buscar pelo nome</label>
        <!-- Campo em pílula com lupa, como a busca da tela de aula do Figma. -->
        <div class="busca">
          <input id="busca" v-model="busca" type="search" placeholder="Ex.: matemática" autocomplete="off" maxlength="100" />
          <IconeLinha nome="busca" class="busca__lupa" />
        </div>
      </div>

      <div class="campo" data-tour="filtros">
        <label for="categoria">Categoria</label>
        <select id="categoria" :value="consulta.categoria" @change="ao('categoria')($event)">
          <option value="">Todas</option>
          <option v-for="c in filtros.categorias" :key="c.valor" :value="c.valor">{{ c.rotulo }}</option>
        </select>
      </div>

      <div class="campo">
        <label for="modalidade">Modalidade</label>
        <select id="modalidade" :value="consulta.modalidade" @change="ao('modalidade')($event)">
          <option value="">Todas</option>
          <option v-for="m in filtros.modalidades" :key="m.valor" :value="m.valor">{{ m.rotulo }}</option>
        </select>
      </div>

      <div class="campo">
        <label for="status">Status</label>
        <select id="status" :value="consulta.status" @change="ao('status')($event)">
          <option value="">Todos</option>
          <option v-for="s in filtros.status" :key="s.valor" :value="s.valor">{{ s.rotulo }}</option>
        </select>
      </div>

      <button type="button" class="botao botao--contorno" :disabled="!temFiltro" @click="limpar">Limpar filtros</button>
    </form>

    <!-- Região viva: o leitor de tela anuncia a contagem a cada busca ou filtro. -->
    <p class="resultado" data-tour="resultado" role="status" aria-live="polite">{{ resumo }}</p>

    <ul v-if="estado === 'carregando'" class="grade" aria-hidden="true">
      <li v-for="n in 6" :key="n" class="destaque destaque--esqueleto" />
    </ul>

    <div v-else-if="estado === 'erro'" class="aviso aviso--erro" role="alert">
      <p>{{ erro }}</p>
      <button v-if="erroInvalido" type="button" class="botao" @click="limpar">Limpar filtros</button>
      <button v-else type="button" class="botao" @click="recarregar">Tentar novamente</button>
    </div>

    <div v-else-if="total === 0" class="aviso">
      <p>Nenhum curso corresponde à sua busca. Tente outro termo ou remova algum filtro.</p>
      <button v-if="temFiltro" type="button" class="botao" @click="limpar">Limpar filtros</button>
    </div>

    <ul v-else class="grade" data-tour="lista" aria-label="Cursos">
      <li v-for="c in cursos" :key="c.id"><CursoCardDestaque :curso="c" :nivel="2" ficha /></li>
    </ul>
  </div>
</template>
