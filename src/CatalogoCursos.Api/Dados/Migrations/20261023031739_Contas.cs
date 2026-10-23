using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoCursos.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class Contas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Papel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "aulas_concluidas",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    AulaId = table.Column<int>(type: "integer", nullable: false),
                    Em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aulas_concluidas", x => new { x.UsuarioId, x.AulaId });
                    table.ForeignKey(
                        name: "FK_aulas_concluidas_aulas_AulaId",
                        column: x => x.AulaId,
                        principalTable: "aulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_aulas_concluidas_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matriculas",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    CursoId = table.Column<int>(type: "integer", nullable: false),
                    Em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matriculas", x => new { x.UsuarioId, x.CursoId });
                    table.ForeignKey(
                        name: "FK_matriculas_cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_matriculas_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "Id", "Email", "Nome", "Papel", "SenhaHash" },
                values: new object[,]
                {
                    { 1, "aluno@exemplo.com", "Ana Souza", "Aluno", "AQAAAAIAAYagAAAAEOb51gKldn8woOxHCSHZRI1DnQRXeDjueJZt8bZ3R8lAz0As3qZOkHSshi+Fpamvzg==" },
                    { 2, "bruno@exemplo.com", "Bruno Lima", "Aluno", "AQAAAAIAAYagAAAAEPu3q9CXXw7l+/F2R5+biUqAuC+pDEl71uD0giWmCL5kEXSwdDoq/cwlHAfF2CPR5w==" },
                    { 3, "carla@exemplo.com", "Carla Mendes", "Aluno", "AQAAAAIAAYagAAAAEE5huoRfewrTExs2BLiy+VlYscDO4M4hssQ8gYQiAecqk9HoeTU2tkjRh55I2b1FyQ==" },
                    { 4, "professor@exemplo.com", "Marina Rocha", "Professor", "AQAAAAIAAYagAAAAEDLXTpaZWRhgbZXeY0sygV6DDAbITcS830+mCHniMcEJEm9cVKkFyHHjjgsvT0KC6w==" }
                });

            migrationBuilder.InsertData(
                table: "aulas_concluidas",
                columns: new[] { "AulaId", "UsuarioId", "Em" },
                values: new object[,]
                {
                    { 44, 1, new DateTimeOffset(new DateTime(2026, 10, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 45, 1, new DateTimeOffset(new DateTime(2026, 10, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 48, 1, new DateTimeOffset(new DateTime(2026, 10, 9, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 44, 2, new DateTimeOffset(new DateTime(2026, 10, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 45, 2, new DateTimeOffset(new DateTime(2026, 10, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 46, 2, new DateTimeOffset(new DateTime(2026, 10, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 47, 2, new DateTimeOffset(new DateTime(2026, 10, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 48, 2, new DateTimeOffset(new DateTime(2026, 10, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 49, 2, new DateTimeOffset(new DateTime(2026, 10, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 44, 3, new DateTimeOffset(new DateTime(2026, 10, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "matriculas",
                columns: new[] { "CursoId", "UsuarioId", "Em" },
                values: new object[,]
                {
                    { 10, 1, new DateTimeOffset(new DateTime(2026, 10, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 11, 1, new DateTimeOffset(new DateTime(2026, 10, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 10, 2, new DateTimeOffset(new DateTime(2026, 10, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 11, 2, new DateTimeOffset(new DateTime(2026, 10, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 10, 3, new DateTimeOffset(new DateTime(2026, 10, 12, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) },
                    { 11, 3, new DateTimeOffset(new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -3, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_aulas_concluidas_AulaId",
                table: "aulas_concluidas",
                column: "AulaId");

            migrationBuilder.CreateIndex(
                name: "IX_matriculas_CursoId",
                table: "matriculas",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aulas_concluidas");

            migrationBuilder.DropTable(
                name: "matriculas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
