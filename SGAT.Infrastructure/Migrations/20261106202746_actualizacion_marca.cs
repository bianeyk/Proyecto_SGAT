using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class actualizacion_marca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UQ_Cliente_NumDoc",
                table: "Clientes",
                newName: "IX_Clientes_NumDoc");

            migrationBuilder.AddColumn<int>(
                name: "IdMarca",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdMarca",
                table: "Productos",
                column: "IdMarca");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Marcas_IdMarca",
                table: "Productos",
                column: "IdMarca",
                principalTable: "Marcas",
                principalColumn: "IdMarca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Marcas_IdMarca",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_IdMarca",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "IdMarca",
                table: "Productos");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_NumDoc",
                table: "Clientes",
                newName: "UQ_Cliente_NumDoc");
        }
    }
}
