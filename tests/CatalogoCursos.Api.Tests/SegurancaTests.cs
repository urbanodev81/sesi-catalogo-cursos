namespace CatalogoCursos.Api.Tests;

[Collection(nameof(ApiCollection))]
public class SegurancaTests(ApiFactory factory)
{
    [Theory]
    [InlineData("/api/cursos")]
    [InlineData("/")]
    public async Task Toda_resposta_leva_os_cabecalhos_de_seguranca(string rota)
    {
        var resposta = await factory.CreateClient().GetAsync(rota);
        string Cabecalho(string nome) => string.Join(",", resposta.Headers.GetValues(nome));

        var csp = Cabecalho("Content-Security-Policy");
        Assert.Contains("frame-ancestors 'none'", csp);
        Assert.Contains("object-src 'none'", csp);
        Assert.DoesNotContain("script-src 'self' 'unsafe-inline'", csp); // script inline continua proibido
        Assert.Equal("nosniff", Cabecalho("X-Content-Type-Options"));
        Assert.Equal("strict-origin-when-cross-origin", Cabecalho("Referrer-Policy"));
    }
}
