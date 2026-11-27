using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipoSerialAndConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Equipos",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "SerialGenerado",
                table: "Equipos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Equipos",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_Modelo",
                table: "Equipos",
                column: "Modelo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Equipos_Modelo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "SerialGenerado",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Equipos");
        }
    }
}
