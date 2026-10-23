using CatalogoCursos.Api.Dados;
using CatalogoCursos.Api.Dominio;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CatalogoCursos.Api.Contas;

/// <summary>Encontro semanal: dia por extenso e horas em "HH:mm", prontos para a tela.</summary>
public record HorarioDto(string Dia, string Inicio, string Fim, string Descricao);
public record MaterialDto(int Id, string Titulo, string NomeArquivo, int Tamanho, DateTimeOffset Em, string CursoSlug, string CursoNome);
public record AulaFeitaDto(int Modulo, int Aula, string Titulo, DateTimeOffset Em);
public record AlunoProgressoDto(
    string Nome, int Concluidas, DateTimeOffset? UltimaAtividade, IReadOnlyList<AulaFeitaDto> Historico,
    int DocumentosEntregues, int DocumentosExigidos);
public record TurmaDto(
    string Slug, string Nome, int TotalAulas, IReadOnlyList<AlunoProgressoDto> Alunos,
    IReadOnlyList<HorarioDto> Horarios, IReadOnlyList<MaterialDto> Materiais);

/// <summary>
/// Área do professor (login de demonstração): turmas com horários, materiais e o histórico de cada
/// aluno. Os materiais são a única escrita do professor; ele vê se o aluno entregou os documentos, mas
/// não os arquivos pessoais.
/// </summary>
public static class ProfessorEndpoints
{
    public const int MateriaisPorTurma = 10;

    public static void MapProfessor(this IEndpointRouteBuilder app)
    {
        var professor = app.MapGroup("/api/professor").WithTags("Professor").RequireAuthorization(nameof(Papel.Professor));
        professor.MapGet("/turmas", Turmas).WithSummary("Turmas com horários, materiais e o histórico de cada aluno");
        professor.MapPost("/turmas/{slug}/materiais", Publicar)
            .DisableAntiforgery() // mesmo motivo do envio de documentos: cookie SameSite=Lax
            .WithMetadata(new RequestSizeLimitAttribute(Arquivos.TamanhoMaximo + 64 * 1024))
            .WithSummary("Publica um material (PDF, JPG ou PNG de até 2 MB) para a turma");
        professor.MapDelete("/materiais/{id:int}", Remover).WithSummary("Remove um material");

        app.MapGet("/api/aluno/materiais", MateriaisDoAluno).WithTags("Aluno").RequireAuthorization(nameof(Papel.Aluno))
            .WithSummary("Materiais publicados nos cursos do aluno, do mais novo para o mais antigo");
        app.MapGet("/api/materiais/{id:int}/arquivo", Baixar).WithTags("Aluno").RequireAuthorization()
            .WithSummary("Baixa um material (professor, ou aluno matriculado no curso)");
    }

    private static int IdDe(ClaimsPrincipal user) => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static readonly string[] Dias =
        ["Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado"];

    /// <summary>Horários por curso, de segunda a domingo (o DayOfWeek do .NET começa no domingo).</summary>
    internal static async Task<Dictionary<int, List<HorarioDto>>> HorariosDe(IReadOnlyCollection<int> cursos, CatalogoContext db, CancellationToken ct)
    {
        var horarios = await db.Horarios.AsNoTracking().Where(h => cursos.Contains(h.CursoId)).ToListAsync(ct);
        return horarios
            .GroupBy(h => h.CursoId)
            .ToDictionary(g => g.Key, g => g
                .OrderBy(h => ((int)h.Dia + 6) % 7).ThenBy(h => h.Inicio)
                .Select(h => new HorarioDto(Dias[(int)h.Dia], h.Inicio.ToString("HH:mm"), h.Fim.ToString("HH:mm"), h.Descricao))
                .ToList());
    }

    /// <summary>Sem o conteúdo: a lista só precisa dos dados do arquivo.</summary>
    private static IQueryable<MaterialDto> Listar(IQueryable<Material> materiais) =>
        materiais.OrderByDescending(m => m.Em)
            .Select(m => new MaterialDto(m.Id, m.Titulo, m.NomeArquivo, m.Tamanho, m.Em, m.Curso.Slug, m.Curso.NomeBreve));

    private static async Task<Ok<List<TurmaDto>>> Turmas(CatalogoContext db, CancellationToken ct)
    {
        var matriculas = await db.Matriculas.AsNoTracking()
            .Include(m => m.Usuario)
            .Include(m => m.Curso).ThenInclude(c => c.Modulos).ThenInclude(m => m.Aulas)
            .AsSplitQuery()
            .ToListAsync(ct);
        var concluidas = await db.AulasConcluidas.AsNoTracking().ToListAsync(ct);
        var entregues = await db.DocumentosEnviados.AsNoTracking()
            .GroupBy(e => e.UsuarioId).Select(g => new { Usuario = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.Usuario, x => x.Total, ct);
        var exigidos = await db.DocumentosExigidos.CountAsync(ct);
        var cursos = matriculas.Select(m => m.CursoId).Distinct().ToList();
        var horarios = await HorariosDe(cursos, db, ct);
        var materiais = (await Listar(db.Materiais.AsNoTracking().Where(m => cursos.Contains(m.CursoId))).ToListAsync(ct))
            .ToLookup(m => m.CursoSlug);

        var turmas = matriculas
            .GroupBy(m => m.Curso.Id)
            .Select(g =>
            {
                var curso = g.First().Curso;
                // Título de cada aula por id, para o histórico dizer "1.2 Requisito e recomendação".
                var aulas = curso.Modulos.SelectMany(m => m.Aulas.Select(a => (a.Id, Modulo: m.Ordem, a.Ordem, a.Titulo)))
                    .ToDictionary(a => a.Id);
                var alunos = g.Select(m =>
                    {
                        var historico = concluidas
                            .Where(c => c.UsuarioId == m.UsuarioId && aulas.ContainsKey(c.AulaId))
                            .OrderByDescending(c => c.Em)
                            .Select(c => new AulaFeitaDto(aulas[c.AulaId].Modulo, aulas[c.AulaId].Ordem, aulas[c.AulaId].Titulo, c.Em))
                            .ToList();
                        return new AlunoProgressoDto(
                            m.Usuario.Nome, historico.Count, historico.FirstOrDefault()?.Em, historico,
                            entregues.GetValueOrDefault(m.UsuarioId), exigidos);
                    })
                    .OrderByDescending(a => a.Concluidas).ThenBy(a => a.Nome)
                    .ToList();
                return new TurmaDto(curso.Slug, curso.NomeCompleto, aulas.Count, alunos,
                    horarios.GetValueOrDefault(curso.Id, []), materiais[curso.Slug].ToList());
            })
            .OrderBy(t => t.Nome)
            .ToList();

        return TypedResults.Ok(turmas);
    }

    private static async Task<Results<Created<MaterialDto>, NotFound, ProblemHttpResult>> Publicar(
        string slug, [FromForm] string? titulo, IFormFile? arquivo, CatalogoContext db, TimeProvider relogio, CancellationToken ct)
    {
        // Só curso com turma (alguém matriculado): é a lista que o professor vê.
        var curso = await db.Cursos.FirstOrDefaultAsync(c => c.Slug == slug && db.Matriculas.Any(m => m.CursoId == c.Id), ct);
        if (curso is null)
            return TypedResults.NotFound();

        titulo = titulo?.Trim() ?? "";
        if (titulo.Length is < 3 or > 120)
            return AlunoEndpoints.Invalido("Dê um título de 3 a 120 caracteres ao material.");
        var (conteudo, tipo, erro) = await Arquivos.Ler(arquivo, ct);
        if (erro is not null)
            return AlunoEndpoints.Invalido(erro);
        if (await db.Materiais.CountAsync(m => m.CursoId == curso.Id, ct) >= MateriaisPorTurma)
            return TypedResults.Problem($"A turma já tem {MateriaisPorTurma} materiais. Remova um antes de publicar outro.",
                statusCode: StatusCodes.Status409Conflict);

        var material = new Material
        {
            CursoId = curso.Id, Titulo = titulo, NomeArquivo = Arquivos.NomeSeguro(arquivo!.FileName, tipo!),
            TipoConteudo = tipo!, Tamanho = conteudo!.Length, Conteudo = conteudo, Em = relogio.GetUtcNow(),
        };
        db.Materiais.Add(material);
        await db.SaveChangesAsync(ct);

        var dto = new MaterialDto(material.Id, material.Titulo, material.NomeArquivo, material.Tamanho, material.Em, curso.Slug, curso.NomeBreve);
        return TypedResults.Created($"/api/materiais/{material.Id}/arquivo", dto);
    }

    private static async Task<Results<NoContent, NotFound>> Remover(int id, CatalogoContext db, CancellationToken ct) =>
        await db.Materiais.Where(m => m.Id == id).ExecuteDeleteAsync(ct) == 0 ? TypedResults.NotFound() : TypedResults.NoContent();

    private static async Task<Ok<List<MaterialDto>>> MateriaisDoAluno(ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var id = IdDe(user);
        var materiais = db.Materiais.AsNoTracking().Where(m => db.Matriculas.Any(x => x.UsuarioId == id && x.CursoId == m.CursoId));
        return TypedResults.Ok(await Listar(materiais).ToListAsync(ct));
    }

    private static async Task<Results<FileContentHttpResult, NotFound>> Baixar(int id, ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var usuario = IdDe(user);
        var professor = user.IsInRole(nameof(Papel.Professor));
        // Aluno só baixa material de curso em que está matriculado; para os outros, o material "não existe".
        var material = await db.Materiais.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && (professor || db.Matriculas.Any(x => x.UsuarioId == usuario && x.CursoId == m.CursoId)), ct);
        return material is null ? TypedResults.NotFound() : TypedResults.File(material.Conteudo, material.TipoConteudo, material.NomeArquivo);
    }
}
