using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoCursos.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class ImagensWebp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagemUrl",
                value: "/assets/imagens/hero-image.webp");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagemUrl",
                value: "/assets/imagens/banner-ava.webp");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagemUrl",
                value: "/assets/imagens/video-thumb.webp");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagemUrl",
                value: "/assets/imagens/hero-image.webp");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagemUrl",
                value: "/assets/imagens/banner-ava.webp");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagemUrl",
                value: "/assets/imagens/video-thumb.webp");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagemUrl",
                value: "/assets/imagens/hero-image.webp");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagemUrl",
                value: "/assets/imagens/banner-ava.webp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagemUrl",
                value: "/assets/imagens/hero-image.png");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagemUrl",
                value: "/assets/imagens/banner-ava.png");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagemUrl",
                value: "/assets/imagens/video-thumb.png");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagemUrl",
                value: "/assets/imagens/hero-image.png");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagemUrl",
                value: "/assets/imagens/banner-ava.png");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagemUrl",
                value: "/assets/imagens/video-thumb.png");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagemUrl",
                value: "/assets/imagens/hero-image.png");

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagemUrl",
                value: "/assets/imagens/banner-ava.png");
        }
    }
}
