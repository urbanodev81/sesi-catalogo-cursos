<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { CONTATO, CURSOS_DESTAQUE } from '../conteudo/institucional'
import LinkAncora from './LinkAncora.vue'

// Do rodapé do Figma ficam só os links que levam a algum lugar: Blog, Termos de Uso e
// Trabalhe Conosco não têm página (diferença listada no README).
</script>

<template>
  <footer class="rodape">
    <div class="container rodape__colunas">
      <div class="rodape__marca">
        <img src="/assets/logos/logo-sesi-industria-white.svg" alt="SESI Indústria" width="188" height="19" />
        <address>
          <a :href="`mailto:${CONTATO.email}`">{{ CONTATO.email }}</a>
          <span>{{ CONTATO.endereco }}</span>
        </address>
      </div>

      <nav aria-labelledby="rodape-navegacao">
        <h2 id="rodape-navegacao" class="rodape__titulo">Navegação</h2>
        <ul>
          <li><RouterLink to="/">Início</RouterLink></li>
          <li><LinkAncora ancora="diferenciais">Certificação</LinkAncora></li>
          <li><RouterLink :to="{ name: 'catalogo' }">Cursos</RouterLink></li>
          <li><LinkAncora ancora="sobre">Sobre</LinkAncora></li>
        </ul>
      </nav>

      <nav aria-labelledby="rodape-cursos">
        <h2 id="rodape-cursos" class="rodape__titulo">Cursos populares</h2>
        <ul>
          <li v-for="c in CURSOS_DESTAQUE" :key="c.slug">
            <RouterLink :to="{ name: 'curso', params: { slug: c.slug } }">{{ c.nome }}</RouterLink>
          </li>
        </ul>
      </nav>

      <nav aria-labelledby="rodape-suporte">
        <h2 id="rodape-suporte" class="rodape__titulo">Suporte</h2>
        <ul>
          <li><a :href="`mailto:${CONTATO.email}`">Fale conosco</a></li>
          <li><a :href="`mailto:${CONTATO.email}?subject=Central de ajuda`">Central de ajuda</a></li>
          <li><RouterLink :to="{ name: 'privacidade' }">Política de Privacidade</RouterLink></li>
        </ul>
      </nav>
    </div>

    <div class="container rodape__base">
      <p>
        <img src="/assets/logos/logo-sesi.svg" alt="SESI" width="57" height="19" />
        Todos os direitos reservados.
      </p>
      <ul class="rodape__redes" aria-label="Redes sociais">
        <li><a href="https://www.linkedin.com/" rel="noopener" target="_blank"><img src="/assets/icones/icon-linkedIn-24px.svg" alt="LinkedIn (abre em nova aba)" width="24" height="24" /></a></li>
        <li><a href="https://www.instagram.com/" rel="noopener" target="_blank"><img src="/assets/icones/icon-instagram-24px.svg" alt="Instagram (abre em nova aba)" width="24" height="24" /></a></li>
        <li><a href="https://www.facebook.com/" rel="noopener" target="_blank"><img src="/assets/icones/icon-facebook-24px.svg" alt="Facebook (abre em nova aba)" width="24" height="24" /></a></li>
      </ul>
    </div>
  </footer>
</template>
