using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibnelve.Api.Migrations
{
    /// <inheritdoc />
    public partial class PainelDeFotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FotosGaleria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Conteudo = table.Column<byte[]>(type: "bytea", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Legenda = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    AtualizadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosGaleria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FotosGaleria_Ordem",
                table: "FotosGaleria",
                column: "Ordem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FotosGaleria");
        }
    }
}
