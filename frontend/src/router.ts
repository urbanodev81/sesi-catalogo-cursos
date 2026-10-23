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
      path: '/cursos/:slug/modulos/:modulo(\\d+)/aulas/:aula(\\d+)',
      name: 'aula',
      component: () => import('./views/AulaView.vue'),
      props: (r) => ({ slug: r.params.slug, modulo: Number(r.params.modulo), aula: Number(r.params.aula) }),
      meta: { titulo: 'Aula', semCasca: true },
    },
    {
      // Porta única de alunos e professores. A mesma tela mostra o login ao visitante e, depois de entrar,
      // leva cada um ao seu painel pelo papel.
      path: '/admin',
      name: 'entrar',
      component: () => import('./views/AreaAlunoView.vue'),
      meta: { titulo: 'Entrar' },
    },
    {
      path: '/admin/aluno',
      name: 'area-do-aluno',
      component: () => import('./views/AreaAlunoView.vue'),
      meta: { titulo: 'Área do aluno' },
    },
    {
      path: '/admin/professor',
      name: 'area-do-professor',
      component: () => import('./views/AreaAlunoView.vue'),
      meta: { titulo: 'Área do professor' },
    },
    {
      path: '/admin/aluno/perfil',
      name: 'perfil',
      component: () => import('./views/PerfilView.vue'),
      meta: { titulo: 'Meu perfil' },
    },
    // Endereços antigos (já publicados) continuam funcionando.
    { path: '/area-do-aluno', redirect: { name: 'area-do-aluno' } },
    { path: '/area-do-aluno/perfil', redirect: { name: 'perfil' } },
    { path: '/area-do-professor', redirect: { name: 'area-do-professor' } },
    {
      path: '/privacidade',
      name: 'privacidade',
      component: () => import('./views/PrivacidadeView.vue'),
      meta: { titulo: 'Política de Privacidade' },
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
