namespace CatalogoCursos.Api.Contas;

/// <summary>Regras comuns aos arquivos enviados (documentos do aluno e materiais do professor).</summary>
public static class Arquivos
{
    public const int TamanhoMaximo = 2 * 1024 * 1024;

    private static readonly byte[] AssinaturaPng = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] AssinaturaJpeg = [0xFF, 0xD8, 0xFF];

    /// <summary>Tipo pelo começo do arquivo: a extensão e o Content-Type são escolhas de quem envia.</summary>
    public static string? TipoReal(ReadOnlySpan<byte> inicio) =>
        inicio.StartsWith("%PDF-"u8) ? "application/pdf"
        : inicio.StartsWith(AssinaturaPng) ? "image/png"
        : inicio.StartsWith(AssinaturaJpeg) ? "image/jpeg"
        : null;

    /// <summary>Só o nome, sem caminho; nome vazio ou longo demais vira um genérico com a extensão certa.</summary>
    public static string NomeSeguro(string? enviado, string tipo)
    {
        var nome = Path.GetFileName(enviado ?? "").Trim();
        return nome.Length is > 0 and <= 200 ? nome
            : tipo == "application/pdf" ? "arquivo.pdf" : tipo == "image/png" ? "arquivo.png" : "arquivo.jpg";
    }

    /// <summary>Lê e valida o arquivo; devolve o conteúdo e o tipo, ou a mensagem para o usuário.</summary>
    public static async Task<(byte[]? Conteudo, string? Tipo, string? Erro)> Ler(IFormFile? arquivo, CancellationToken ct)
    {
        if (arquivo is null || arquivo.Length == 0)
            return (null, null, "Escolha um arquivo para enviar.");
        if (arquivo.Length > TamanhoMaximo)
            return (null, null, "O arquivo passa de 2 MB. Envie uma versão menor.");

        using var memoria = new MemoryStream((int)arquivo.Length);
        await arquivo.CopyToAsync(memoria, ct);
        var conteudo = memoria.ToArray();
        var tipo = TipoReal(conteudo);
        return tipo is null ? (null, null, "Envie o arquivo em PDF, JPG ou PNG.") : (conteudo, tipo, null);
    }
}
