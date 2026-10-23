using CatalogoCursos.Api.Dados;
using CatalogoCursos.Api.Dominio;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Cursos;

public record Opcao(string Valor, string Rotulo);
public record CategoriaDto(string Valor, string Rotulo, string Icone);
public record FiltrosDto(IReadOnlyList<CategoriaDto> Categorias, IReadOnlyList<Opcao> Modalidades, IReadOnlyList<Opcao> Status);

public record CursoResumoDto(
    int Id, string Slug, string NomeCompleto, string NomeBreve, string Resumo, string Descricao,
    CategoriaDto Categoria, Opcao Modalidade, int CargaHorariaHoras, string PublicoAlvo,
    Opcao Status, string ImagemUrl, DateOnly? DataDisponibilizacao);

public record ListaCursosDto(int Total, IReadOnlyList<CursoResumoDto> Itens);

public record AulaDto(int Ordem, string Titulo, string Descricao);
public record ModuloDto(int Ordem, string Titulo, IReadOnlyList<AulaDto> Aulas);
public record CursoDetalheDto(CursoResumoDto Curso, IReadOnlyList<ModuloDto> Modulos);

/// <summary>Aula endereçada pela ordem (módulo 2, aula 1), que é o que a tela mostra e o que fica estável na URL.</summary>
public record AulaRefDto(int Modulo, int Aula, string Titulo);
public record AulaConteudoDto(
    string CursoSlug, string CursoNome, int Modulo, string ModuloTitulo, int Aula, string Titulo,
    string Descricao, string Conteudo, AulaRefDto? Anterior, AulaRefDto? Proxima);

public static class CursosEndpoints
{
    public const int TamanhoMaximoBusca = 100;

    public static void MapCursos(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api").WithTags("Catálogo");
        api.MapGet("/cursos", Listar).WithSummary("Lista cursos com busca por nome e filtros combináveis");
        api.MapGet("/cursos/{slug}", Detalhar).WithSummary("Detalhe de um curso, com módulos e aulas");
        api.MapGet("/cursos/{slug}/modulos/{modulo:int}/aulas/{aula:int}", Aula)
            .WithSummary("Conteúdo de uma aula, com a anterior e a próxima do curso");
        api.MapGet("/filtros", Filtros).WithSummary("Opções válidas de categoria, modalidade e status");
    }

    private static async Task<Results<Ok<ListaCursosDto>, ValidationProblem>> Listar(
        CatalogoContext db, CancellationToken ct,
        string? busca = null, string? categoria = null, string? modalidade = null, string? status = null)
    {
        var erros = new Dictionary<string, string[]>();
        var consulta = db.Cursos.AsNoTracking().Include(c => c.Categoria).AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            if (busca.Length > TamanhoMaximoBusca)
                erros["busca"] = [$"A busca aceita até {TamanhoMaximoBusca} caracteres."];
            var termo = TextoBusca.Normalizar(busca);
            consulta = consulta.Where(c => c.NomeBusca.Contains(termo));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            if (!await db.Categorias.AnyAsync(c => c.Slug == categoria, ct))
                erros["categoria"] = [$"Categoria '{categoria}' não existe."];
            consulta = consulta.Where(c => c.Categoria.Slug == categoria);
        }

        if (!string.IsNullOrWhiteSpace(modalidade))
        {
            if (Rotulos.TentarModalidade(modalidade, out var m))
                consulta = consulta.Where(c => c.Modalidade == m);
            else
                erros["modalidade"] = [$"Modalidade '{modalidade}' não existe."];
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Rotulos.TentarStatus(status, out var s))
                consulta = consulta.Where(c => c.Status == s);
            else
                erros["status"] = [$"Status '{status}' não existe."];
        }

        if (erros.Count > 0)
            return TypedResults.ValidationProblem(erros, title: "Filtro inválido");

        // Aberto primeiro, depois o que vem, por último o encerrado; dentro de cada um, por nome.
        var cursos = await consulta
            .OrderBy(c => c.Status == StatusCurso.InscricoesAbertas ? 0 : c.Status == StatusCurso.EmBreve ? 1 : 2)
            .ThenBy(c => c.NomeCompleto)
            .ToListAsync(ct);

        return TypedResults.Ok(new ListaCursosDto(cursos.Count, cursos.Select(Resumo).ToList()));
    }

    private static async Task<Results<Ok<CursoDetalheDto>, NotFound>> Detalhar(
        string slug, CatalogoContext db, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking()
            .Include(c => c.Categoria)
            .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Slug == slug, ct);

        if (curso is null)
            return TypedResults.NotFound();

        var modulos = curso.Modulos.OrderBy(m => m.Ordem)
            .Select(m => new ModuloDto(m.Ordem, m.Titulo,
                m.Aulas.OrderBy(a => a.Ordem).Select(a => new AulaDto(a.Ordem, a.Titulo, a.Descricao)).ToList()))
            .ToList();

        return TypedResults.Ok(new CursoDetalheDto(Resumo(curso), modulos));
    }

    private static async Task<Results<Ok<AulaConteudoDto>, NotFound>> Aula(
        string slug, int modulo, int aula, CatalogoContext db, CancellationToken ct)
    {
        var curso = await db.Cursos.AsNoTracking()
            .Include(c => c.Modulos).ThenInclude(m => m.Aulas)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Slug == slug, ct);

        // O curso inteiro tem poucas aulas: achatar em memória dá a anterior e a próxima
        // atravessando a fronteira dos módulos sem consulta extra.
        var sequencia = curso?.Modulos.OrderBy(m => m.Ordem)
            .SelectMany(m => m.Aulas.OrderBy(a => a.Ordem).Select(a => (Modulo: m, Aula: a)))
            .ToList() ?? [];
        var i = sequencia.FindIndex(x => x.Modulo.Ordem == modulo && x.Aula.Ordem == aula);
        if (i < 0)
            return TypedResults.NotFound();

        AulaRefDto? Ref(int j) => j >= 0 && j < sequencia.Count
            ? new AulaRefDto(sequencia[j].Modulo.Ordem, sequencia[j].Aula.Ordem, sequencia[j].Aula.Titulo)
            : null;

        var (m, a) = sequencia[i];
        return TypedResults.Ok(new AulaConteudoDto(
            curso!.Slug, curso.NomeCompleto, m.Ordem, m.Titulo, a.Ordem, a.Titulo,
            a.Descricao, a.Conteudo, Ref(i - 1), Ref(i + 1)));
    }

    private static async Task<Ok<FiltrosDto>> Filtros(CatalogoContext db, CancellationToken ct)
    {
        var categorias = await db.Categorias.AsNoTracking().OrderBy(c => c.Nome)
            .Select(c => new CategoriaDto(c.Slug, c.Nome, c.Icone))
            .ToListAsync(ct);

        return TypedResults.Ok(new FiltrosDto(
            categorias,
            Rotulos.TodasModalidades.Select(m => new Opcao(m.Valor, m.Rotulo)).ToList(),
            Rotulos.TodosStatus.Select(s => new Opcao(s.Valor, s.Rotulo)).ToList()));
    }

    private static CursoResumoDto Resumo(Curso c)
    {
        var modalidade = Rotulos.De(c.Modalidade);
        var status = Rotulos.De(c.Status);
        return new CursoResumoDto(
            c.Id, c.Slug, c.NomeCompleto, c.NomeBreve, c.Resumo, c.Descricao,
            new CategoriaDto(c.Categoria.Slug, c.Categoria.Nome, c.Categoria.Icone),
            new Opcao(modalidade.Valor, modalidade.Rotulo), c.CargaHorariaHoras, c.PublicoAlvo,
            new Opcao(status.Valor, status.Rotulo), c.ImagemUrl, c.DataDisponibilizacao);
    }
}
