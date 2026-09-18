using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibnelve.Api.Migrations
{
    /// <inheritdoc />
    public partial class Patrimonio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItensPatrimonio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    NumeroPatrimonio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TipoControle = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeTotal = table.Column<int>(type: "integer", nullable: false),
                    FotoConteudo = table.Column<byte[]>(type: "bytea", nullable: true),
                    FotoContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: true),
                    Situacao = table.Column<int>(type: "integer", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FotoDefeitoConteudo = table.Column<byte[]>(type: "bytea", nullable: true),
                    FotoDefeitoContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ObservacaoBaixa = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DataBaixa = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CriadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPatrimonio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensPatrimonio_Locais_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmprestimosPatrimonio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemPatrimonioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    QuemRetirou = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ObservacaoRetirada = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DataHoraRetirada = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    QuemDevolveu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ObservacaoDevolucao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DataHoraDevolucao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CriadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmprestimosPatrimonio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmprestimosPatrimonio_ItensPatrimonio_ItemPatrimonioId",
                        column: x => x.ItemPatrimonioId,
                        principalTable: "ItensPatrimonio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmprestimosPatrimonio_ItemPatrimonioId",
                table: "EmprestimosPatrimonio",
                column: "ItemPatrimonioId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPatrimonio_LocalId",
                table: "ItensPatrimonio",
                column: "LocalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmprestimosPatrimonio");

            migrationBuilder.DropTable(
                name: "ItensPatrimonio");
        }
    }
}
