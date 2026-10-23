using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoCursos.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AreaDoProfessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "horarios_turma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CursoId = table.Column<int>(type: "integer", nullable: false),
                    Dia = table.Column<int>(type: "integer", nullable: false),
                    Inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Fim = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horarios_turma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_horarios_turma_cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "materiais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CursoId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoConteudo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Tamanho = table.Column<int>(type: "integer", nullable: false),
                    Conteudo = table.Column<byte[]>(type: "bytea", nullable: false),
                    Em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materiais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_materiais_cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "horarios_turma",
                columns: new[] { "Id", "CursoId", "Descricao", "Dia", "Fim", "Inicio" },
                values: new object[,]
                {
                    { 1, 10, "Aula ao vivo, no ambiente virtual", 2, new TimeOnly(21, 0, 0), new TimeOnly(19, 0, 0) },
                    { 2, 10, "Aula ao vivo, no ambiente virtual", 4, new TimeOnly(21, 0, 0), new TimeOnly(19, 0, 0) },
                    { 3, 11, "Aula ao vivo, no ambiente virtual", 1, new TimeOnly(21, 0, 0), new TimeOnly(19, 0, 0) },
                    { 4, 11, "Plantão de dúvidas, por videochamada", 6, new TimeOnly(11, 0, 0), new TimeOnly(9, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "materiais",
                columns: new[] { "Id", "Conteudo", "CursoId", "Em", "NomeArquivo", "Tamanho", "TipoConteudo", "Titulo" },
                values: new object[] { 1, new byte[] { 37, 80, 68, 70, 45, 49, 46, 52, 10, 49, 32, 48, 32, 111, 98, 106, 60, 60, 47, 84, 121, 112, 101, 47, 67, 97, 116, 97, 108, 111, 103, 47, 80, 97, 103, 101, 115, 32, 50, 32, 48, 32, 82, 62, 62, 101, 110, 100, 111, 98, 106, 10, 50, 32, 48, 32, 111, 98, 106, 60, 60, 47, 84, 121, 112, 101, 47, 80, 97, 103, 101, 115, 47, 75, 105, 100, 115, 91, 51, 32, 48, 32, 82, 93, 47, 67, 111, 117, 110, 116, 32, 49, 62, 62, 101, 110, 100, 111, 98, 106, 10, 51, 32, 48, 32, 111, 98, 106, 60, 60, 47, 84, 121, 112, 101, 47, 80, 97, 103, 101, 47, 80, 97, 114, 101, 110, 116, 32, 50, 32, 48, 32, 82, 47, 77, 101, 100, 105, 97, 66, 111, 120, 91, 48, 32, 48, 32, 53, 57, 53, 32, 56, 52, 50, 93, 62, 62, 101, 110, 100, 111, 98, 106, 10, 116, 114, 97, 105, 108, 101, 114, 60, 60, 47, 82, 111, 111, 116, 32, 49, 32, 48, 32, 82, 62, 62, 10, 37, 37, 69, 79, 70, 10 }, 11, new DateTimeOffset(new DateTime(2026, 10, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)), "guia-modulo-1.pdf", 193, "application/pdf", "Guia de estudo do módulo 1" });

            migrationBuilder.CreateIndex(
                name: "IX_horarios_turma_CursoId",
                table: "horarios_turma",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_materiais_CursoId_Em",
                table: "materiais",
                columns: new[] { "CursoId", "Em" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "horarios_turma");

            migrationBuilder.DropTable(
                name: "materiais");
        }
    }
}
