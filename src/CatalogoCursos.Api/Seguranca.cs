namespace CatalogoCursos.Api;

public static class Seguranca
{
    // O plugin em vlibras.gov.br redireciona para o repositório oficial do governo no jsDelivr: só o
    // caminho desse repositório é liberado, não o CDN inteiro.
    private const string VLibras =
        "https://*.vlibras.gov.br https://vlibras.gov.br https://cdn.jsdelivr.net/gh/spbgovbr-vlibras/";

    /// <summary>
    /// Content-Security-Policy. Script só da própria origem e do VLibras; nenhum script inline
    /// (o build do Vite não gera nenhum). `frame-ancestors 'none'`: o site não pode ser embutido em
    /// outra página (clickjacking). As exceções de `unsafe-eval`, `blob:` e `wasm` existem só porque o
    /// avatar do VLibras é um app Unity WebGL; sem elas, o tradutor de Libras não abre.
    /// </summary>
    public static readonly string Csp = string.Join("; ",
        "default-src 'self'",
        $"script-src 'self' 'unsafe-eval' 'wasm-unsafe-eval' {VLibras}",
        $"style-src 'self' 'unsafe-inline' {VLibras}",
        $"img-src 'self' data: blob: {VLibras}",
        $"font-src 'self' data: {VLibras}",
        $"connect-src 'self' blob: {VLibras}",
        $"media-src 'self' blob: {VLibras}",
        "worker-src 'self' blob:",
        $"frame-src {VLibras}", // o avatar de Libras abre num iframe do próprio vlibras.gov.br
        "frame-ancestors 'none'",
        "base-uri 'self'",
        "form-action 'self'",
        "object-src 'none'");
}
