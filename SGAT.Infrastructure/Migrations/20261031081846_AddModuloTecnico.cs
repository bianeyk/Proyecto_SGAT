using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloTecnico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoOrden",
                table: "OrdenesServicio",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BitacorasTecnicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenId = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacorasTecnicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacorasTecnicas_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiagnosticosTecnicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenId = table.Column<int>(type: "int", nullable: false),
                    TecnicoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FallaReportada = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DiagnosticoDetallado = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SolucionAplicada = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiereRepuestos = table.Column<bool>(type: "bit", nullable: false),
                    RepuestosSugeridos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CostoManoDeObra = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CostoTotalEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosticosTecnicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosticosTecnicos_AspNetUsers_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiagnosticosTecnicos_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BitacorasTecnicas_OrdenId",
                table: "BitacorasTecnicas",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticosTecnicos_OrdenId",
                table: "DiagnosticosTecnicos",
                column: "OrdenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticosTecnicos_TecnicoId",
                table: "DiagnosticosTecnicos",
                column: "TecnicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BitacorasTecnicas");

            migrationBuilder.DropTable(
                name: "DiagnosticosTecnicos");

            migrationBuilder.DropColumn(
                name: "EstadoOrden",
                table: "OrdenesServicio");
        }
    }
}
