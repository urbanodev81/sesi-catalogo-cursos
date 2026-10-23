using CatalogoCursos.Api.Dominio;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Dados;

public class CatalogoContext(DbContextOptions<CatalogoContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<AulaConcluida> AulasConcluidas => Set<AulaConcluida>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Categoria>(e =>
        {
            e.ToTable("categorias");
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Slug).HasMaxLength(60);
            e.Property(x => x.Nome).HasMaxLength(100);
            e.Property(x => x.Icone).HasMaxLength(120);
        });

        mb.Entity<Curso>(e =>
        {
            e.ToTable("cursos");
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Slug).HasMaxLength(120);
            e.Property(x => x.NomeCompleto).HasMaxLength(200);
            e.Property(x => x.NomeBreve).HasMaxLength(80);
            e.Property(x => x.Resumo).HasMaxLength(300);
            e.Property(x => x.PublicoAlvo).HasMaxLength(300);
            e.Property(x => x.ImagemUrl).HasMaxLength(300);
            e.Property(x => x.NomeBusca).HasMaxLength(300);
            // Enum como texto: o banco fica legível e reordenar o enum não corrompe dado.
            e.Property(x => x.Modalidade).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            e.HasOne(x => x.Categoria).WithMany().HasForeignKey(x => x.CategoriaId);
            e.HasMany(x => x.Modulos).WithOne().HasForeignKey(x => x.CursoId);
        });

        mb.Entity<Modulo>(e =>
        {
            e.ToTable("modulos");
            e.Property(x => x.Titulo).HasMaxLength(200);
            e.HasIndex(x => new { x.CursoId, x.Ordem }).IsUnique();
            e.HasMany(x => x.Aulas).WithOne().HasForeignKey(x => x.ModuloId);
        });

        mb.Entity<Aula>(e =>
        {
            e.ToTable("aulas");
            e.Property(x => x.Titulo).HasMaxLength(200);
            e.HasIndex(x => new { x.ModuloId, x.Ordem }).IsUnique();
        });

        mb.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Nome).HasMaxLength(120);
            e.Property(x => x.SenhaHash).HasMaxLength(200);
            e.Property(x => x.Papel).HasConversion<string>().HasMaxLength(20);
        });

        mb.Entity<Matricula>(e =>
        {
            e.ToTable("matriculas");
            e.HasKey(x => new { x.UsuarioId, x.CursoId });
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
            e.HasOne(x => x.Curso).WithMany().HasForeignKey(x => x.CursoId);
        });

        mb.Entity<AulaConcluida>(e =>
        {
            e.ToTable("aulas_concluidas");
            e.HasKey(x => new { x.UsuarioId, x.AulaId });
            e.HasOne<Usuario>().WithMany().HasForeignKey(x => x.UsuarioId);
            e.HasOne(x => x.Aula).WithMany().HasForeignKey(x => x.AulaId);
        });

        DadosIniciais.Aplicar(mb);
        ContasDeDemonstracao.Aplicar(mb);
    }
}
