using System.Threading.RateLimiting;
using CatalogoCursos.Api.Contas;
using CatalogoCursos.Api.Cursos;
using CatalogoCursos.Api.Dados;
using CatalogoCursos.Api.Dominio;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogoContext>(o => o.UseNpgsql(ConexaoBanco.Resolver(builder.Configuration)));
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System); // os testes trocam por um relógio fixo

// Login por cookie: a SPA e a API têm a mesma origem, então não há token para guardar no
// JavaScript. HttpOnly (script não lê), SameSite=Lax (outro site não manda o cookie num POST,
// o que fecha o CSRF) e Secure quando a requisição chega por HTTPS.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "sesi.sessao";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
        // API não redireciona para tela de login: responde 401/403 e o front decide.
        o.Events.OnRedirectToLogin = c => { c.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
        o.Events.OnRedirectToAccessDenied = c => { c.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(nameof(Papel.Aluno), p => p.RequireRole(nameof(Papel.Aluno)))
    .AddPolicy(nameof(Papel.Professor), p => p.RequireRole(nameof(Papel.Professor)));

// Tentativas de login: 5 por minuto por IP, contra quem tenta adivinhar senha.
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddPolicy(ContasEndpoints.PoliticaLogin, http => RateLimitPartition.GetFixedWindowLimiter(
        http.Connection.RemoteIpAddress?.ToString() ?? "sem-ip",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));
});

// Na hospedagem o HTTPS termina no proxy: estes cabeçalhos dizem à API o esquema e o IP reais
// (Secure no cookie e limite de login por aluno, não pelo proxy).
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownIPNetworks.Clear();
    o.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseStatusCodePages();

// Migrations (schema + catálogo fictício) aplicadas ao subir: a hospedagem gratuita
// não dá shell para rodar `dotnet ef database update` à mão.
if (app.Configuration.GetValue("Banco:MigrarAoIniciar", true))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<CatalogoContext>().Database.MigrateAsync();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapOpenApi();
app.MapCursos();
app.MapContas();
app.MapAluno();
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
