using System.Security.Claims;
using CatalogoCursos.Api.Cursos;
using CatalogoCursos.Api.Dados;
using CatalogoCursos.Api.Dominio;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Contas;

public record EntrarDto(string Email, string Senha);
public record UsuarioDto(string Nome, string Email, string Papel);

/// <summary>Aula endereçada como na tela e na URL: "módulo.aula".</summary>
public record AulaMarcadaDto(int Modulo, int Aula);
public record ProgressoDto(bool Matriculado, IReadOnlyList<string> Concluidas);

public record MeuCursoDto(CursoResumoDto Curso, int TotalAulas, int Concluidas, AulaMarcadaDto? Proxima);

public record AlunoProgressoDto(string Nome, int Concluidas, DateTimeOffset? UltimaAtividade);
public record TurmaDto(string Slug, string Nome, int TotalAulas, IReadOnlyList<AlunoProgressoDto> Alunos);

public static class ContasEndpoints
{
    public const string PoliticaLogin = "login";

    private static readonly PasswordHasher<Usuario> Hasher = new();
    private static readonly string HashDeEspera = Hasher.HashPassword(null!, Guid.NewGuid().ToString());

    public static void MapContas(this IEndpointRouteBuilder app)
    {
        var conta = app.MapGroup("/api/conta").WithTags("Conta");
        conta.MapPost("/entrar", Entrar).RequireRateLimiting(PoliticaLogin)
            .WithSummary("Entra com e-mail e senha; a sessão vai num cookie HttpOnly");
        // Cast: com só HttpContext de parâmetro, o método casaria com RequestDelegate e o 204 se perderia.
        conta.MapPost("/sair", (Delegate)Sair).WithSummary("Encerra a sessão");
        conta.MapGet("/", Eu).RequireAuthorization().WithSummary("Quem está logado (401 se ninguém)");
        conta.MapGet("/cursos", MeusCursos).RequireAuthorization(nameof(Papel.Aluno))
            .WithSummary("Cursos do aluno, com o progresso e a próxima aula");

        var progresso = app.MapGroup("/api/cursos/{slug}").WithTags("Conta").RequireAuthorization(nameof(Papel.Aluno));
        progresso.MapGet("/progresso", Progresso).WithSummary("Aulas concluídas pelo aluno neste curso");
        progresso.MapPost("/matricula", Matricular).WithSummary("Matricula o aluno (idempotente; só com inscrições abertas)");
        progresso.MapPost("/progresso", Concluir).WithSummary("Marca uma aula como concluída (idempotente)");

        app.MapGet("/api/professor/turmas", Turmas).WithTags("Conta").RequireAuthorization(nameof(Papel.Professor))
            .WithSummary("Progresso de cada aluno, por curso (somente leitura)");
    }

    private static async Task<Results<Ok<UsuarioDto>, ProblemHttpResult>> Entrar(
        EntrarDto dados, CatalogoContext db, HttpContext http, CancellationToken ct)
    {
        var email = dados.Email?.Trim().ToLowerInvariant() ?? "";
        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

        // Mesma resposta e mesmo custo para e-mail inexistente e senha errada: sem conta, a senha é
        // conferida contra um hash qualquer, para o tempo de resposta não revelar quem tem cadastro.
        var confere = Hasher.VerifyHashedPassword(usuario!, usuario?.SenhaHash ?? HashDeEspera, dados.Senha ?? "");
        if (usuario is null || confere == PasswordVerificationResult.Failed)
            return TypedResults.Problem("E-mail ou senha incorretos.", statusCode: StatusCodes.Status401Unauthorized);

        var identidade = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Papel.ToString()),
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);
        await http.SignInAsync(new ClaimsPrincipal(identidade));

        return TypedResults.Ok(ParaDto(usuario.Nome, usuario.Email, usuario.Papel.ToString()));
    }

    private static async Task<NoContent> Sair(HttpContext http)
    {
        await http.SignOutAsync();
        return TypedResults.NoContent();
    }

    private static Ok<UsuarioDto> Eu(ClaimsPrincipal user) =>
        TypedResults.Ok(ParaDto(user.Identity!.Name!, user.FindFirstValue(ClaimTypes.Email)!, user.FindFirstValue(ClaimTypes.Role)!));

    private static UsuarioDto ParaDto(string nome, string email, string papel) =>
        new(nome, email, papel == nameof(Papel.Professor) ? "professor" : "aluno");

    private static int IdDe(ClaimsPrincipal user) => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static async Task<Ok<List<MeuCursoDto>>> MeusCursos(ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var id = IdDe(user);
        var cursos = await db.Cursos.AsNoTracking()
            .Where(c => db.Matriculas.Any(m => m.UsuarioId == id && m.CursoId == c.Id))
            .Include(c => c.Categoria)
            .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
            .AsSplitQuery()
            .OrderBy(c => c.NomeCompleto)
            .ToListAsync(ct);
        var concluidas = (await db.AulasConcluidas.AsNoTracking().Where(a => a.UsuarioId == id).Select(a => a.AulaId).ToListAsync(ct))
            .ToHashSet();

        var resultado = cursos.Select(c =>
        {
            var aulas = Sequencia(c);
            var proxima = aulas.FirstOrDefault(a => !concluidas.Contains(a.Id));
            return new MeuCursoDto(
                CursosEndpoints.Resumo(c), aulas.Count, aulas.Count(a => concluidas.Contains(a.Id)),
                proxima is null ? null : new AulaMarcadaDto(proxima.Modulo, proxima.Aula));
        }).ToList();

        return TypedResults.Ok(resultado);
    }

    private static async Task<Results<Ok<ProgressoDto>, NotFound>> Progresso(
        string slug, ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var id = IdDe(user);
        var curso = await CursoComAulas(slug, db, ct);
        if (curso is null)
            return TypedResults.NotFound();

        var matriculado = await db.Matriculas.AnyAsync(m => m.UsuarioId == id && m.CursoId == curso.Id, ct);
        var ids = await db.AulasConcluidas.Where(a => a.UsuarioId == id).Select(a => a.AulaId).ToListAsync(ct);
        var concluidas = Sequencia(curso).Where(a => ids.Contains(a.Id)).Select(a => $"{a.Modulo}.{a.Aula}").ToList();

        return TypedResults.Ok(new ProgressoDto(matriculado, concluidas));
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> Matricular(
        string slug, ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var id = IdDe(user);
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Slug == slug, ct);
        if (curso is null)
            return TypedResults.NotFound();
        if (await db.Matriculas.AnyAsync(m => m.UsuarioId == id && m.CursoId == curso.Id, ct))
            return TypedResults.NoContent();
        if (curso.Status != StatusCurso.InscricoesAbertas)
            return TypedResults.Problem("As inscrições deste curso não estão abertas.", statusCode: StatusCodes.Status409Conflict);

        db.Matriculas.Add(new Matricula { UsuarioId = id, CursoId = curso.Id, Em = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> Concluir(
        string slug, AulaMarcadaDto marcada, ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var id = IdDe(user);
        var curso = await CursoComAulas(slug, db, ct);
        var aula = curso is null ? null : Sequencia(curso).FirstOrDefault(a => a.Modulo == marcada.Modulo && a.Aula == marcada.Aula);
        if (aula is null)
            return TypedResults.NotFound();
        if (!await db.Matriculas.AnyAsync(m => m.UsuarioId == id && m.CursoId == curso!.Id, ct))
            return TypedResults.Problem("Matricule-se no curso antes de concluir aulas.", statusCode: StatusCodes.Status409Conflict);

        if (!await db.AulasConcluidas.AnyAsync(a => a.UsuarioId == id && a.AulaId == aula.Id, ct))
        {
            db.AulasConcluidas.Add(new AulaConcluida { UsuarioId = id, AulaId = aula.Id, Em = DateTimeOffset.UtcNow });
            await db.SaveChangesAsync(ct);
        }
        return TypedResults.NoContent();
    }

    private static async Task<Ok<List<TurmaDto>>> Turmas(CatalogoContext db, CancellationToken ct)
    {
        var matriculas = await db.Matriculas.AsNoTracking()
            .Include(m => m.Usuario)
            .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(m => m.Aulas)
            .AsSplitQuery()
            .ToListAsync(ct);
        var concluidas = await db.AulasConcluidas.AsNoTracking().ToListAsync(ct);

        var turmas = matriculas
            .GroupBy(m => m.Curso.Id)
            .Select(g =>
            {
                var curso = g.First().Curso;
                var aulas = Sequencia(curso).Select(a => a.Id).ToHashSet();
                var alunos = g.Select(m =>
                    {
                        var feitas = concluidas.Where(c => c.UsuarioId == m.UsuarioId && aulas.Contains(c.AulaId)).ToList();
                        return new AlunoProgressoDto(m.Usuario.Nome, feitas.Count, feitas.Count > 0 ? feitas.Max(f => f.Em) : null);
                    })
                    .OrderByDescending(a => a.Concluidas).ThenBy(a => a.Nome)
                    .ToList();
                return new TurmaDto(curso.Slug, curso.NomeCompleto, aulas.Count, alunos);
            })
            .OrderBy(t => t.Nome)
            .ToList();

        return TypedResults.Ok(turmas);
    }

    private static Task<Curso?> CursoComAulas(string slug, CatalogoContext db, CancellationToken ct) =>
        db.Cursos.AsNoTracking()
            .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Slug == slug, ct);

    private sealed record AulaNaSequencia(int Id, int Modulo, int Aula);

    /// <summary>As aulas do curso na ordem em que o aluno as faz.</summary>
    private static List<AulaNaSequencia> Sequencia(Curso c) =>
        c.Modulos.OrderBy(m => m.Ordem)
            .SelectMany(m => m.Aulas.OrderBy(a => a.Ordem).Select(a => new AulaNaSequencia(a.Id, m.Ordem, a.Ordem)))
            .ToList();
}
