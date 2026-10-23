using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CatalogoCursos.Api.Contas;

namespace CatalogoCursos.Api.Tests;

/// <summary>
/// Área do professor. Materiais são publicados numa turma própria deste arquivo (o Bruno se matricula em
/// "Água e efluentes"), para não mexer nas turmas que os outros testes leem; tudo é removido no fim.
/// </summary>
[Collection(nameof(ApiCollection))]
public class ProfessorApiTests(ApiFactory factory)
{
    private const string Normas = "leitura-interpretacao-normas-tecnicas";
    private const string Agua = "agua-efluentes-industria";
    private static int _ip = 200;

    private async Task<HttpClient> Logado(string email)
    {
        var http = factory.CreateClient();
        http.DefaultRequestHeaders.Add("X-Forwarded-For", $"10.0.2.{Interlocked.Increment(ref _ip)}");
        (await http.PostAsJsonAsync("/api/conta/entrar", new EntrarDto(email, "sesi2026"))).EnsureSuccessStatusCode();
        return http;
    }

    private static MultipartFormDataContent Material(string titulo, byte[] conteudo, string nome = "material.pdf")
    {
        var parte = new ByteArrayContent(conteudo);
        parte.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        return new MultipartFormDataContent { { new StringContent(titulo), "titulo" }, { parte, "arquivo", nome } };
    }

    private static readonly byte[] Pdf = [.. "%PDF-1.4 material de teste"u8];

    [Fact]
    public async Task Turma_traz_horarios_materiais_e_historico_de_cada_aluno()
    {
        var turmas = (await (await Logado("professor@exemplo.com")).GetFromJsonAsync<List<TurmaDto>>("/api/professor/turmas"))!;
        var normas = Assert.Single(turmas, t => t.Slug == Normas);

        // Segunda antes de sábado, mesmo com o DayOfWeek do .NET começando no domingo.
        Assert.Equal(["Segunda-feira", "Sábado"], normas.Horarios.Select(h => h.Dia));
        Assert.Equal(("19:00", "21:00"), (normas.Horarios[0].Inicio, normas.Horarios[0].Fim));
        Assert.Contains(normas.Materiais, m => m.Titulo == "Guia de estudo do módulo 1");

        var ana = Assert.Single(normas.Alunos, a => a.Nome == "Ana Souza");
        var feita = Assert.Single(ana.Historico);
        Assert.Equal((1, 1), (feita.Modulo, feita.Aula));
        Assert.False(string.IsNullOrWhiteSpace(feita.Titulo));
        Assert.Equal((1, 4), (ana.DocumentosEntregues, ana.DocumentosExigidos));
    }

    [Fact]
    public async Task Aluno_ve_os_horarios_nos_seus_cursos()
    {
        var cursos = (await (await Logado("aluno@exemplo.com")).GetFromJsonAsync<List<MeuCursoDto>>("/api/conta/cursos"))!;
        Assert.Equal(2, Assert.Single(cursos, c => c.Curso.Slug == Normas).Horarios.Count);
    }

    [Fact]
    public async Task Material_publicado_so_aparece_e_baixa_para_quem_esta_na_turma()
    {
        var professor = await Logado("professor@exemplo.com");
        var bruno = await Logado("bruno@exemplo.com");
        var ana = await Logado("aluno@exemplo.com");

        // Sem turma ainda: o curso não aparece para o professor publicar.
        Assert.Equal(HttpStatusCode.NotFound, (await professor.PostAsync($"/api/professor/turmas/{Agua}/materiais", Material("Apostila", Pdf))).StatusCode);
        (await bruno.PostAsync($"/api/cursos/{Agua}/matricula", null)).EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Forbidden, (await bruno.PostAsync($"/api/professor/turmas/{Agua}/materiais", Material("Apostila", Pdf))).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await professor.PostAsync($"/api/professor/turmas/{Agua}/materiais", Material("A", Pdf))).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity,
            (await professor.PostAsync($"/api/professor/turmas/{Agua}/materiais", Material("Apostila", "texto"u8.ToArray()))).StatusCode);

        var publicado = await professor.PostAsync($"/api/professor/turmas/{Agua}/materiais", Material("Apostila do módulo 1", Pdf, "../apostila.pdf"));
        Assert.Equal(HttpStatusCode.Created, publicado.StatusCode);
        var material = (await publicado.Content.ReadFromJsonAsync<MaterialDto>())!;
        Assert.Equal("apostila.pdf", material.NomeArquivo);

        try
        {
            // O Bruno está na turma: vê e baixa. A Ana não está: não vê, e o download responde 404.
            Assert.Contains((await bruno.GetFromJsonAsync<List<MaterialDto>>("/api/aluno/materiais"))!, m => m.Id == material.Id);
            Assert.Equal(Pdf, await bruno.GetByteArrayAsync($"/api/materiais/{material.Id}/arquivo"));
            Assert.DoesNotContain((await ana.GetFromJsonAsync<List<MaterialDto>>("/api/aluno/materiais"))!, m => m.Id == material.Id);
            Assert.Equal(HttpStatusCode.NotFound, (await ana.GetAsync($"/api/materiais/{material.Id}/arquivo")).StatusCode);

            // Limite por turma: do 10º em diante, 409.
            var extras = new List<int>();
            for (var i = 1; i < ProfessorEndpoints.MateriaisPorTurma; i++)
                extras.Add((await (await professor.PostAsync($"/api/professor/turmas/{Agua}/materiais", Material($"Extra {i}", Pdf)))
                    .Content.ReadFromJsonAsync<MaterialDto>())!.Id);
            Assert.Equal(HttpStatusCode.Conflict, (await professor.PostAsync($"/api/professor/turmas/{Agua}/materiais", Material("Demais", Pdf))).StatusCode);
            foreach (var id in extras)
                Assert.Equal(HttpStatusCode.NoContent, (await professor.DeleteAsync($"/api/professor/materiais/{id}")).StatusCode);
        }
        finally
        {
            await professor.DeleteAsync($"/api/professor/materiais/{material.Id}");
        }
        Assert.Equal(HttpStatusCode.NotFound, (await bruno.GetAsync($"/api/materiais/{material.Id}/arquivo")).StatusCode);
    }
}
