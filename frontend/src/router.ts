import { createRouter, createWebHistory } from 'vue-router'
import InicioView from './views/InicioView.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'inicio', component: InicioView, meta: { titulo: 'Cursos para a indústria' } },
    {
      path: '/cursos',
      name: 'catalogo',
      component: () => import('./views/CatalogoView.vue'),
      meta: { titulo: 'Catálogo de cursos' },
    },
    {
      path: '/cursos/:slug',
      name: 'curso',
      component: () => import('./views/CursoView.vue'),
      props: true,
      meta: { titulo: 'Detalhes do curso' },
    },
    {
      path: '/area-do-aluno',
      name: 'area-do-aluno',
      component: () => import('./views/AreaAlunoView.vue'),
      meta: { titulo: 'Área do aluno' },
    },
    {
      path: '/:caminho(.*)*',
      name: 'nao-encontrado',
      component: () => import('./views/NaoEncontradoView.vue'),
      meta: { titulo: 'Página não encontrada' },
    },
  ],
  // Âncora (/#sobre) rola até a seção descontando o cabeçalho fixo (72px, --altura-topo):
  // o router rola com scrollTo e ignora o scroll-margin-top do CSS.
  scrollBehavior: (to, _from, salva) => salva ?? (to.hash ? { el: to.hash, top: 72 } : { top: 0 }),
})

// Título da aba por página: leitor de tela anuncia a troca de rota pelo título.
router.afterEach((to) => {
  document.title = `${to.meta.titulo ?? 'Catálogo de cursos'} | SESI Indústria`
})
