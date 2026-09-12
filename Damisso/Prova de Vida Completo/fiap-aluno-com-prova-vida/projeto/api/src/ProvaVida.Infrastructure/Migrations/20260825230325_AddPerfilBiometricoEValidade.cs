using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProvaVida.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfilBiometricoEValidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerfisBiometricos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContribuinteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nuit = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    FaceHashReferencia = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CadastradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisBiometricos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerfisBiometricos_ContribuinteId",
                table: "PerfisBiometricos",
                column: "ContribuinteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisBiometricos_Nuit",
                table: "PerfisBiometricos",
                column: "Nuit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerfisBiometricos");
        }
    }
}
