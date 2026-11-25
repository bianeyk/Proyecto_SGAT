using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Subtotal es una columna calculada que depende de Cantidad y
            // CostoUnitario. SQL Server no permite modificar esas columnas
            // mientras la columna calculada exista.
            migrationBuilder.Sql(
                "ALTER TABLE [CompraDetalles] DROP COLUMN [Subtotal];");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoUnitario",
                table: "CompraDetalles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "CompraDetalles",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Clientes",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                defaultValueSql: "CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "NumDoc",
                table: "Clientes",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Clientes",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                defaultValueSql: "CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Auditorias",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                defaultValueSql: "CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "CompraDetalles",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [CostoUnitario]",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // La columna calculada debe quitarse antes de restaurar los tipos
            // antiguos de sus columnas dependientes.
            migrationBuilder.Sql(
                "ALTER TABLE [CompraDetalles] DROP COLUMN [Subtotal];");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoUnitario",
                table: "CompraDetalles",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "CompraDetalles",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldPrecision: 0,
                oldDefaultValueSql: "CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))");

            migrationBuilder.AlterColumn<string>(
                name: "NumDoc",
                table: "Clientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldPrecision: 0,
                oldDefaultValueSql: "CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Auditorias",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldPrecision: 0,
                oldDefaultValueSql: "CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'SA Pacific Standard Time' AS DATETIME2(0))");

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "CompraDetalles",
                type: "decimal(10,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [CostoUnitario]",
                stored: true);
        }
    }
}
