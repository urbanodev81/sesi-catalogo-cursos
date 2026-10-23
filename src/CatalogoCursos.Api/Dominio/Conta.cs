namespace CatalogoCursos.Api.Dominio;

public enum Papel
{
    Aluno,
    Professor,
}

public class Usuario
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Nome { get; set; }
    /// <summary>Hash do <c>PasswordHasher</c> do ASP.NET Core (PBKDF2 com sal); a senha nunca é gravada.</summary>
    public required string SenhaHash { get; set; }
    public Papel Papel { get; set; }
    /// <summary>Último sinal de presença do front (a cada minuto, com a aba aberta). Base do "online" dos colegas.</summary>
    public DateTimeOffset? UltimoAcesso { get; set; }
}

/// <summary>Aluno inscrito num curso. Nasce quando ele abre a primeira aula de um curso com inscrições abertas.</summary>
public class Matricula
{
    public int UsuarioId { get; set; }
    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public DateTimeOffset Em { get; set; }
}

/// <summary>Uma linha por aula concluída: o progresso é contar linhas, sem percentual gravado para ficar velho.</summary>
public class AulaConcluida
{
    public int UsuarioId { get; set; }
    public int AulaId { get; set; }
    public Aula Aula { get; set; } = null!;
    public DateTimeOffset Em { get; set; }
}
