using System.Globalization;
using System.Text;

namespace CatalogoCursos.Api.Dominio;

public class Categoria
{
    public int Id { get; set; }
    public required string Slug { get; set; }
    public required string Nome { get; set; }
    /// <summary>Arquivo do ícone em <c>/assets/icones</c>, vindo do material do Figma.</summary>
    public required string Icone { get; set; }
}

public class Curso
{
    public int Id { get; set; }
    public required string Slug { get; set; }
    public required string NomeCompleto { get; set; }
    public required string NomeBreve { get; set; }
    public required string Descricao { get; set; }
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public Modalidade Modalidade { get; set; }
    public int CargaHorariaHoras { get; set; }
    public required string PublicoAlvo { get; set; }
    public StatusCurso Status { get; set; }
    public required string ImagemUrl { get; set; }
    /// <summary>Só faz sentido para curso que ainda não abriu (status "em breve").</summary>
    public DateOnly? DataDisponibilizacao { get; set; }

    /// <summary>
    /// Nome completo + breve, minúsculo e sem acento. A busca compara contra esta
    /// coluna, então "matematica" acha "Matemática" em qualquer banco, sem extensão.
    /// </summary>
    public required string NomeBusca { get; set; }

    public List<Modulo> Modulos { get; set; } = [];
}

public class Modulo
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public int Ordem { get; set; }
    public required string Titulo { get; set; }
    public List<Aula> Aulas { get; set; } = [];
}

public class Aula
{
    public int Id { get; set; }
    public int ModuloId { get; set; }
    public int Ordem { get; set; }
    public required string Titulo { get; set; }
    public required string Descricao { get; set; }
}

public static class TextoBusca
{
    public static string Normalizar(string texto)
    {
        var decomposto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposto.Length);
        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
