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

        // Avaliações dos dois cursos com alunos: uma já passada (some da lista) e as próximas.
        mb.Entity<Avaliacao>().HasData(
            Avaliacao(1, Ingles, "Questionário do módulo 1: vocabulário técnico", TipoAvaliacao.Questionario, 2026, 10, 15),
            Avaliacao(2, Ingles, "Tradução de um trecho de manual de máquina", TipoAvaliacao.Trabalho, 2026, 11, 6),
            Avaliacao(3, Ingles, "Prova final de inglês instrumental", TipoAvaliacao.Prova, 2026, 12, 4),
            Avaliacao(4, Normas, "Questionário: estrutura de uma norma", TipoAvaliacao.Questionario, 2026, 10, 30),
            Avaliacao(5, Normas, "Análise de uma norma do seu setor", TipoAvaliacao.Trabalho, 2026, 11, 20),
            Avaliacao(6, Normas, "Prova final de normas técnicas", TipoAvaliacao.Prova, 2026, 12, 11));

        mb.Entity<DocumentoExigido>().HasData(
            new DocumentoExigido { Id = 1, Ordem = 1, Titulo = "Documento de identidade",
                Orientacao = "RG, CNH ou outro documento oficial com foto, frente e verso." },
            new DocumentoExigido { Id = 2, Ordem = 2, Titulo = "CPF",
                Orientacao = "Pode ser o próprio documento de identidade, se trouxer o número do CPF." },
            new DocumentoExigido { Id = 3, Ordem = 3, Titulo = "Comprovante de escolaridade",
                Orientacao = "Certificado ou histórico da última etapa concluída." },
            new DocumentoExigido { Id = 4, Ordem = 4, Titulo = "Declaração de vínculo com a indústria",
                Orientacao = "Emitida pela empresa onde trabalha, com data dos últimos 90 dias." });

        // Os dois cursos com turma são EAD: os horários são encontros ao vivo e plantão, online.
        mb.Entity<HorarioTurma>().HasData(
            Horario(1, Ingles, DayOfWeek.Tuesday, 19, 21, "Aula ao vivo, no ambiente virtual"),
            Horario(2, Ingles, DayOfWeek.Thursday, 19, 21, "Aula ao vivo, no ambiente virtual"),
            Horario(3, Normas, DayOfWeek.Monday, 19, 21, "Aula ao vivo, no ambiente virtual"),
            Horario(4, Normas, DayOfWeek.Saturday, 9, 11, "Plantão de dúvidas, por videochamada"));

        mb.Entity<Material>().HasData(new
        {
            Id = 1, CursoId = DadosIniciais.IdCurso(Normas), Titulo = "Guia de estudo do módulo 1",
            NomeArquivo = "guia-modulo-1.pdf", TipoConteudo = "application/pdf", Tamanho = PdfDeExemplo.Length,
            Conteudo = PdfDeExemplo, Em = Inicio.AddDays(2),
        });

        // A Ana já entregou a identidade; os outros três aparecem como pendentes.
        mb.Entity<DocumentoEnviado>().HasData(new
        {
            UsuarioId = 1, DocumentoId = 1, NomeArquivo = "identidade-ana.pdf", TipoConteudo = "application/pdf",
            Tamanho = PdfDeExemplo.Length, Conteudo = PdfDeExemplo, Em = Inicio.AddDays(1),
        });
    }

    /// <summary>PDF mínimo válido (uma página em branco), para o download do seed abrir de verdade.</summary>
    private static readonly byte[] PdfDeExemplo = System.Text.Encoding.ASCII.GetBytes(
        "%PDF-1.4\n1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj\n" +
        "3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 595 842]>>endobj\ntrailer<</Root 1 0 R>>\n%%EOF\n");

    private static object Horario(int id, string slug, DayOfWeek dia, int de, int ate, string descricao) =>
        new { Id = id, CursoId = DadosIniciais.IdCurso(slug), Dia = dia, Inicio = new TimeOnly(de, 0), Fim = new TimeOnly(ate, 0), Descricao = descricao };

    private static object Avaliacao(int id, string slug, string titulo, TipoAvaliacao tipo, int ano, int mes, int dia) =>
        new { Id = id, CursoId = DadosIniciais.IdCurso(slug), Titulo = titulo, Tipo = tipo,
            Em = new DateTimeOffset(ano, mes, dia, 19, 0, 0, TimeSpan.FromHours(-3)) };

    private static object Matricula(int usuario, string slug, int dias) =>
        new { UsuarioId = usuario, CursoId = DadosIniciais.IdCurso(slug), Em = Inicio.AddDays(dias) };

    private static object Concluida(int usuario, string slug, int modulo, int aula, int dias) =>
        new { UsuarioId = usuario, AulaId = DadosIniciais.IdAula(slug, modulo, aula), Em = Inicio.AddDays(dias) };
}
