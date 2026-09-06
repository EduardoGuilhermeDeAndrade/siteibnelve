using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibnelve.Api.Migrations
{
    /// <inheritdoc />
    public partial class PedidoOracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidosOracao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Anonimo = table.Column<bool>(type: "boolean", nullable: false),
                    Contato = table.Column<string>(type: "text", nullable: true),
                    DesejaFalarComPastor = table.Column<bool>(type: "boolean", nullable: false),
                    Mensagem = table.Column<string>(type: "text", nullable: false),
                    Lido = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosOracao", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidosOracao_DataCriacao",
                table: "PedidosOracao",
                column: "DataCriacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidosOracao");
        }
    }
}
