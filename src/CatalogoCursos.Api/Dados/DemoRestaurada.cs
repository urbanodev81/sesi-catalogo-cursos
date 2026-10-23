using CatalogoCursos.Api.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CatalogoCursos.Api.Dados;

/// <summary>
/// Volta ao seed tudo o que os visitantes mudam com as contas de demonstração, cuja senha é pública:
/// matrículas, aulas concluídas, documentos enviados, materiais e o último acesso. Na publicação roda a
/// cada boot (<c>Demo:RestaurarAoIniciar</c>); o plano gratuito reinicia quando fica parado, então quem
/// chega encontra a demonstração limpa, e não o que outro visitante deixou.
/// </summary>
public static class DemoRestaurada
{
    public static async Task Executar(CatalogoContext db, CancellationToken ct = default)
    {
        await using var transacao = await db.Database.BeginTransactionAsync(ct);

        await db.AulasConcluidas.ExecuteDeleteAsync(ct);
        await db.Matriculas.ExecuteDeleteAsync(ct);
        await db.DocumentosEnviados.ExecuteDeleteAsync(ct);
        await db.Materiais.ExecuteDeleteAsync(ct);
        await db.Usuarios.ExecuteUpdateAsync(s => s.SetProperty(u => u.UltimoAcesso, (DateTimeOffset?)null), ct);

        // O seed é o mesmo HasData das migrations, lido do modelo de design (o modelo de execução do EF
        // não guarda o HasData): não existe uma segunda cópia dos dados para ficar desatualizada.
        var modelo = db.GetService<IDesignTimeModel>().Model;
        Semear<Matricula>(db, modelo);
        Semear<AulaConcluida>(db, modelo);
        Semear<DocumentoEnviado>(db, modelo);
        Semear<Material>(db, modelo);
        await db.SaveChangesAsync(ct);

        await transacao.CommitAsync(ct);
    }

    private static void Semear<T>(CatalogoContext db, IModel modelo) where T : class
    {
        foreach (var linha in modelo.FindEntityType(typeof(T))!.GetSeedData())
        {
            var entidade = Activator.CreateInstance<T>();
            var entrada = db.Add(entidade);
            foreach (var (campo, valor) in linha)
                // O seed tem datas em -03:00; o Npgsql só grava timestamptz em UTC.
                entrada.Property(campo).CurrentValue = valor is DateTimeOffset d ? d.ToUniversalTime() : valor;
        }
    }
}
