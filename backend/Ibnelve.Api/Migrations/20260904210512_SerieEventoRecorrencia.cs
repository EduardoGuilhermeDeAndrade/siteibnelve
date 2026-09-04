using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibnelve.Api.Migrations
{
    /// <inheritdoc />
    public partial class SerieEventoRecorrencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeriesEvento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Visibilidade = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TipoRecorrencia = table.Column<int>(type: "integer", nullable: false),
                    Intervalo = table.Column<int>(type: "integer", nullable: true),
                    DiaSemana = table.Column<int>(type: "integer", nullable: true),
                    DiaMes = table.Column<int>(type: "integer", nullable: true),
                    PosicaoNoMes = table.Column<int>(type: "integer", nullable: true),
                    HoraInicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HoraFim = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    DataInicioRecorrencia = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFimRecorrencia = table.Column<DateOnly>(type: "date", nullable: true),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocalTexto = table.Column<string>(type: "text", nullable: true),
                    ImagemUrl = table.Column<string>(type: "text", nullable: true),
                    Destaque = table.Column<bool>(type: "boolean", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriesEvento_Locais_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ExcecoesEvento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SerieEventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataOriginal = table.Column<DateOnly>(type: "date", nullable: false),
                    Cancelado = table.Column<bool>(type: "boolean", nullable: false),
                    NovaData = table.Column<DateOnly>(type: "date", nullable: true),
                    NovaHoraInicio = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NovaHoraFim = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    TituloSubstituto = table.Column<string>(type: "text", nullable: true),
                    DescricaoSubstituta = table.Column<string>(type: "text", nullable: true),
                    LocalSubstitutoId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocalTextoSubstituto = table.Column<string>(type: "text", nullable: true),
                    Motivo = table.Column<string>(type: "text", nullable: true),
                    CriadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcecoesEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExcecoesEvento_Locais_LocalSubstitutoId",
                        column: x => x.LocalSubstitutoId,
                        principalTable: "Locais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ExcecoesEvento_SeriesEvento_SerieEventoId",
                        column: x => x.SerieEventoId,
                        principalTable: "SeriesEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExcecoesEvento_LocalSubstitutoId",
                table: "ExcecoesEvento",
                column: "LocalSubstitutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExcecoesEvento_SerieEventoId_DataOriginal",
                table: "ExcecoesEvento",
                columns: new[] { "SerieEventoId", "DataOriginal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeriesEvento_LocalId",
                table: "SeriesEvento",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesEvento_Visibilidade_Status_Ativo",
                table: "SeriesEvento",
                columns: new[] { "Visibilidade", "Status", "Ativo" });

            // Migra os eventos já cadastrados na entidade Evento (v1 simplificado da Fase 5) para o
            // modelo completo SerieEvento antes de descartar a tabela antiga — nenhum dado se perde.
            // RecorrenciaSemanal=true -> TipoRecorrencia.Semanal (1); false -> TipoRecorrencia.Nenhuma (0).
            migrationBuilder.Sql(
                """
                INSERT INTO "SeriesEvento" (
                    "Id", "Titulo", "Descricao", "Categoria", "Visibilidade", "Status",
                    "TipoRecorrencia", "Intervalo", "DiaSemana", "DiaMes", "PosicaoNoMes",
                    "HoraInicio", "HoraFim", "DataInicioRecorrencia", "DataFimRecorrencia",
                    "LocalId", "LocalTexto", "ImagemUrl", "Destaque", "Ativo",
                    "CriadoPorUsuarioId", "DataCriacao", "AtualizadoPorUsuarioId", "DataAtualizacao"
                )
                SELECT
                    "Id", "Titulo", "Descricao", "Categoria", "Visibilidade", "Status",
                    CASE WHEN "RecorrenciaSemanal" THEN 1 ELSE 0 END, NULL,
                    CASE WHEN "RecorrenciaSemanal"
                        THEN EXTRACT(DOW FROM "DataHoraInicio" AT TIME ZONE 'America/Sao_Paulo')::int
                        ELSE "DiaSemana"
                    END,
                    NULL, NULL,
                    ("DataHoraInicio" AT TIME ZONE 'America/Sao_Paulo')::time, NULL,
                    ("DataHoraInicio" AT TIME ZONE 'America/Sao_Paulo')::date, NULL,
                    "LocalId", NULLIF("LocalTexto", ''), "ImagemUrl", false, true,
                    "CriadoPorUsuarioId", "DataCriacao", "AtualizadoPorUsuarioId", "DataAtualizacao"
                FROM "Eventos";
                """);

            migrationBuilder.DropTable(
                name: "Eventos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExcecoesEvento");

            migrationBuilder.DropTable(
                name: "SeriesEvento");

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalId = table.Column<Guid>(type: "uuid", nullable: true),
                    AtualizadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    CriadoPorUsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DataHoraFim = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DataHoraInicio = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    DiaSemana = table.Column<int>(type: "integer", nullable: true),
                    ImagemUrl = table.Column<string>(type: "text", nullable: true),
                    LocalTexto = table.Column<string>(type: "text", nullable: true),
                    RecorrenciaSemanal = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Visibilidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_Locais_LocalId",
                        column: x => x.LocalId,
                        principalTable: "Locais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_LocalId",
                table: "Eventos",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_Visibilidade_Status_DataHoraInicio",
                table: "Eventos",
                columns: new[] { "Visibilidade", "Status", "DataHoraInicio" });
        }
    }
}
