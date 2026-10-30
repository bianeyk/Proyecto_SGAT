using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SGAT.Infrastructure.Data;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261029213000_ActualizarCliente")]
    public partial class ActualizarCliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_AspNetUsers_IdUsuario",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_IdUsuario",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Clientes",
                newName: "IdUsuarioCrea");

            migrationBuilder.RenameColumn(
                name: "IsTecnico",
                table: "Clientes",
                newName: "EsColaborador");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdUsuarioCrea",
                table: "Clientes",
                column: "IdUsuarioCrea");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_AspNetUsers_IdUsuarioCrea",
                table: "Clientes",
                column: "IdUsuarioCrea",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_AspNetUsers_IdUsuarioCrea",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_IdUsuarioCrea",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioCrea",
                table: "Clientes",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "EsColaborador",
                table: "Clientes",
                newName: "IsTecnico");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdUsuario",
                table: "Clientes",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_AspNetUsers_IdUsuario",
                table: "Clientes",
                column: "IdUsuario",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
