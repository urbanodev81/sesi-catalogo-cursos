using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CatalogoCursos.Api.Contas;

namespace CatalogoCursos.Api.Tests;

/// <summary>
/// Painel do aluno. A Ana só é lida (os outros testes também contam com o seed dela); quem envia
/// documento é o Bruno. O relógio da API está fixo em 24/10/2026 (<see cref="ApiFactory.Relogio"/>).
/// </summary>
[Collection(nameof(ApiCollection))]
public class AlunoApiTests(ApiFactory factory)
{
    private static int _ip = 100;

    private async Task<HttpClient> Logado(string email)
    {
        var http = factory.CreateClient();
        http.DefaultRequestHeaders.Add("X-Forwarded-For", $"10.0.1.{Interlocked.Increment(ref _ip)}");
        (await http.PostAsJsonAsync("/api/conta/entrar", new EntrarDto(email, "sesi2026"))).EnsureSuccessStatusCode();
        return http;
    }

    private static MultipartFormDataContent Arquivo(byte[] conteudo, string nome)
    {
        var parte = new ByteArrayContent(conteudo);
        parte.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // o tipo vem dos bytes, não daqui
        return new MultipartFormDataContent { { parte, "arquivo", nome } };
    }

    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1, 2, 3, 4];

    [Theory]
    [InlineData("/api/aluno/avaliacoes")]
    [InlineData("/api/aluno/documentos")]
    [InlineData("/api/aluno/colegas")]
    [InlineData("/api/aluno/perfil")]
    public async Task Rotas_do_aluno_exigem_login_de_aluno(string rota)
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await factory.CreateClient().GetAsync(rota)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await (await Logado("professor@exemplo.com")).GetAsync(rota)).StatusCode);
    }

    [Fact]
    public async Task Avaliacoes_sao_so_as_futuras_dos_cursos_do_aluno_em_ordem_de_data()
    {
        var avaliacoes = (await (await Logado("aluno@exemplo.com")).GetFromJsonAsync<List<AvaliacaoDto>>("/api/aluno/avaliacoes"))!;

        // A de 15/10 já passou; as cinco seguintes, dos dois cursos da Ana, vêm em ordem.
        Assert.Equal([4, 2, 5, 3, 6], avaliacoes.Select(a => a.Id));
        Assert.Equal(("questionario", "Questionário"), (avaliacoes[0].Tipo.Valor, avaliacoes[0].Tipo.Rotulo));
    }

    [Fact]
    public async Task Envio_de_documento_valida_pelos_bytes_substitui_e_so_o_dono_baixa()
    {
        var bruno = await Logado("bruno@exemplo.com");

        // Texto com nome de PDF é recusado: quem decide o tipo é o conteúdo.
        var falso = await bruno.PostAsync("/api/aluno/documentos/3", Arquivo("ola"u8.ToArray(), "diploma.pdf"));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, falso.StatusCode);
        Assert.Contains("PDF, JPG ou PNG", await falso.Content.ReadAsStringAsync());

        var grande = new byte[Arquivos.TamanhoMaximo + 1];
        "%PDF-"u8.CopyTo(grande);
        Assert.Equal(HttpStatusCode.UnprocessableEntity,
            (await bruno.PostAsync("/api/aluno/documentos/3", Arquivo(grande, "grande.pdf"))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await bruno.PostAsync("/api/aluno/documentos/99", Arquivo(Png, "a.png"))).StatusCode);

        // Enviar duas vezes substitui; a lista mostra o último.
        (await bruno.PostAsync("/api/aluno/documentos/3", Arquivo([.. "%PDF-1.4 primeiro"u8], "primeiro.pdf"))).EnsureSuccessStatusCode();
        (await bruno.PostAsync("/api/aluno/documentos/3", Arquivo(Png, "../../escolaridade.png"))).EnsureSuccessStatusCode();
        var documentos = (await bruno.GetFromJsonAsync<List<DocumentoDto>>("/api/aluno/documentos"))!;
        var envio = Assert.Single(documentos, d => d.Id == 3).Envio!;
        Assert.Equal(("escolaridade.png", Png.Length), (envio.NomeArquivo, envio.Tamanho)); // sem o caminho

        var baixado = await bruno.GetAsync("/api/aluno/documentos/3/arquivo");
        Assert.Equal("image/png", baixado.Content.Headers.ContentType!.MediaType);
        Assert.Equal(Png, await baixado.Content.ReadAsByteArrayAsync());

        // A Ana não enviou o documento 3; o arquivo do Bruno não aparece para ela.
        Assert.Equal(HttpStatusCode.NotFound, (await (await Logado("aluno@exemplo.com")).GetAsync("/api/aluno/documentos/3/arquivo")).StatusCode);
    }

    [Fact]
    public async Task Colegas_sao_quem_tem_curso_em_comum_e_ficam_offline_sem_sinal()
    {
        var ana = await Logado("aluno@exemplo.com");
        var bruno = await Logado("bruno@exemplo.com");
        Assert.Equal(HttpStatusCode.NoContent, (await bruno.PostAsync("/api/conta/presenca", null)).StatusCode);

        var resposta = await ana.GetStringAsync("/api/aluno/colegas");
        Assert.DoesNotContain("@exemplo.com", resposta); // o e-mail do colega não vaza
        var colegas = (await ana.GetFromJsonAsync<List<ColegaDto>>("/api/aluno/colegas"))!;
        Assert.Equal(["Bruno Lima", "Carla Mendes"], colegas.Select(c => c.Nome).Order());
        Assert.True(Assert.Single(colegas, c => c.Nome == "Bruno Lima").Online);

        var antes = factory.Relogio.Agora;
        try
        {
            factory.Relogio.Agora = antes + AlunoEndpoints.JanelaOnline + TimeSpan.FromSeconds(1);
            colegas = (await ana.GetFromJsonAsync<List<ColegaDto>>("/api/aluno/colegas"))!;
            Assert.False(Assert.Single(colegas, c => c.Nome == "Bruno Lima").Online);
        }
        finally
        {
            factory.Relogio.Agora = antes;
        }
    }

    [Fact]
    public async Task Perfil_resume_cursos_aulas_documentos_e_proxima_avaliacao()
    {
        var perfil = (await (await Logado("aluno@exemplo.com")).GetFromJsonAsync<PerfilDto>("/api/aluno/perfil"))!;

        Assert.Equal(("Ana Souza", "aluno@exemplo.com"), (perfil.Nome, perfil.Email));
        Assert.Equal((2, 3, 3), (perfil.Cursos, perfil.AulasConcluidas, perfil.DocumentosPendentes));
        Assert.Equal(4, perfil.ProximaAvaliacao!.Id);
        Assert.Equal(new DateTimeOffset(2026, 10, 5, 9, 0, 0, TimeSpan.FromHours(-3)), perfil.AlunoDesde);
    }
}
