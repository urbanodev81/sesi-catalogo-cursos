namespace CatalogoCursos.Api.Dominio;

public enum TipoAvaliacao
{
    Prova,
    Trabalho,
    Questionario,
}

/// <summary>Avaliação marcada num curso; o aluno vê as futuras dos cursos em que está matriculado.</summary>
public class Avaliacao
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public required string Titulo { get; set; }
    public TipoAvaliacao Tipo { get; set; }
    public DateTimeOffset Em { get; set; }
}

/// <summary>Documento que todo aluno precisa entregar (identidade, escolaridade...).</summary>
public class DocumentoExigido
{
    public int Id { get; set; }
    public int Ordem { get; set; }
    public required string Titulo { get; set; }
    /// <summary>O que serve como esse documento, mostrado ao lado do botão de envio.</summary>
    public required string Orientacao { get; set; }
}

/// <summary>
/// Arquivo que o aluno enviou para um documento exigido. Um por documento: enviar de novo substitui.
/// Fica no próprio Postgres (bytea): o disco da hospedagem gratuita é apagado a cada reinício, e o
/// volume é pequeno (só as contas de demonstração, 2 MB por arquivo, 4 documentos).
/// </summary>
public class DocumentoEnviado
{
    public int UsuarioId { get; set; }
    public int DocumentoId { get; set; }
    public DocumentoExigido Documento { get; set; } = null!;
    public required string NomeArquivo { get; set; }
    /// <summary>Tipo detectado pelos primeiros bytes do arquivo, nunca o informado pelo navegador.</summary>
    public required string TipoConteudo { get; set; }
    public int Tamanho { get; set; }
    public required byte[] Conteudo { get; set; }
    public DateTimeOffset Em { get; set; }
}

/// <summary>Encontro fixo da turma na semana (aula ao vivo, plantão de dúvidas).</summary>
public class HorarioTurma
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public DayOfWeek Dia { get; set; }
    public TimeOnly Inicio { get; set; }
    public TimeOnly Fim { get; set; }
    /// <summary>O que é e onde acontece, ex.: "Aula ao vivo, no ambiente virtual".</summary>
    public required string Descricao { get; set; }
}

/// <summary>
/// Arquivo que o professor publica para a turma de um curso. Guardado no banco pelo mesmo motivo do
/// <see cref="DocumentoEnviado"/>; o volume é limitado (2 MB por arquivo, 10 por turma).
/// </summary>
public class Material
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public required string Titulo { get; set; }
    public required string NomeArquivo { get; set; }
    public required string TipoConteudo { get; set; }
    public int Tamanho { get; set; }
    public required byte[] Conteudo { get; set; }
    public DateTimeOffset Em { get; set; }
}
