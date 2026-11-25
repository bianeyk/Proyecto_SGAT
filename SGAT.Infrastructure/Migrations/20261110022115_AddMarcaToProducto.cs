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
            migrationBuilder.Sql(@"
                IF COL_LENGTH(N'dbo.Productos', N'IdMarca') IS NULL
                    ALTER TABLE [dbo].[Productos] ADD [IdMarca] int NULL;

                IF EXISTS (
                    SELECT 1
                    FROM [dbo].[Productos] p
                    WHERE p.[IdMarca] IS NULL
                       OR NOT EXISTS (SELECT 1 FROM [dbo].[Marcas] m WHERE m.[IdMarca] = p.[IdMarca]))
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[Marcas] WHERE [Nombre] = N'Sin marca')
                    BEGIN
                        INSERT INTO [dbo].[Marcas] ([Nombre], [Descripcion], [Activo])
                        VALUES (N'Sin marca', N'Marca asignada por defecto al agregar el campo Marca a Productos.', 1);
                    END;

                    DECLARE @IdMarcaPorDefecto int = (
                        SELECT TOP 1 [IdMarca]
                        FROM [dbo].[Marcas]
                        WHERE [Nombre] = N'Sin marca'
                        ORDER BY [IdMarca]);

                    UPDATE p
                    SET [IdMarca] = @IdMarcaPorDefecto
                    FROM [dbo].[Productos] p
                    WHERE p.[IdMarca] IS NULL
                       OR NOT EXISTS (SELECT 1 FROM [dbo].[Marcas] m WHERE m.[IdMarca] = p.[IdMarca]);
                END;

                IF EXISTS (SELECT 1 FROM [dbo].[Productos] WHERE [IdMarca] IS NULL)
                    THROW 51000, 'No se pudieron asignar marcas a todos los productos.', 1;

                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE [object_id] = OBJECT_ID(N'dbo.Productos')
                      AND [name] = N'IdMarca'
                      AND [is_nullable] = 1)
                    ALTER TABLE [dbo].[Productos] ALTER COLUMN [IdMarca] int NOT NULL;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE [object_id] = OBJECT_ID(N'dbo.Productos')
                      AND [name] = N'IX_Productos_IdMarca')
                    CREATE INDEX [IX_Productos_IdMarca] ON [dbo].[Productos] ([IdMarca]);

                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE [parent_object_id] = OBJECT_ID(N'dbo.Productos')
                      AND [name] = N'FK_Productos_Marcas_IdMarca')
                    ALTER TABLE [dbo].[Productos]
                    ADD CONSTRAINT [FK_Productos_Marcas_IdMarca]
                    FOREIGN KEY ([IdMarca]) REFERENCES [dbo].[Marcas] ([IdMarca]);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
