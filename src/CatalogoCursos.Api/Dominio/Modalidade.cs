namespace CatalogoCursos.Api.Dominio;

public enum Modalidade
{
    Ead,
    Presencial,
    Hibrido,
}

public enum StatusCurso
{
    InscricoesAbertas,
    EmBreve,
    Encerrado,
}

/// <summary>
/// Valor (o que trafega na URL e no JSON) e rótulo (o que a tela mostra) de cada enum.
/// Um lugar só, para a API e o filtro nunca divergirem.
/// </summary>
public static class Rotulos
{
    private static readonly (Modalidade Item, string Valor, string Rotulo)[] Modalidades =
    [
        (Modalidade.Ead, "ead", "EAD"),
        (Modalidade.Presencial, "presencial", "Presencial"),
        (Modalidade.Hibrido, "hibrido", "Híbrido"),
    ];

    private static readonly (StatusCurso Item, string Valor, string Rotulo)[] Status =
    [
        (StatusCurso.InscricoesAbertas, "inscricoes-abertas", "Inscrições abertas"),
        (StatusCurso.EmBreve, "em-breve", "Em breve"),
        (StatusCurso.Encerrado, "encerrado", "Encerrado"),
    ];

    private static readonly (TipoAvaliacao Item, string Valor, string Rotulo)[] TiposAvaliacao =
    [
        (TipoAvaliacao.Prova, "prova", "Prova"),
        (TipoAvaliacao.Trabalho, "trabalho", "Trabalho"),
        (TipoAvaliacao.Questionario, "questionario", "Questionário"),
    ];

    public static (string Valor, string Rotulo) De(TipoAvaliacao t) =>
        TiposAvaliacao.Where(x => x.Item == t).Select(x => (x.Valor, x.Rotulo)).Single();

    public static IEnumerable<(string Valor, string Rotulo)> TodasModalidades =>
        Modalidades.Select(m => (m.Valor, m.Rotulo));

    public static IEnumerable<(string Valor, string Rotulo)> TodosStatus =>
        Status.Select(s => (s.Valor, s.Rotulo));

    public static (string Valor, string Rotulo) De(Modalidade m) =>
        Modalidades.Where(x => x.Item == m).Select(x => (x.Valor, x.Rotulo)).Single();

    public static (string Valor, string Rotulo) De(StatusCurso s) =>
        Status.Where(x => x.Item == s).Select(x => (x.Valor, x.Rotulo)).Single();

    public static bool TentarModalidade(string valor, out Modalidade modalidade)
    {
        var achou = Modalidades.FirstOrDefault(x => x.Valor == valor);
        modalidade = achou.Item;
        return achou.Valor is not null;
    }

    public static bool TentarStatus(string valor, out StatusCurso status)
    {
        var achou = Status.FirstOrDefault(x => x.Valor == valor);
        status = achou.Item;
        return achou.Valor is not null;
    }
}
