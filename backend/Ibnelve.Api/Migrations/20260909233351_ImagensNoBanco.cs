using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibnelve.Api.Migrations
{
    /// <inheritdoc />
    public partial class ImagensNoBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ImagensSite");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ImagensSite",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Conteudo",
                table: "ImagensSite",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ImagensSite");

            migrationBuilder.DropColumn(
                name: "Conteudo",
                table: "ImagensSite");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ImagensSite",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
