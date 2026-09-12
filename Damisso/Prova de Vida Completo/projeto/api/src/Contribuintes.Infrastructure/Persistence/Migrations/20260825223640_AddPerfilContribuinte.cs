using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contribuintes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfilContribuinte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Perfil",
                table: "Contribuintes",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "TrabalhadorAtivo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "Contribuintes");
        }
    }
}
