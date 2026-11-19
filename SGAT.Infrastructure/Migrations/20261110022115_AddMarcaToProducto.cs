using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMarcaToProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Se agrega como nullable para poder rellenar los productos existentes
            // antes de volverla obligatoria.
            migrationBuilder.AddColumn<int>(
                name: "IdMarca",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM [Productos] WHERE [IdMarca] IS NULL)
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [Marcas] WHERE [Nombre] = N'Sin marca')
                    BEGIN
                        INSERT INTO [Marcas] ([Nombre], [Descripcion], [Activo])
                        VALUES (N'Sin marca', N'Marca asignada por defecto al agregar el campo Marca a Productos.', 1);
                    END;

                    DECLARE @IdMarcaPorDefecto int = (SELECT TOP 1 [IdMarca] FROM [Marcas] WHERE [Nombre] = N'Sin marca');
                    UPDATE [Productos] SET [IdMarca] = @IdMarcaPorDefecto WHERE [IdMarca] IS NULL;
                END;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "IdMarca",
                table: "Productos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
        }
    }
}
