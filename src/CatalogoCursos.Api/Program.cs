using CatalogoCursos.Api.Cursos;
using CatalogoCursos.Api.Dados;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogoContext>(o => o.UseNpgsql(ConexaoBanco.Resolver(builder.Configuration)));
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Migrations (schema + catálogo fictício) aplicadas ao subir: a hospedagem gratuita
// não dá shell para rodar `dotnet ef database update` à mão.
if (app.Configuration.GetValue("Banco:MigrarAoIniciar", true))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<CatalogoContext>().Database.MigrateAsync();
}

app.MapOpenApi();
app.MapCursos();
app.MapGet("/health", async (CatalogoContext db, CancellationToken ct) =>
    await db.Database.CanConnectAsync(ct)
        ? Results.Ok(new { status = "ok" })
        : Results.Problem("Banco indisponível", statusCode: StatusCodes.Status503ServiceUnavailable))
    .ExcludeFromDescription();

// Rota de API inexistente é 404 de verdade, não o index.html do fallback abaixo.
app.Map("/api/{**resto}", () => Results.NotFound()).ExcludeFromDescription();

// O front (build do Vite) é servido daqui: um endereço só, sem CORS.
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
