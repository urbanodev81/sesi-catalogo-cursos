using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoCursos.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class CursosLinguagensEAgua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "cursos",
                columns: new[] { "Id", "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "ImagemUrl", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug", "Status" },
                values: new object[,]
                {
                    { 10, 30, 2, new DateOnly(2026, 9, 14), "Boa parte dos manuais, catálogos e painéis de máquinas chega em inglês. O curso ensina estratégias de leitura e o vocabulário técnico para entender esses textos sem depender de tradução.", "/assets/imagens/hero-image.webp", "Ead", "Inglês Instrumental", "ingles instrumental para manuais tecnicos ingles instrumental", "Inglês Instrumental para Manuais Técnicos", "Operadores e técnicos que consultam documentação de equipamentos importados.", "Leitura de manuais, catálogos e telas de máquinas em inglês, com foco no vocabulário da produção.", "ingles-instrumental-manuais-tecnicos", "InscricoesAbertas" },
                    { 11, 16, 2, new DateOnly(2026, 10, 5), "Normas técnicas definem como medir, fabricar e inspecionar. O curso mostra a estrutura de uma norma, a diferença entre requisito e recomendação e como transformar o texto em procedimento.", "/assets/imagens/banner-ava.webp", "Ead", "Normas Técnicas", "leitura e interpretacao de normas tecnicas normas tecnicas", "Leitura e Interpretação de Normas Técnicas", "Técnicos de qualidade, inspetores e líderes de produção.", "Como uma norma é organizada, o que é requisito e o que é recomendação, e como aplicá-la no posto de trabalho.", "leitura-interpretacao-normas-tecnicas", "InscricoesAbertas" },
                    { 12, 24, 4, new DateOnly(2026, 12, 7), "Água é insumo e efluente é custo. O curso cobre o balanço hídrico da fábrica, as oportunidades de reúso e as etapas de tratamento exigidas antes do descarte.", "/assets/imagens/video-thumb.webp", "Presencial", "Água e Efluentes", "agua e efluentes na industria agua e efluentes", "Água e Efluentes na Indústria", "Técnicos de meio ambiente, operadores de utilidades e supervisores de produção.", "Uso racional da água, reúso e tratamento de efluentes para reduzir custo e atender à legislação.", "agua-efluentes-industria", "InscricoesAbertas" }
                });

            migrationBuilder.InsertData(
                table: "modulos",
                columns: new[] { "Id", "CursoId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 24, 10, 1, "Estratégias de leitura" },
                    { 25, 10, 2, "O manual da máquina" },
                    { 26, 11, 1, "A estrutura da norma" },
                    { 27, 11, 2, "Da norma ao procedimento" },
                    { 28, 12, 1, "Uso da água" },
                    { 29, 12, 2, "Tratamento de efluentes" }
                });

            migrationBuilder.InsertData(
                table: "aulas",
                columns: new[] { "Id", "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 44, "Skimming é passar os olhos para entender do que o texto trata; scanning é procurar uma informação específica, como um valor de torque.\n\nNo manual, os dois começam pelo sumário, pelos títulos e pelas tabelas.", "Achar a ideia geral e a informação exata sem ler tudo.", 24, 1, "Skimming e scanning" },
                    { 45, "Cognatos como \"temperature\" e \"pressure\" aceleram a leitura.\n\n## Atenção\n\nFalsos cognatos enganam: \"actually\" é \"na verdade\", e \"pretend\" é \"fingir\", não \"pretender\".", "Palavras parecidas que ajudam e as que enganam.", 24, 2, "Cognatos e falsos cognatos" },
                    { 46, "Os avisos seguem uma escala: \"notice\" informa, \"caution\" alerta para risco leve, \"warning\" para risco grave e \"danger\" para risco iminente.\n\nReconhecer a palavra-chave basta para saber o quanto parar e reler.", "Warning, caution e danger: o que cada nível exige.", 25, 1, "Avisos de segurança" },
                    { 47, "Instruções usam o imperativo: \"remove\", \"tighten\", \"check\". A ordem dos passos é a ordem da execução.\n\nPalavras como \"before\", \"after\" e \"until\" mudam essa ordem e merecem atenção.", "Verbos no imperativo e a ordem das etapas.", 25, 2, "Instruções passo a passo" },
                    { 48, "O escopo diz a que produtos e processos a norma se aplica; ler essa seção primeiro evita aplicar a norma errada.\n\nAs definições fixam o sentido de cada termo dentro daquele documento.", "As primeiras seções dizem onde a norma vale.", 26, 1, "Escopo, referências e definições" },
                    { 49, "Na linguagem normativa, \"deve\" indica requisito: sem ele, não há conformidade. \"Convém\" indica recomendação, e \"pode\" indica permissão.\n\nConfundir os verbos gera auditoria reprovada ou trabalho desnecessário.", "\"Deve\" obriga; \"convém\" orienta.", 26, 2, "Requisito e recomendação" },
                    { 50, "Cada requisito aplicável vira um passo verificável do procedimento, com responsável e registro.\n\n## Exercício\n\nEscolha um requisito de inspeção e escreva o passo correspondente em uma frase.", "Escrever o procedimento que cumpre a norma.", 27, 1, "Transformar requisito em passo" },
                    { 51, "O balanço hídrico compara a água captada com a consumida em cada processo e a descartada.\n\nA diferença aponta vazamentos e usos que ninguém mede.", "Quanto entra, quanto sai e onde se perde.", 28, 1, "Balanço hídrico" },
                    { 52, "Água de enxágue final pode lavar pisos; condensado pode voltar à caldeira.\n\nO critério é a qualidade exigida pelo uso seguinte, não a origem da água.", "Onde a água de um processo serve a outro.", 28, 2, "Reúso" },
                    { 53, "O tratamento preliminar retira sólidos grosseiros; o primário, material sedimentável; o secundário, matéria orgânica por ação biológica.\n\nO terciário remove o que sobra quando a legislação ou o reúso exigem.", "Preliminar, primário, secundário e terciário.", 29, 1, "Etapas do tratamento" },
                    { 54, "A turma acompanha a coleta de amostras e a leitura dos indicadores da estação.\n\nO relatório final compara os resultados com os limites de lançamento.", "Acompanhamento da operação de uma estação de tratamento.", 29, 2, "Visita à estação" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
