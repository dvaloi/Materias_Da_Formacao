using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProvaVida.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosProvaVida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContribuinteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nuit = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    RealizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScoreLiveness = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FaceHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    MovimentosDetectados = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosProvaVida", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosProvaVida_ContribuinteId",
                table: "RegistrosProvaVida",
                column: "ContribuinteId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosProvaVida_Nuit",
                table: "RegistrosProvaVida",
                column: "Nuit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosProvaVida");
        }
    }
}
