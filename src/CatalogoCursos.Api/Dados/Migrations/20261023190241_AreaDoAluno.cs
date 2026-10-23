using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoCursos.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AreaDoAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UltimoAcesso",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CursoId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avaliacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_avaliacoes_cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documentos_exigidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Orientacao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_exigidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "documentos_enviados",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    DocumentoId = table.Column<int>(type: "integer", nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoConteudo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Tamanho = table.Column<int>(type: "integer", nullable: false),
                    Conteudo = table.Column<byte[]>(type: "bytea", nullable: false),
                    Em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_enviados", x => new { x.UsuarioId, x.DocumentoId });
                    table.ForeignKey(
                        name: "FK_documentos_enviados_documentos_exigidos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "documentos_exigidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_documentos_enviados_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "avaliacoes",
                columns: new[] { "Id", "CursoId", "Em", "Tipo", "Titulo" },
                values: new object[,]
                {
                    { 1, 10, new DateTimeOffset(new DateTime(2026, 10, 15, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "Questionario", "Questionário do módulo 1: vocabulário técnico" },
                    { 2, 10, new DateTimeOffset(new DateTime(2026, 11, 6, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "Trabalho", "Tradução de um trecho de manual de máquina" },
                    { 3, 10, new DateTimeOffset(new DateTime(2026, 12, 4, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "Prova", "Prova final de inglês instrumental" },
                    { 4, 11, new DateTimeOffset(new DateTime(2026, 10, 30, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "Questionario", "Questionário: estrutura de uma norma" },
                    { 5, 11, new DateTimeOffset(new DateTime(2026, 11, 20, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "Trabalho", "Análise de uma norma do seu setor" },
                    { 6, 11, new DateTimeOffset(new DateTime(2026, 12, 11, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "Prova", "Prova final de normas técnicas" }
                });

            migrationBuilder.InsertData(
                table: "documentos_exigidos",
                columns: new[] { "Id", "Ordem", "Orientacao", "Titulo" },
                values: new object[,]
                {
                    { 1, 1, "RG, CNH ou outro documento oficial com foto, frente e verso.", "Documento de identidade" },
                    { 2, 2, "Pode ser o próprio documento de identidade, se trouxer o número do CPF.", "CPF" },
                    { 3, 3, "Certificado ou histórico da última etapa concluída.", "Comprovante de escolaridade" },
                    { 4, 4, "Emitida pela empresa onde trabalha, com data dos últimos 90 dias.", "Declaração de vínculo com a indústria" }
                });

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "UltimoAcesso",
                value: null);

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "UltimoAcesso",
                value: null);

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "UltimoAcesso",
                value: null);

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "UltimoAcesso",
                value: null);

            migrationBuilder.InsertData(
                table: "documentos_enviados",
                columns: new[] { "DocumentoId", "UsuarioId", "Conteudo", "Em", "NomeArquivo", "Tamanho", "TipoConteudo" },
                values: new object[] { 1, 1, new byte[] { 37, 80, 68, 70, 45, 49, 46, 52, 10, 49, 32, 48, 32, 111, 98, 106, 60, 60, 47, 84, 121, 112, 101, 47, 67, 97, 116, 97, 108, 111, 103, 47, 80, 97, 103, 101, 115, 32, 50, 32, 48, 32, 82, 62, 62, 101, 110, 100, 111, 98, 106, 10, 50, 32, 48, 32, 111, 98, 106, 60, 60, 47, 84, 121, 112, 101, 47, 80, 97, 103, 101, 115, 47, 75, 105, 100, 115, 91, 51, 32, 48, 32, 82, 93, 47, 67, 111, 117, 110, 116, 32, 49, 62, 62, 101, 110, 100, 111, 98, 106, 10, 51, 32, 48, 32, 111, 98, 106, 60, 60, 47, 84, 121, 112, 101, 47, 80, 97, 103, 101, 47, 80, 97, 114, 101, 110, 116, 32, 50, 32, 48, 32, 82, 47, 77, 101, 100, 105, 97, 66, 111, 120, 91, 48, 32, 48, 32, 53, 57, 53, 32, 56, 52, 50, 93, 62, 62, 101, 110, 100, 111, 98, 106, 10, 116, 114, 97, 105, 108, 101, 114, 60, 60, 47, 82, 111, 111, 116, 32, 49, 32, 48, 32, 82, 62, 62, 10, 37, 37, 69, 79, 70, 10 }, new DateTimeOffset(new DateTime(2026, 10, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "identidade-ana.pdf", 193, "application/pdf" });

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_CursoId_Em",
                table: "avaliacoes",
                columns: new[] { "CursoId", "Em" });

            migrationBuilder.CreateIndex(
                name: "IX_documentos_enviados_DocumentoId",
                table: "documentos_enviados",
                column: "DocumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "avaliacoes");

            migrationBuilder.DropTable(
                name: "documentos_enviados");

            migrationBuilder.DropTable(
                name: "documentos_exigidos");

            migrationBuilder.DropColumn(
                name: "UltimoAcesso",
                table: "usuarios");
        }
    }
}
