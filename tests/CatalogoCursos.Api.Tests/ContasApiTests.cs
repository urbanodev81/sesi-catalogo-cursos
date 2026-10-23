using System.Net;
using System.Net.Http.Json;
using CatalogoCursos.Api.Contas;

namespace CatalogoCursos.Api.Tests;

[Collection(nameof(ApiCollection))]
public class ContasApiTests(ApiFactory factory)
{
    private const string Ingles = "ingles-instrumental-manuais-tecnicos";
    private static int _ip;

    /// <summary>
    /// Cliente com cookies e um IP próprio (X-Forwarded-For): o limite de login é por IP,
    /// e cada teste precisa da sua cota para não esbarrar no dos outros.
    /// </summary>
    private HttpClient NovoCliente()
    {
        var http = factory.CreateClient();
        http.DefaultRequestHeaders.Add("X-Forwarded-For", $"10.0.0.{Interlocked.Increment(ref _ip)}");
        return http;
    }

    private async Task<HttpClient> Logado(string email)
    {
        var http = NovoCliente();
        var resposta = await http.PostAsJsonAsync("/api/conta/entrar", new EntrarDto(email, "sesi2026"));
        resposta.EnsureSuccessStatusCode();
        return http;
    }

    [Fact]
    public async Task Login_valido_abre_sessao_e_sair_encerra()
    {
        var http = await Logado("ALUNO@exemplo.com "); // e-mail sem diferença de caixa e espaço

        var eu = await http.GetFromJsonAsync<UsuarioDto>("/api/conta");
        Assert.Equal("Ana Souza", eu!.Nome);
        Assert.Equal("aluno", eu.Papel);

        Assert.Equal(HttpStatusCode.NoContent, (await http.PostAsync("/api/conta/sair", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await http.GetAsync("/api/conta")).StatusCode);
    }

    [Theory]
    [InlineData("aluno@exemplo.com", "errada")]
    [InlineData("ninguem@exemplo.com", "sesi2026")]
    public async Task Senha_errada_e_email_inexistente_tem_a_mesma_resposta(string email, string senha)
    {
        var resposta = await NovoCliente().PostAsJsonAsync("/api/conta/entrar", new EntrarDto(email, senha));

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        Assert.Contains("E-mail ou senha incorretos.", await resposta.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Sexta_tentativa_de_login_no_mesmo_minuto_e_bloqueada()
    {
        var http = NovoCliente();
        for (var i = 0; i < 5; i++)
            await http.PostAsJsonAsync("/api/conta/entrar", new EntrarDto("aluno@exemplo.com", "errada"));

        var sexta = await http.PostAsJsonAsync("/api/conta/entrar", new EntrarDto("aluno@exemplo.com", "sesi2026"));

        Assert.Equal(HttpStatusCode.TooManyRequests, sexta.StatusCode);
    }

    [Theory]
    [InlineData("/api/conta/cursos")]
    [InlineData("/api/cursos/" + Ingles + "/progresso")]
    [InlineData("/api/professor/turmas")]
    public async Task Rotas_da_conta_sem_login_respondem_401(string rota)
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await NovoCliente().GetAsync(rota)).StatusCode);
    }

    [Fact]
    public async Task Aluno_nao_ve_o_painel_do_professor_e_professor_nao_marca_aula()
    {
        var aluno = await Logado("aluno@exemplo.com");
        var professor = await Logado("professor@exemplo.com");

        Assert.Equal(HttpStatusCode.Forbidden, (await aluno.GetAsync("/api/professor/turmas")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await professor.PostAsJsonAsync($"/api/cursos/{Ingles}/progresso", new AulaMarcadaDto(1, 1))).StatusCode);
    }

    [Fact]
    public async Task Aluno_de_demonstracao_ja_esta_cursando_os_dois_cursos_em_andamento()
    {
        var http = await Logado("aluno@exemplo.com");

        var cursos = (await http.GetFromJsonAsync<List<MeuCursoDto>>("/api/conta/cursos"))!;

        var ingles = Assert.Single(cursos, c => c.Curso.Slug == Ingles);
        Assert.Equal(4, ingles.TotalAulas);
        Assert.Equal(2, ingles.Concluidas);
        Assert.Equal(new AulaMarcadaDto(2, 1), ingles.Proxima); // a primeira que falta, já no módulo 2
        Assert.Contains(cursos, c => c.Curso.Slug == "leitura-interpretacao-normas-tecnicas");
    }

    [Fact]
    public async Task Matricular_e_concluir_aula_aparece_no_progresso_e_no_painel()
    {
        const string curso = "quimica-sustentabilidade";
        var aluno = await Logado("carla@exemplo.com");

        // Sem matrícula não conclui; matricular é idempotente.
        Assert.Equal(HttpStatusCode.Conflict,
            (await aluno.PostAsJsonAsync($"/api/cursos/{curso}/progresso", new AulaMarcadaDto(1, 1))).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await aluno.PostAsync($"/api/cursos/{curso}/matricula", null)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await aluno.PostAsync($"/api/cursos/{curso}/matricula", null)).StatusCode);

        // Concluir duas vezes a mesma aula conta uma vez.
        for (var i = 0; i < 2; i++)
            Assert.Equal(HttpStatusCode.NoContent,
                (await aluno.PostAsJsonAsync($"/api/cursos/{curso}/progresso", new AulaMarcadaDto(1, 2))).StatusCode);

        var progresso = await aluno.GetFromJsonAsync<ProgressoDto>($"/api/cursos/{curso}/progresso");
        Assert.True(progresso!.Matriculado);
        Assert.Equal(["1.2"], progresso.Concluidas);

        var turmas = (await (await Logado("professor@exemplo.com")).GetFromJsonAsync<List<TurmaDto>>("/api/professor/turmas"))!;
        var carla = Assert.Single(Assert.Single(turmas, t => t.Slug == curso).Alunos);
        Assert.Equal(("Carla Mendes", 1), (carla.Nome, carla.Concluidas));
    }

    [Fact]
    public async Task Nao_matricula_em_curso_encerrado_nem_conclui_aula_inexistente()
    {
        var aluno = await Logado("bruno@exemplo.com");

        Assert.Equal(HttpStatusCode.Conflict,
            (await aluno.PostAsync("/api/cursos/ciencia-materiais-metalurgia/matricula", null)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await aluno.PostAsJsonAsync($"/api/cursos/{Ingles}/progresso", new AulaMarcadaDto(9, 9))).StatusCode);
    }
}
