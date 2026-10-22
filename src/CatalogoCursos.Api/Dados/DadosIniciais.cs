using CatalogoCursos.Api.Dominio;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Dados;

/// <summary>
/// Catálogo fictício, semeado pela migration (HasData): o script SQL gerado por
/// <c>dotnet ef migrations script</c> sai com schema e dados, e o banco nasce
/// igual em qualquer máquina. Ids fixos e sequenciais — HasData exige.
/// </summary>
public static class DadosIniciais
{
    private const string Img = "/assets/imagens/";

    private static readonly Categoria[] Categorias =
    [
        new() { Id = 1, Slug = "matematica", Nome = "Matemática", Icone = "icon-matematica-20px.svg" },
        new() { Id = 2, Slug = "linguagens", Nome = "Linguagens", Icone = "icon-linguagens-20px.svg" },
        new() { Id = 3, Slug = "ciencias-da-natureza", Nome = "Ciências da Natureza", Icone = "icon-ciencias-natureza-20px.svg" },
        new() { Id = 4, Slug = "sustentabilidade", Nome = "Sustentabilidade", Icone = "icon-sustentabilidade-20px.svg" },
    ];

    private sealed record AulaSpec(string Titulo, string Descricao);
    private sealed record ModuloSpec(string Titulo, AulaSpec[] Aulas);
    private sealed record CursoSpec(
        string Slug, string NomeCompleto, string NomeBreve, string Descricao, int CategoriaId,
        Modalidade Modalidade, int CargaHoraria, string PublicoAlvo, StatusCurso Status,
        string Imagem, DateOnly? Disponibilizacao, ModuloSpec[] Modulos);

    private static readonly CursoSpec[] Cursos =
    [
        new("matematica-aplicada-industria-4-0", "Matemática Aplicada à Indústria 4.0", "Matemática 4.0",
            "Razão, proporção, funções e leitura de gráficos aplicadas a indicadores de produção, sensores e automação.",
            1, Modalidade.Ead, 40, "Trabalhadores da indústria com ensino médio em andamento ou concluído.",
            StatusCurso.InscricoesAbertas, "hero-image.webp", null,
            [
                new("Números no chão de fábrica", [
                    new("Razão e proporção na produção", "Como calcular rendimento, perdas e consumo por peça."),
                    new("Porcentagem e indicadores", "Leitura de OEE, refugo e meta de produção."),
                ]),
                new("Funções e sensores", [
                    new("Função afim na calibração", "Relação linear entre leitura do sensor e grandeza medida."),
                    new("Gráficos de tendência", "Interpretar séries de dados vindas de máquinas conectadas."),
                    new("Estudo de caso", "Montar um painel simples de acompanhamento de linha."),
                ]),
            ]),
        new("estatistica-controle-qualidade", "Estatística para Controle de Qualidade", "Estatística na Qualidade",
            "Amostragem, média, desvio-padrão e cartas de controle para acompanhar a estabilidade de um processo.",
            1, Modalidade.Hibrido, 60, "Inspetores de qualidade, líderes de produção e técnicos de processo.",
            StatusCurso.EmBreve, "banner-ava.webp", new DateOnly(2026, 11, 16),
            [
                new("Fundamentos de estatística", [
                    new("Amostra e população", "Como escolher uma amostra representativa do lote."),
                    new("Medidas de tendência e dispersão", "Média, mediana, amplitude e desvio-padrão."),
                ]),
                new("Controle estatístico de processo", [
                    new("Cartas de controle", "Limites de controle e sinais de processo instável."),
                    new("Capacidade do processo", "Cp e Cpk explicados com exemplos da indústria."),
                ]),
                new("Prática presencial", [
                    new("Coleta em laboratório", "Medição real de peças e montagem da carta de controle."),
                ]),
            ]),
        new("comunicacao-tecnica-redacao-profissional", "Comunicação Técnica e Redação Profissional", "Redação Profissional",
            "Escrita clara de relatórios, e-mails e procedimentos operacionais, com foco em objetividade e revisão.",
            2, Modalidade.Ead, 30, "Profissionais que redigem relatórios, procedimentos ou comunicados no trabalho.",
            StatusCurso.InscricoesAbertas, "video-thumb.webp", null,
            [
                new("Escrever para ser entendido", [
                    new("Clareza e objetividade", "Frases curtas, voz ativa e uma ideia por parágrafo."),
                    new("Adequação ao leitor", "Ajustar o texto para gestor, colega ou cliente."),
                ]),
                new("Gêneros do trabalho", [
                    new("Relatório técnico", "Estrutura, dados e conclusão que sustenta a decisão."),
                    new("Procedimento operacional", "Passo a passo verificável e sem ambiguidade."),
                ]),
            ]),
        new("ingles-instrumental-industria", "Inglês Instrumental para a Indústria", "Inglês Instrumental",
            "Leitura de manuais, fichas técnicas e telas de equipamentos em inglês, com vocabulário do setor industrial.",
            2, Modalidade.Presencial, 80, "Operadores e técnicos que lidam com manuais e equipamentos importados.",
            StatusCurso.Encerrado, "hero-image.webp", null,
            [
                new("Estratégias de leitura", [
                    new("Skimming e scanning", "Achar a informação certa sem traduzir o texto inteiro."),
                    new("Cognatos e falsos cognatos", "Palavras que ajudam e as que enganam."),
                ]),
                new("Vocabulário técnico", [
                    new("Manuais de manutenção", "Termos de segurança, ferramentas e procedimentos."),
                    new("Interfaces de máquinas", "Mensagens de alarme e menus de IHM."),
                ]),
            ]),
        new("quimica-materiais-industriais", "Química dos Materiais Industriais", "Química dos Materiais",
            "Propriedades de metais, polímeros e cerâmicos, e como a estrutura química define o uso de cada material.",
            3, Modalidade.Presencial, 48, "Estudantes do ensino médio e técnicos interessados em processos de transformação.",
            StatusCurso.InscricoesAbertas, "banner-ava.webp", null,
            [
                new("Estrutura da matéria", [
                    new("Ligações químicas", "Iônica, covalente e metálica, e o que cada uma explica."),
                    new("Propriedades dos materiais", "Dureza, condutividade e resistência à corrosão."),
                ]),
                new("Famílias de materiais", [
                    new("Metais e ligas", "Aço, alumínio e tratamentos térmicos."),
                    new("Polímeros", "Termoplásticos, termofixos e reciclagem."),
                    new("Laboratório", "Ensaios simples de identificação de materiais."),
                ]),
            ]),
        new("fisica-aplicada-maquinas", "Física Aplicada a Máquinas e Equipamentos", "Física Aplicada",
            "Força, torque, energia e potência para entender o funcionamento e a eficiência de máquinas industriais.",
            3, Modalidade.Ead, 36, "Trabalhadores da manutenção e da operação de máquinas.",
            StatusCurso.EmBreve, "video-thumb.webp", new DateOnly(2027, 2, 1),
            [
                new("Mecânica", [
                    new("Força e torque", "Alavancas, engrenagens e transmissão de movimento."),
                    new("Atrito e lubrificação", "Por que a máquina aquece e como reduzir perdas."),
                ]),
                new("Energia", [
                    new("Trabalho e potência", "Calcular a potência necessária para uma tarefa."),
                    new("Eficiência energética", "Onde a energia se perde num motor elétrico."),
                ]),
            ]),
        new("economia-circular-industria", "Economia Circular na Indústria", "Economia Circular",
            "Princípios da economia circular, ciclo de vida do produto e exemplos de redução de desperdício na produção.",
            4, Modalidade.Ead, 20, "Qualquer pessoa interessada em sustentabilidade no ambiente de trabalho.",
            StatusCurso.InscricoesAbertas, "hero-image.webp", null,
            [
                new("Do linear ao circular", [
                    new("O modelo extrair-produzir-descartar", "Limites do modelo linear de produção."),
                    new("Os princípios da circularidade", "Reduzir, reutilizar, remanufaturar e reciclar."),
                ]),
                new("Na prática", [
                    new("Ciclo de vida do produto", "Mapear impactos do projeto ao descarte."),
                ]),
            ]),
        new("gestao-residuos-eficiencia-energetica", "Gestão de Resíduos e Eficiência Energética", "Gestão de Resíduos",
            "Classificação de resíduos, logística reversa e ações de eficiência energética com retorno mensurável.",
            4, Modalidade.Hibrido, 64, "Supervisores, técnicos de meio ambiente e membros de comissões internas.",
            StatusCurso.Encerrado, "banner-ava.webp", null,
            [
                new("Resíduos", [
                    new("Classificação e armazenamento", "Classes de resíduos e cuidados de armazenamento."),
                    new("Logística reversa", "Responsabilidade compartilhada e destinação correta."),
                ]),
                new("Energia", [
                    new("Diagnóstico de consumo", "Ler a conta de energia e achar os maiores consumidores."),
                    new("Plano de ação", "Priorizar medidas pelo retorno e pelo custo."),
                ]),
                new("Projeto final", [
                    new("Visita técnica", "Levantamento em campo e apresentação do plano."),
                ]),
            ]),
    ];

    public static void Aplicar(ModelBuilder mb)
    {
        mb.Entity<Categoria>().HasData(Categorias);

        var cursos = new List<object>();
        var modulos = new List<object>();
        var aulas = new List<object>();
        int idModulo = 0, idAula = 0;

        for (var i = 0; i < Cursos.Length; i++)
        {
            var c = Cursos[i];
            var idCurso = i + 1;
            cursos.Add(new
            {
                Id = idCurso, c.Slug, c.NomeCompleto, c.NomeBreve, c.Descricao, c.CategoriaId, c.Modalidade,
                CargaHorariaHoras = c.CargaHoraria, c.PublicoAlvo, c.Status, ImagemUrl = Img + c.Imagem,
                DataDisponibilizacao = c.Disponibilizacao,
                NomeBusca = TextoBusca.Normalizar($"{c.NomeCompleto} {c.NomeBreve}"),
            });

            for (var m = 0; m < c.Modulos.Length; m++)
            {
                idModulo++;
                modulos.Add(new { Id = idModulo, CursoId = idCurso, Ordem = m + 1, c.Modulos[m].Titulo });
                for (var a = 0; a < c.Modulos[m].Aulas.Length; a++)
                {
                    var aula = c.Modulos[m].Aulas[a];
                    aulas.Add(new { Id = ++idAula, ModuloId = idModulo, Ordem = a + 1, aula.Titulo, aula.Descricao });
                }
            }
        }

        mb.Entity<Curso>().HasData(cursos.ToArray());
        mb.Entity<Modulo>().HasData(modulos.ToArray());
        mb.Entity<Aula>().HasData(aulas.ToArray());
    }
}
