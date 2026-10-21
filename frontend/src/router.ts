import { createRouter, createWebHistory } from 'vue-router'
import CatalogoView from './views/CatalogoView.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'catalogo', component: CatalogoView, meta: { titulo: 'Catálogo de cursos' } },
    {
      path: '/cursos/:slug',
      name: 'curso',
      component: () => import('./views/CursoView.vue'),
      props: true,
      meta: { titulo: 'Detalhes do curso' },
    },
    {
      path: '/:caminho(.*)*',
      name: 'nao-encontrado',
      component: () => import('./views/NaoEncontradoView.vue'),
      meta: { titulo: 'Página não encontrada' },
    },
  ],
  scrollBehavior: (_to, _from, salva) => salva ?? { top: 0 },
})

// Título da aba por página: leitor de tela anuncia a troca de rota pelo título.
router.afterEach((to) => {
  document.title = `${to.meta.titulo ?? 'Catálogo de cursos'} | SESI Indústria`
})
