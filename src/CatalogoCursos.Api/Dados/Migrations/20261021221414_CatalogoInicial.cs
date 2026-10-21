using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoCursos.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class CatalogoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Icone = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    NomeCompleto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NomeBreve = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    Modalidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CargaHorariaHoras = table.Column<int>(type: "integer", nullable: false),
                    PublicoAlvo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ImagemUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DataDisponibilizacao = table.Column<DateOnly>(type: "date", nullable: true),
                    NomeBusca = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cursos_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CursoId = table.Column<int>(type: "integer", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_modulos_cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aulas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuloId = table.Column<int>(type: "integer", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aulas_modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categorias",
                columns: new[] { "Id", "Icone", "Nome", "Slug" },
                values: new object[,]
                {
                    { 1, "icon-matematica-20px.svg", "Matemática", "matematica" },
                    { 2, "icon-linguagens-20px.svg", "Linguagens", "linguagens" },
                    { 3, "icon-ciencias-natureza-20px.svg", "Ciências da Natureza", "ciencias-da-natureza" },
                    { 4, "icon-sustentabilidade-20px.svg", "Sustentabilidade", "sustentabilidade" }
                });

            migrationBuilder.InsertData(
                table: "cursos",
                columns: new[] { "Id", "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "ImagemUrl", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug", "Status" },
                values: new object[,]
                {
                    { 1, 40, 1, null, "Razão, proporção, funções e leitura de gráficos aplicadas a indicadores de produção, sensores e automação.", "/assets/imagens/hero-image.png", "Ead", "Matemática 4.0", "matematica aplicada a industria 4.0 matematica 4.0", "Matemática Aplicada à Indústria 4.0", "Trabalhadores da indústria com ensino médio em andamento ou concluído.", "matematica-aplicada-industria-4-0", "InscricoesAbertas" },
                    { 2, 60, 1, new DateOnly(2026, 11, 16), "Amostragem, média, desvio-padrão e cartas de controle para acompanhar a estabilidade de um processo.", "/assets/imagens/banner-ava.png", "Hibrido", "Estatística na Qualidade", "estatistica para controle de qualidade estatistica na qualidade", "Estatística para Controle de Qualidade", "Inspetores de qualidade, líderes de produção e técnicos de processo.", "estatistica-controle-qualidade", "EmBreve" },
                    { 3, 30, 2, null, "Escrita clara de relatórios, e-mails e procedimentos operacionais, com foco em objetividade e revisão.", "/assets/imagens/video-thumb.png", "Ead", "Redação Profissional", "comunicacao tecnica e redacao profissional redacao profissional", "Comunicação Técnica e Redação Profissional", "Profissionais que redigem relatórios, procedimentos ou comunicados no trabalho.", "comunicacao-tecnica-redacao-profissional", "InscricoesAbertas" },
                    { 4, 80, 2, null, "Leitura de manuais, fichas técnicas e telas de equipamentos em inglês, com vocabulário do setor industrial.", "/assets/imagens/hero-image.png", "Presencial", "Inglês Instrumental", "ingles instrumental para a industria ingles instrumental", "Inglês Instrumental para a Indústria", "Operadores e técnicos que lidam com manuais e equipamentos importados.", "ingles-instrumental-industria", "Encerrado" },
                    { 5, 48, 3, null, "Propriedades de metais, polímeros e cerâmicos, e como a estrutura química define o uso de cada material.", "/assets/imagens/banner-ava.png", "Presencial", "Química dos Materiais", "quimica dos materiais industriais quimica dos materiais", "Química dos Materiais Industriais", "Estudantes do ensino médio e técnicos interessados em processos de transformação.", "quimica-materiais-industriais", "InscricoesAbertas" },
                    { 6, 36, 3, new DateOnly(2027, 2, 1), "Força, torque, energia e potência para entender o funcionamento e a eficiência de máquinas industriais.", "/assets/imagens/video-thumb.png", "Ead", "Física Aplicada", "fisica aplicada a maquinas e equipamentos fisica aplicada", "Física Aplicada a Máquinas e Equipamentos", "Trabalhadores da manutenção e da operação de máquinas.", "fisica-aplicada-maquinas", "EmBreve" },
                    { 7, 20, 4, null, "Princípios da economia circular, ciclo de vida do produto e exemplos de redução de desperdício na produção.", "/assets/imagens/hero-image.png", "Ead", "Economia Circular", "economia circular na industria economia circular", "Economia Circular na Indústria", "Qualquer pessoa interessada em sustentabilidade no ambiente de trabalho.", "economia-circular-industria", "InscricoesAbertas" },
                    { 8, 64, 4, null, "Classificação de resíduos, logística reversa e ações de eficiência energética com retorno mensurável.", "/assets/imagens/banner-ava.png", "Hibrido", "Gestão de Resíduos", "gestao de residuos e eficiencia energetica gestao de residuos", "Gestão de Resíduos e Eficiência Energética", "Supervisores, técnicos de meio ambiente e membros de comissões internas.", "gestao-residuos-eficiencia-energetica", "Encerrado" }
                });

            migrationBuilder.InsertData(
                table: "modulos",
                columns: new[] { "Id", "CursoId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 1, 1, 1, "Números no chão de fábrica" },
                    { 2, 1, 2, "Funções e sensores" },
                    { 3, 2, 1, "Fundamentos de estatística" },
                    { 4, 2, 2, "Controle estatístico de processo" },
                    { 5, 2, 3, "Prática presencial" },
                    { 6, 3, 1, "Escrever para ser entendido" },
                    { 7, 3, 2, "Gêneros do trabalho" },
                    { 8, 4, 1, "Estratégias de leitura" },
                    { 9, 4, 2, "Vocabulário técnico" },
                    { 10, 5, 1, "Estrutura da matéria" },
                    { 11, 5, 2, "Famílias de materiais" },
                    { 12, 6, 1, "Mecânica" },
                    { 13, 6, 2, "Energia" },
                    { 14, 7, 1, "Do linear ao circular" },
                    { 15, 7, 2, "Na prática" },
                    { 16, 8, 1, "Resíduos" },
                    { 17, 8, 2, "Energia" },
                    { 18, 8, 3, "Projeto final" }
                });

            migrationBuilder.InsertData(
                table: "aulas",
                columns: new[] { "Id", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 1, "Como calcular rendimento, perdas e consumo por peça.", 1, 1, "Razão e proporção na produção" },
                    { 2, "Leitura de OEE, refugo e meta de produção.", 1, 2, "Porcentagem e indicadores" },
                    { 3, "Relação linear entre leitura do sensor e grandeza medida.", 2, 1, "Função afim na calibração" },
                    { 4, "Interpretar séries de dados vindas de máquinas conectadas.", 2, 2, "Gráficos de tendência" },
                    { 5, "Montar um painel simples de acompanhamento de linha.", 2, 3, "Estudo de caso" },
                    { 6, "Como escolher uma amostra representativa do lote.", 3, 1, "Amostra e população" },
                    { 7, "Média, mediana, amplitude e desvio-padrão.", 3, 2, "Medidas de tendência e dispersão" },
                    { 8, "Limites de controle e sinais de processo instável.", 4, 1, "Cartas de controle" },
                    { 9, "Cp e Cpk explicados com exemplos da indústria.", 4, 2, "Capacidade do processo" },
                    { 10, "Medição real de peças e montagem da carta de controle.", 5, 1, "Coleta em laboratório" },
                    { 11, "Frases curtas, voz ativa e uma ideia por parágrafo.", 6, 1, "Clareza e objetividade" },
                    { 12, "Ajustar o texto para gestor, colega ou cliente.", 6, 2, "Adequação ao leitor" },
                    { 13, "Estrutura, dados e conclusão que sustenta a decisão.", 7, 1, "Relatório técnico" },
                    { 14, "Passo a passo verificável e sem ambiguidade.", 7, 2, "Procedimento operacional" },
                    { 15, "Achar a informação certa sem traduzir o texto inteiro.", 8, 1, "Skimming e scanning" },
                    { 16, "Palavras que ajudam e as que enganam.", 8, 2, "Cognatos e falsos cognatos" },
                    { 17, "Termos de segurança, ferramentas e procedimentos.", 9, 1, "Manuais de manutenção" },
                    { 18, "Mensagens de alarme e menus de IHM.", 9, 2, "Interfaces de máquinas" },
                    { 19, "Iônica, covalente e metálica, e o que cada uma explica.", 10, 1, "Ligações químicas" },
                    { 20, "Dureza, condutividade e resistência à corrosão.", 10, 2, "Propriedades dos materiais" },
                    { 21, "Aço, alumínio e tratamentos térmicos.", 11, 1, "Metais e ligas" },
                    { 22, "Termoplásticos, termofixos e reciclagem.", 11, 2, "Polímeros" },
                    { 23, "Ensaios simples de identificação de materiais.", 11, 3, "Laboratório" },
                    { 24, "Alavancas, engrenagens e transmissão de movimento.", 12, 1, "Força e torque" },
                    { 25, "Por que a máquina aquece e como reduzir perdas.", 12, 2, "Atrito e lubrificação" },
                    { 26, "Calcular a potência necessária para uma tarefa.", 13, 1, "Trabalho e potência" },
                    { 27, "Onde a energia se perde num motor elétrico.", 13, 2, "Eficiência energética" },
                    { 28, "Limites do modelo linear de produção.", 14, 1, "O modelo extrair-produzir-descartar" },
                    { 29, "Reduzir, reutilizar, remanufaturar e reciclar.", 14, 2, "Os princípios da circularidade" },
                    { 30, "Mapear impactos do projeto ao descarte.", 15, 1, "Ciclo de vida do produto" },
                    { 31, "Classes de resíduos e cuidados de armazenamento.", 16, 1, "Classificação e armazenamento" },
                    { 32, "Responsabilidade compartilhada e destinação correta.", 16, 2, "Logística reversa" },
                    { 33, "Ler a conta de energia e achar os maiores consumidores.", 17, 1, "Diagnóstico de consumo" },
                    { 34, "Priorizar medidas pelo retorno e pelo custo.", 17, 2, "Plano de ação" },
                    { 35, "Levantamento em campo e apresentação do plano.", 18, 1, "Visita técnica" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_aulas_ModuloId_Ordem",
                table: "aulas",
                columns: new[] { "ModuloId", "Ordem" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categorias_Slug",
                table: "categorias",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cursos_CategoriaId",
                table: "cursos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_cursos_Slug",
                table: "cursos",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modulos_CursoId_Ordem",
                table: "modulos",
                columns: new[] { "CursoId", "Ordem" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aulas");

            migrationBuilder.DropTable(
                name: "modulos");

            migrationBuilder.DropTable(
                name: "cursos");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
