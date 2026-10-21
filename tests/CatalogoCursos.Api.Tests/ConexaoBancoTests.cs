using CatalogoCursos.Api.Dados;
using CatalogoCursos.Api.Dominio;
using Npgsql;

namespace CatalogoCursos.Api.Tests;

public class ConexaoBancoTests
{
    [Fact]
    public void Converte_DATABASE_URL_no_formato_do_Neon()
    {
        var cs = new NpgsqlConnectionStringBuilder(
            ConexaoBanco.DeUrl("postgresql://usuario:s%40nha@ep-x.neon.tech/catalogo?sslmode=require"));

        Assert.Equal("ep-x.neon.tech", cs.Host);
        Assert.Equal(5432, cs.Port);
        Assert.Equal("catalogo", cs.Database);
        Assert.Equal("usuario", cs.Username);
        Assert.Equal("s@nha", cs.Password);
        Assert.Equal(SslMode.Require, cs.SslMode);
    }

    [Fact]
    public void Respeita_porta_explicita()
    {
        var cs = new NpgsqlConnectionStringBuilder(ConexaoBanco.DeUrl("postgres://u:p@db:6543/x"));

        Assert.Equal(6543, cs.Port);
    }

    [Theory]
    [InlineData("Ciências da Natureza", "ciencias da natureza")]
    [InlineData("  EDUCAÇÃO ", "educacao")]
    public void Normaliza_texto_de_busca(string entrada, string esperado) =>
        Assert.Equal(esperado, TextoBusca.Normalizar(entrada));
}
