<script setup lang="ts">
import { CONTATO } from '../conteudo/institucional'

// O que o site guarda e o que sai dele. Sem rastreamento nem publicidade, nada aqui depende de
// consentimento (LGPD, art. 7º, e o guia de cookies da ANPD): a obrigação é a transparência.
</script>

<template>
  <div class="container privacidade">
    <h1>Política de Privacidade</h1>
    <p class="privacidade__nota">
      Este é um site de demonstração (teste técnico). Os cursos, as contas e as pessoas são fictícios.
    </p>

    <h2>O que guardamos no seu navegador</h2>
    <ul>
      <li>
        <strong>Cookie de sessão</strong> (<code>sesi.sessao</code>): só existe depois que você entra com
        e-mail e senha. Mantém você conectado por até 8 horas e não pode ser lido por scripts da página.
      </li>
      <li>
        <strong>Suas preferências</strong>, no armazenamento local do navegador, nunca enviadas ao servidor:
        as opções de acessibilidade, se você já viu o aviso de privacidade e, enquanto você não entra, as
        aulas marcadas como concluídas. Depois de entrar, o progresso fica na sua conta (veja abaixo).
      </li>
    </ul>
    <p>
      Não usamos cookies de análise, de publicidade ou de redes sociais. Por isso não há o que aceitar
      ou recusar: tudo o que é guardado serve para o site funcionar.
    </p>

    <h2>Serviços de terceiros</h2>
    <p>
      O tradutor para Libras é o <strong>VLibras</strong>, do governo federal. Para estar pronto quando você
      precisar, ele é carregado de <code>vlibras.gov.br</code> ao abrir o site; nisso, o servidor do VLibras
      recebe o endereço IP do seu acesso, como em qualquer página que você visita. As fontes e as imagens
      vêm do próprio site.
    </p>

    <h2>Dados de quem entra</h2>
    <p>
      Para alunos e professores, guardamos nome, e-mail, matrículas, aulas concluídas e o horário do último
      acesso (que mostra aos colegas de turma quem está online). Os documentos enviados ficam no banco de
      dados e só o próprio aluno consegue baixá-los; o professor vê apenas quantos foram entregues.
    </p>
    <p>
      As contas desta demonstração são compartilhadas (a senha está publicada). Por isso, quando o servidor
      reinicia, tudo o que foi enviado ou marcado com elas é apagado e a demonstração volta ao estado
      inicial. Não envie documentos verdadeiros.
    </p>

    <h2>Contato</h2>
    <p>Dúvidas sobre os seus dados: <a :href="`mailto:${CONTATO.email}`">{{ CONTATO.email }}</a>.</p>
  </div>
</template>
