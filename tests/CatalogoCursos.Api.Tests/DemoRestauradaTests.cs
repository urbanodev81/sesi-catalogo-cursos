using CatalogoCursos.Api.Dados;
using CatalogoCursos.Api.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogoCursos.Api.Tests;

[Collection(nameof(ApiCollection))]
public class DemoRestauradaTests(ApiFactory factory)
{
    [Fact]
    public async Task Restaurar_desfaz_o_que_os_visitantes_mudaram_e_volta_ao_seed()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogoContext>();

        // O que um visitante faria com as contas públicas: trocar o documento da Ana, apagar o
        // progresso dela, publicar um material e aparecer online.
        await db.DocumentosEnviados.ExecuteUpdateAsync(s => s.SetProperty(d => d.NomeArquivo, "trocado.pdf"));
        await db.AulasConcluidas.Where(a => a.UsuarioId == 1).ExecuteDeleteAsync();
        var curso = await db.Materiais.Select(m => m.CursoId).FirstAsync();
        db.Materiais.Add(new Material
        {
            CursoId = curso, Titulo = "Arquivo de um visitante", NomeArquivo = "x.pdf",
            TipoConteudo = "application/pdf", Tamanho = 5, Conteudo = "%PDF-"u8.ToArray(), Em = DateTimeOffset.UtcNow,
        });
        await db.SaveChangesAsync();
        await db.Usuarios.ExecuteUpdateAsync(s => s.SetProperty(u => u.UltimoAcesso, DateTimeOffset.UtcNow));

        await DemoRestaurada.Executar(db);

        Assert.Equal(6, await db.Matriculas.CountAsync());
        Assert.Equal(3, await db.AulasConcluidas.CountAsync(a => a.UsuarioId == 1));
        Assert.Equal(10, await db.AulasConcluidas.CountAsync());
        var documento = Assert.Single(await db.DocumentosEnviados.ToListAsync());
        Assert.Equal("identidade-ana.pdf", documento.NomeArquivo);
        var material = Assert.Single(await db.Materiais.ToListAsync());
        Assert.Equal("Guia de estudo do módulo 1", material.Titulo);
        Assert.True(await db.Usuarios.AllAsync(u => u.UltimoAcesso == null));
    }
}
