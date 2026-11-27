using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeUserSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_NombreCompleto",
                table: "AspNetUsers",
                column: "NombreCompleto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_NombreCompleto",
                table: "AspNetUsers");
        }
    }
}
