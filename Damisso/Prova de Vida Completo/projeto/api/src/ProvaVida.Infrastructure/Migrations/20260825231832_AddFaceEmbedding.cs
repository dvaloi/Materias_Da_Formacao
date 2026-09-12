using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProvaVida.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFaceEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaceEmbeddingJson",
                table: "PerfisBiometricos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaceEmbeddingJson",
                table: "PerfisBiometricos");
        }
    }
}
