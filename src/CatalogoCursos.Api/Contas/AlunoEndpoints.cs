using System.Security.Claims;
using CatalogoCursos.Api.Cursos;
using CatalogoCursos.Api.Dados;
using CatalogoCursos.Api.Dominio;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Contas;

public record AvaliacaoDto(int Id, string Titulo, Opcao Tipo, DateTimeOffset Em, string CursoSlug, string CursoNome);
public record DocumentoDto(int Id, string Titulo, string Orientacao, EnvioDto? Envio);
public record EnvioDto(string NomeArquivo, int Tamanho, DateTimeOffset Em);
public record ColegaDto(string Nome, bool Online, IReadOnlyList<string> Cursos);
public record PerfilDto(
    string Nome, string Email, DateTimeOffset? AlunoDesde, int Cursos, int AulasConcluidas,
    int DocumentosPendentes, AvaliacaoDto? ProximaAvaliacao);

/// <summary>
/// Painel do aluno (login de demonstração): próximas avaliações, documentos, colegas e perfil.
/// "Agora" vem do <see cref="TimeProvider"/>, para os testes fixarem o relógio em vez de depender da data.
/// </summary>
public static class AlunoEndpoints
{
    public const int TamanhoMaximo = 2 * 1024 * 1024;

    /// <summary>Sem sinal de presença há mais que isto, o colega aparece offline (o front manda um por minuto).</summary>
    public static readonly TimeSpan JanelaOnline = TimeSpan.FromMinutes(2);

    public static void MapAluno(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/conta/presenca", Presenca).WithTags("Conta").RequireAuthorization()
            .WithSummary("Sinal de presença (o front manda a cada minuto com a aba aberta)");

        var aluno = app.MapGroup("/api/aluno").WithTags("Aluno").RequireAuthorization(nameof(Papel.Aluno));
        aluno.MapGet("/avaliacoes", Avaliacoes).WithSummary("Próximas avaliações dos cursos do aluno");
        aluno.MapGet("/documentos", Documentos).WithSummary("Documentos exigidos, com o que o aluno já enviou");
        aluno.MapPost("/documentos/{id:int}", Enviar)
            // Antiforgery: o cookie é SameSite=Lax, então outro site não consegue mandar este POST com a
            // sessão; o token do ASP.NET seria uma segunda trava para o mesmo problema.
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(TamanhoMaximo + 64 * 1024))
            .WithSummary("Envia (ou substitui) o arquivo de um documento: PDF, JPG ou PNG de até 2 MB");
        aluno.MapGet("/documentos/{id:int}/arquivo", Baixar).WithSummary("Baixa o arquivo que o próprio aluno enviou");
        aluno.MapGet("/colegas", Colegas).WithSummary("Colegas com curso em comum, e quem está online");
        aluno.MapGet("/perfil", Perfil).WithSummary("Dados do aluno e um resumo da situação");
    }

    private static int IdDe(ClaimsPrincipal user) => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static async Task<NoContent> Presenca(ClaimsPrincipal user, CatalogoContext db, TimeProvider relogio, CancellationToken ct)
    {
        var id = IdDe(user);
        var agora = relogio.GetUtcNow();
        await db.Usuarios.Where(u => u.Id == id).ExecuteUpdateAsync(s => s.SetProperty(u => u.UltimoAcesso, agora), ct);
        return TypedResults.NoContent();
    }

    private static async Task<List<AvaliacaoDto>> ProximasAvaliacoes(int id, CatalogoContext db, TimeProvider relogio, CancellationToken ct)
    {
        var agora = relogio.GetUtcNow();
        var avaliacoes = await db.Avaliacoes.AsNoTracking()
            .Where(a => a.Em >= agora && db.Matriculas.Any(m => m.UsuarioId == id && m.CursoId == a.CursoId))
            .Include(a => a.Curso)
            .OrderBy(a => a.Em)
            .ToListAsync(ct);
        return avaliacoes.Select(a => new AvaliacaoDto(
            a.Id, a.Titulo, Tipo(a.Tipo), a.Em, a.Curso.Slug, a.Curso.NomeBreve)).ToList();
    }

    private static Opcao Tipo(TipoAvaliacao t)
    {
        var (valor, rotulo) = Rotulos.De(t);
        return new Opcao(valor, rotulo);
    }

    private static async Task<Ok<List<AvaliacaoDto>>> Avaliacoes(ClaimsPrincipal user, CatalogoContext db, TimeProvider relogio, CancellationToken ct) =>
        TypedResults.Ok(await ProximasAvaliacoes(IdDe(user), db, relogio, ct));

    private static async Task<Ok<List<DocumentoDto>>> Documentos(ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var id = IdDe(user);
        // Sem o conteúdo: a lista só precisa saber se há envio, não trazer os arquivos.
        var envios = await db.DocumentosEnviados.AsNoTracking()
            .Where(e => e.UsuarioId == id)
            .Select(e => new { e.DocumentoId, Envio = new EnvioDto(e.NomeArquivo, e.Tamanho, e.Em) })
            .ToDictionaryAsync(e => e.DocumentoId, e => e.Envio, ct);
        var exigidos = await db.DocumentosExigidos.AsNoTracking().OrderBy(d => d.Ordem).ToListAsync(ct);

        return TypedResults.Ok(exigidos.Select(d => new DocumentoDto(d.Id, d.Titulo, d.Orientacao, envios.GetValueOrDefault(d.Id))).ToList());
    }

    private static readonly byte[] AssinaturaPng = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] AssinaturaJpeg = [0xFF, 0xD8, 0xFF];

    /// <summary>Tipo pelo começo do arquivo: a extensão e o Content-Type são escolhas de quem envia.</summary>
    private static string? TipoReal(ReadOnlySpan<byte> inicio) =>
        inicio.StartsWith("%PDF-"u8) ? "application/pdf"
        : inicio.StartsWith(AssinaturaPng) ? "image/png"
        : inicio.StartsWith(AssinaturaJpeg) ? "image/jpeg"
        : null;

    private static async Task<Results<Ok<EnvioDto>, NotFound, ProblemHttpResult>> Enviar(
        int id, IFormFile? arquivo, ClaimsPrincipal user, CatalogoContext db, TimeProvider relogio, CancellationToken ct)
    {
        if (!await db.DocumentosExigidos.AnyAsync(d => d.Id == id, ct))
            return TypedResults.NotFound();
        if (arquivo is null || arquivo.Length == 0)
            return Invalido("Escolha um arquivo para enviar.");
        if (arquivo.Length > TamanhoMaximo)
            return Invalido("O arquivo passa de 2 MB. Envie uma versão menor.");

        using var memoria = new MemoryStream((int)arquivo.Length);
        await arquivo.CopyToAsync(memoria, ct);
        var conteudo = memoria.ToArray();
        var tipo = TipoReal(conteudo);
        if (tipo is null)
            return Invalido("Envie o documento em PDF, JPG ou PNG.");

        var usuario = IdDe(user);
        var nome = Path.GetFileName(arquivo.FileName).Trim();
        if (nome.Length is 0 or > 200)
            nome = tipo == "application/pdf" ? "documento.pdf" : tipo == "image/png" ? "documento.png" : "documento.jpg";

        var envio = await db.DocumentosEnviados.FindAsync([usuario, id], ct);
        if (envio is null)
            db.DocumentosEnviados.Add(envio = new DocumentoEnviado
            {
                UsuarioId = usuario, DocumentoId = id, NomeArquivo = nome, TipoConteudo = tipo, Conteudo = conteudo,
            });
        envio.NomeArquivo = nome;
        envio.TipoConteudo = tipo;
        envio.Conteudo = conteudo;
        envio.Tamanho = conteudo.Length;
        envio.Em = relogio.GetUtcNow();
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(new EnvioDto(envio.NomeArquivo, envio.Tamanho, envio.Em));
    }

    private static ProblemHttpResult Invalido(string mensagem) =>
        TypedResults.Problem(mensagem, statusCode: StatusCodes.Status422UnprocessableEntity);

    private static async Task<Results<FileContentHttpResult, NotFound>> Baixar(int id, ClaimsPrincipal user, CatalogoContext db, CancellationToken ct)
    {
        var usuario = IdDe(user);
        // Só o envio do próprio aluno: o id na URL é o do documento, e o dono vem da sessão.
        var envio = await db.DocumentosEnviados.AsNoTracking().FirstOrDefaultAsync(e => e.UsuarioId == usuario && e.DocumentoId == id, ct);
        return envio is null
            ? TypedResults.NotFound()
            : TypedResults.File(envio.Conteudo, envio.TipoConteudo, envio.NomeArquivo);
    }

    private static async Task<Ok<List<ColegaDto>>> Colegas(ClaimsPrincipal user, CatalogoContext db, TimeProvider relogio, CancellationToken ct)
    {
        var id = IdDe(user);
        var limite = relogio.GetUtcNow() - JanelaOnline;
        // Só nome e cursos em comum: o e-mail do colega não aparece para outro aluno.
        var linhas = await db.Matriculas.AsNoTracking()
            .Where(m => m.UsuarioId != id && m.Usuario.Papel == Papel.Aluno
                && db.Matriculas.Any(minha => minha.UsuarioId == id && minha.CursoId == m.CursoId))
            .Select(m => new { m.UsuarioId, m.Usuario.Nome, m.Usuario.UltimoAcesso, Curso = m.Curso.NomeBreve })
            .ToListAsync(ct);

        var colegas = linhas
            .GroupBy(l => l.UsuarioId)
            .Select(g => new ColegaDto(
                g.First().Nome, g.First().UltimoAcesso >= limite, g.Select(l => l.Curso).Order().ToList()))
            .OrderByDescending(c => c.Online).ThenBy(c => c.Nome)
            .ToList();
        return TypedResults.Ok(colegas);
    }

    private static async Task<Ok<PerfilDto>> Perfil(ClaimsPrincipal user, CatalogoContext db, TimeProvider relogio, CancellationToken ct)
    {
        var id = IdDe(user);
        var matriculas = await db.Matriculas.AsNoTracking().Where(m => m.UsuarioId == id).Select(m => m.Em).ToListAsync(ct);
        var aulas = await db.AulasConcluidas.CountAsync(a => a.UsuarioId == id, ct);
        var enviados = await db.DocumentosEnviados.CountAsync(e => e.UsuarioId == id, ct);
        var exigidos = await db.DocumentosExigidos.CountAsync(ct);
        var proxima = (await ProximasAvaliacoes(id, db, relogio, ct)).FirstOrDefault();

        return TypedResults.Ok(new PerfilDto(
            user.Identity!.Name!, user.FindFirstValue(ClaimTypes.Email)!,
            matriculas.Count > 0 ? matriculas.Min() : null, matriculas.Count, aulas, exigidos - enviados, proxima));
    }
}
