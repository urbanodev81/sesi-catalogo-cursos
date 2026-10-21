using Npgsql;

namespace CatalogoCursos.Api.Dados;

public static class ConexaoBanco
{
    /// <summary>
    /// <c>ConnectionStrings:Catalogo</c> (formato Npgsql) ou <c>DATABASE_URL</c>
    /// (<c>postgresql://usuario:senha@host:porta/banco?sslmode=require</c>), que é o
    /// formato que Render e Neon entregam.
    /// </summary>
    public static string Resolver(IConfiguration config)
    {
        var npgsql = config.GetConnectionString("Catalogo");
        var url = config["DATABASE_URL"];
        if (string.IsNullOrWhiteSpace(npgsql) && string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException(
                "Configure ConnectionStrings__Catalogo ou DATABASE_URL (veja o README, seção Configuração do banco).");

        var csb = new NpgsqlConnectionStringBuilder(string.IsNullOrWhiteSpace(npgsql) ? DeUrl(url!) : npgsql)
        {
            // A imagem aspnet não traz Kerberos; sem isto o Npgsql loga erro de libgssapi a cada conexão.
            GssEncryptionMode = GssEncryptionMode.Disable,
        };
        return csb.ConnectionString;
    }

    public static string DeUrl(string url)
    {
        var uri = new Uri(url);
        var credenciais = uri.UserInfo.Split(':', 2);
        var csb = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort || uri.Port <= 0 ? 5432 : uri.Port,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(credenciais[0]),
            Password = credenciais.Length > 1 ? Uri.UnescapeDataString(credenciais[1]) : null,
        };

        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        if (query["sslmode"] is { } ssl && Enum.TryParse<SslMode>(ssl.Replace("-", ""), true, out var modo))
            csb.SslMode = modo;

        return csb.ConnectionString;
    }
}
