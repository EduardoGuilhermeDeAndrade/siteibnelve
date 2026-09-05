using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibnelve.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracaoContribuicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracoesContribuicao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChavePix = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoChave = table.Column<int>(type: "integer", nullable: false),
                    Favorecido = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AtualizadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesContribuicao", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracoesContribuicao");
        }
    }
}
