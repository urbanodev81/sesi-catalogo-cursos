using System.Net;
using System.Net.Http.Json;
using CatalogoCursos.Api.Cursos;

namespace CatalogoCursos.Api.Tests;

[Collection(nameof(ApiCollection))]
public class CursosApiTests(ApiFactory factory)
{
    private readonly HttpClient _http = factory.CreateClient();

    private async Task<ListaCursosDto> Listar(string query = "") =>
        (await _http.GetFromJsonAsync<ListaCursosDto>($"/api/cursos{query}"))!;

    [Fact]
    public async Task Lista_traz_o_catalogo_com_o_minimo_exigido()
    {
        var lista = await Listar();

        Assert.Equal(lista.Itens.Count, lista.Total);
        Assert.True(lista.Total >= 6, "o desafio exige pelo menos 6 cursos");
        Assert.True(lista.Itens.Select(c => c.Categoria.Valor).Distinct().Count() >= 3);
        Assert.True(lista.Itens.Select(c => c.Modalidade.Valor).Distinct().Count() >= 2);
        Assert.True(lista.Itens.Select(c => c.Status.Valor).Distinct().Count() >= 3);
        Assert.True(lista.Itens.Select(c => c.CargaHorariaHoras).Distinct().Count() > 1);
    }

    [Fact]
    public async Task Lista_mostra_inscricoes_abertas_primeiro_e_encerrados_por_ultimo()
    {
        var status = (await Listar()).Itens.Select(c => c.Status.Valor).ToList();

        Assert.Equal("inscricoes-abertas", status.First());
        Assert.Equal("encerrado", status.Last());
    }

    [Theory]
    [InlineData("matematica")]   // sem acento acha "Matemática"
    [InlineData("MATEMÁTICA")]   // maiúscula
    [InlineData("  matemática ")] // espaço nas pontas
    [InlineData("precisao")]     // pedaço do nome
    public async Task Busca_por_nome_ignora_acento_caixa_e_espacos(string busca)
    {
        var lista = await Listar($"?busca={Uri.EscapeDataString(busca)}");

        var curso = Assert.Single(lista.Itens);
        Assert.Equal("matematica-precisao-medidas-escalas-metalurgia", curso.Slug);
    }

    [Fact]
    public async Task Busca_tambem_considera_o_nome_breve()
    {
        var lista = await Listar("?busca=redacao profissional");

        Assert.Contains(lista.Itens, c => c.Slug == "comunicacao-tecnica-redacao-profissional");
    }

    [Fact]
    public async Task Busca_e_filtros_se_combinam()
    {
        var porCategoria = await Listar("?categoria=sustentabilidade");
        var combinado = await Listar("?categoria=sustentabilidade&modalidade=ead&status=inscricoes-abertas");

        Assert.True(porCategoria.Total > combinado.Total);
        Assert.All(combinado.Itens, c =>
        {
            Assert.Equal("sustentabilidade", c.Categoria.Valor);
            Assert.Equal("ead", c.Modalidade.Valor);
            Assert.Equal("inscricoes-abertas", c.Status.Valor);
        });

        var comBusca = await Listar("?busca=economia&categoria=sustentabilidade");
        Assert.Equal("economia-circular-industria", Assert.Single(comBusca.Itens).Slug);
    }

    [Fact]
    public async Task Sem_resultado_volta_lista_vazia_e_total_zero()
    {
        var lista = await Listar("?busca=nao-existe-curso-assim");

        Assert.Equal(0, lista.Total);
        Assert.Empty(lista.Itens);
    }

    [Theory]
    [InlineData("?modalidade=foo", "modalidade")]
    [InlineData("?status=aberto", "status")]
    [InlineData("?categoria=culinaria", "categoria")]
    public async Task Filtro_com_valor_desconhecido_responde_400_dizendo_qual(string query, string campo)
    {
        var resposta = await _http.GetAsync($"/api/cursos{query}");

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Contains($"\"{campo}\"", await resposta.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Busca_longa_demais_responde_400()
    {
        var resposta = await _http.GetAsync($"/api/cursos?busca={new string('a', CursosEndpoints.TamanhoMaximoBusca + 1)}");

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Detalhe_traz_modulos_e_aulas_em_ordem()
    {
        var detalhe = await _http.GetFromJsonAsync<CursoDetalheDto>("/api/cursos/estatistica-controle-qualidade-producao");

        Assert.NotNull(detalhe);
        Assert.Equal("em-breve", detalhe.Curso.Status.Valor);
        Assert.NotNull(detalhe.Curso.DataDisponibilizacao);
        Assert.NotEmpty(detalhe.Modulos);
        Assert.Equal(Enumerable.Range(1, detalhe.Modulos.Count), detalhe.Modulos.Select(m => m.Ordem));
        Assert.All(detalhe.Modulos, m => Assert.NotEmpty(m.Aulas));
    }

    [Fact]
    public async Task Catalogo_traz_os_seis_cursos_do_figma()
    {
        var nomes = (await Listar()).Itens.Select(c => c.NomeCompleto).ToList();

        Assert.Contains("Matemática para a Precisão: Medidas e Escalas na Metalurgia", nomes);
        Assert.Contains("Geometria e Modelagem no Design Têxtil", nomes);
        Assert.Contains("Estatística e Controle de Qualidade na Produção", nomes);
        Assert.Contains("Física do Movimento e da Energia na Metalurgia", nomes);
        Assert.Contains("Química e Sustentabilidade", nomes);
        Assert.Contains("Ciência dos Materiais na Metalurgia", nomes);
    }

    [Fact]
    public async Task Aula_traz_conteudo_e_vizinhas_atravessando_modulos()
    {
        const string curso = "/api/cursos/matematica-precisao-medidas-escalas-metalurgia";

        var primeira = await _http.GetFromJsonAsync<AulaConteudoDto>($"{curso}/modulos/1/aulas/1");
        Assert.NotNull(primeira);
        Assert.Null(primeira.Anterior);
        Assert.Equal(new AulaRefDto(1, 2, "Boas práticas"), primeira.Proxima);

        // Última aula do módulo 1: a próxima é a primeira do módulo 2.
        var fimDoModulo = await _http.GetFromJsonAsync<AulaConteudoDto>($"{curso}/modulos/1/aulas/3");
        Assert.NotNull(fimDoModulo);
        Assert.Equal((2, 1), (fimDoModulo.Proxima!.Modulo, fimDoModulo.Proxima.Aula));
        Assert.False(string.IsNullOrWhiteSpace(fimDoModulo.Conteudo));
    }

    [Theory]
    [InlineData("/api/cursos/matematica-precisao-medidas-escalas-metalurgia/modulos/1/aulas/99")]
    [InlineData("/api/cursos/matematica-precisao-medidas-escalas-metalurgia/modulos/99/aulas/1")]
    [InlineData("/api/cursos/curso-que-nao-existe/modulos/1/aulas/1")]
    public async Task Aula_inexistente_responde_404(string url)
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _http.GetAsync(url)).StatusCode);
    }

    [Fact]
    public async Task Curso_inexistente_responde_404()
    {
        var resposta = await _http.GetAsync("/api/cursos/curso-que-nao-existe");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Rota_de_api_inexistente_e_404_e_nao_o_index_do_front()
    {
        var resposta = await _http.GetAsync("/api/rota-que-nao-existe");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Filtros_listam_as_opcoes_que_a_lista_aceita()
    {
        var filtros = await _http.GetFromJsonAsync<FiltrosDto>("/api/filtros");

        Assert.NotNull(filtros);
        foreach (var c in filtros.Categorias)
            Assert.Equal(HttpStatusCode.OK, (await _http.GetAsync($"/api/cursos?categoria={c.Valor}")).StatusCode);
        foreach (var m in filtros.Modalidades)
            Assert.Equal(HttpStatusCode.OK, (await _http.GetAsync($"/api/cursos?modalidade={m.Valor}")).StatusCode);
        foreach (var s in filtros.Status)
            Assert.Equal(HttpStatusCode.OK, (await _http.GetAsync($"/api/cursos?status={s.Valor}")).StatusCode);
    }

    [Fact]
    public async Task Health_confirma_o_banco()
    {
        var resposta = await _http.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }
}
