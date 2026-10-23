using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace CatalogoCursos.Api.Tests;

/// <summary>
/// Sobe um Postgres descartável (Testcontainers) e a API real contra ele. A API
/// aplica as migrations ao subir, então o teste valida schema e seed juntos.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();

    /// <summary>Relógio da API nos testes: fixo em 24/10/2026, e os testes de presença o adiantam.</summary>
    public RelogioDeTeste Relogio { get; } = new(new DateTimeOffset(2026, 10, 24, 12, 0, 0, TimeSpan.Zero));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Catalogo", _postgres.GetConnectionString());
        builder.ConfigureTestServices(s => s.AddSingleton<TimeProvider>(Relogio));
    }

    public Task InitializeAsync() => _postgres.StartAsync();

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}

[CollectionDefinition(nameof(ApiCollection))]
public class ApiCollection : ICollectionFixture<ApiFactory>;

public sealed class RelogioDeTeste(DateTimeOffset agora) : TimeProvider
{
    public DateTimeOffset Agora { get; set; } = agora;
    public override DateTimeOffset GetUtcNow() => Agora;
}
