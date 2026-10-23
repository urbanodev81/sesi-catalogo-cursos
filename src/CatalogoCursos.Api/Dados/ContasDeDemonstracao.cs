using CatalogoCursos.Api.Dominio;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Dados;

/// <summary>
/// Contas fictícias para a demonstração (senha de todas: <c>sesi2026</c>, publicada no README).
/// Os hashes foram gerados uma vez com o <c>PasswordHasher</c>: o HasData precisa de valor fixo,
/// e o sal aleatório faria cada migration nova "mudar" a senha.
/// Três alunos matriculados nos dois cursos que já começaram, com progressos diferentes, para o
/// painel do professor ter o que mostrar.
/// </summary>
public static class ContasDeDemonstracao
{
    public const string Senha = "sesi2026";

    private const string Ingles = "ingles-instrumental-manuais-tecnicos";
    private const string Normas = "leitura-interpretacao-normas-tecnicas";

    private static readonly DateTimeOffset Inicio = new(2026, 10, 5, 9, 0, 0, TimeSpan.FromHours(-3));

    public static void Aplicar(ModelBuilder mb)
    {
        mb.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Email = "aluno@exemplo.com", Nome = "Ana Souza", Papel = Papel.Aluno,
                SenhaHash = "AQAAAAIAAYagAAAAEOb51gKldn8woOxHCSHZRI1DnQRXeDjueJZt8bZ3R8lAz0As3qZOkHSshi+Fpamvzg==" },
            new Usuario { Id = 2, Email = "bruno@exemplo.com", Nome = "Bruno Lima", Papel = Papel.Aluno,
                SenhaHash = "AQAAAAIAAYagAAAAEPu3q9CXXw7l+/F2R5+biUqAuC+pDEl71uD0giWmCL5kEXSwdDoq/cwlHAfF2CPR5w==" },
            new Usuario { Id = 3, Email = "carla@exemplo.com", Nome = "Carla Mendes", Papel = Papel.Aluno,
                SenhaHash = "AQAAAAIAAYagAAAAEE5huoRfewrTExs2BLiy+VlYscDO4M4hssQ8gYQiAecqk9HoeTU2tkjRh55I2b1FyQ==" },
            new Usuario { Id = 4, Email = "professor@exemplo.com", Nome = "Marina Rocha", Papel = Papel.Professor,
                SenhaHash = "AQAAAAIAAYagAAAAEDLXTpaZWRhgbZXeY0sygV6DDAbITcS830+mCHniMcEJEm9cVKkFyHHjjgsvT0KC6w==" });

        mb.Entity<Matricula>().HasData(
            Matricula(1, Ingles, 0), Matricula(1, Normas, 2),
            Matricula(2, Ingles, 0), Matricula(2, Normas, 0),
            Matricula(3, Ingles, 7), Matricula(3, Normas, 9));

        mb.Entity<AulaConcluida>().HasData(
            // Ana: metade do inglês, um terço de normas
            Concluida(1, Ingles, 1, 1, 1), Concluida(1, Ingles, 1, 2, 3), Concluida(1, Normas, 1, 1, 4),
            // Bruno: inglês completo, normas quase
            Concluida(2, Ingles, 1, 1, 1), Concluida(2, Ingles, 1, 2, 2), Concluida(2, Ingles, 2, 1, 6),
            Concluida(2, Ingles, 2, 2, 8), Concluida(2, Normas, 1, 1, 3), Concluida(2, Normas, 1, 2, 5),
            // Carla: começou o inglês, ainda não abriu normas
            Concluida(3, Ingles, 1, 1, 8));
    }

    private static object Matricula(int usuario, string slug, int dias) =>
        new { UsuarioId = usuario, CursoId = DadosIniciais.IdCurso(slug), Em = Inicio.AddDays(dias) };

    private static object Concluida(int usuario, string slug, int modulo, int aula, int dias) =>
        new { UsuarioId = usuario, AulaId = DadosIniciais.IdAula(slug, modulo, aula), Em = Inicio.AddDays(dias) };
}
