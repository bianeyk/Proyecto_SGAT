using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseIntegerQuantitiesAndDecimal18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM Productos WHERE StockActual <> FLOOR(StockActual) OR StockMinimo <> FLOOR(StockMinimo) OR StockReservado <> FLOOR(StockReservado))
    THROW 51000, 'No se pueden convertir a enteros los stocks de Productos: hay valores fraccionarios.', 1;
IF EXISTS (SELECT 1 FROM CompraDetalles WHERE Cantidad <> FLOOR(Cantidad))
    THROW 51000, 'No se pueden convertir a enteros las cantidades de CompraDetalles: hay valores fraccionarios.', 1;
IF EXISTS (SELECT 1 FROM FacturaDetalles WHERE Cantidad <> FLOOR(Cantidad))
    THROW 51000, 'No se pueden convertir a enteros las cantidades de FacturaDetalles: hay valores fraccionarios.', 1;
IF EXISTS (SELECT 1 FROM LotesProducto WHERE CantidadInicial <> FLOOR(CantidadInicial) OR CantidadDisponible <> FLOOR(CantidadDisponible))
    THROW 51000, 'No se pueden convertir a enteros las cantidades de LotesProducto: hay valores fraccionarios.', 1;
IF EXISTS (SELECT 1 FROM MovimientosInventario WHERE Cantidad <> FLOOR(Cantidad) OR (StockAnterior IS NOT NULL AND StockAnterior <> FLOOR(StockAnterior)) OR (StockNuevo IS NOT NULL AND StockNuevo <> FLOOR(StockNuevo)))
    THROW 51000, 'No se pueden convertir a enteros las cantidades de MovimientosInventario: hay valores fraccionarios.', 1;
IF EXISTS (SELECT 1 FROM OrdenServicioDetalleProductos WHERE Cantidad <> FLOOR(Cantidad))
    THROW 51000, 'No se pueden convertir a enteros las cantidades de OrdenServicioDetalleProductos: hay valores fraccionarios.', 1;
");

            migrationBuilder.DropCheckConstraint("CK_Producto_PrecioVenta", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_PrecioDescuento", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_StockActual", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_StockMinimo", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_StockReservado", "Productos");
            migrationBuilder.DropCheckConstraint("CK_PagoCartera_Abono", "PagosCartera");
            migrationBuilder.DropCheckConstraint("CK_OrdenServicioDetalleProducto_Cantidad", "OrdenServicioDetalleProductos");
            migrationBuilder.DropCheckConstraint("CK_MovimientoInventario_Cantidad", "MovimientosInventario");
            migrationBuilder.DropCheckConstraint("CK_MovimientoInventario_StockAnterior", "MovimientosInventario");
            migrationBuilder.DropCheckConstraint("CK_MovimientoInventario_StockNuevo", "MovimientosInventario");
            migrationBuilder.DropCheckConstraint("CK_LoteProducto_CantidadDisponible", "LotesProducto");
            migrationBuilder.DropCheckConstraint("CK_LoteProducto_CantidadInicial", "LotesProducto");
            migrationBuilder.DropCheckConstraint("CK_LoteProducto_CostoUnitario", "LotesProducto");
            migrationBuilder.DropCheckConstraint("CK_Factura_Total", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_Factura_Subtotal", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_Factura_SaldoPendiente", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_Factura_Iva", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_FacturaDetalle_Cantidad", "FacturaDetalles");
            migrationBuilder.DropCheckConstraint("CK_FacturaDetalle_PrecioUnitario", "FacturaDetalles");
            migrationBuilder.DropCheckConstraint("CK_Compra_Total", "Compras");
            migrationBuilder.DropCheckConstraint("CK_CompraDetalle_Cantidad", "CompraDetalles");
            migrationBuilder.DropCheckConstraint("CK_CompraDetalle_CostoUnitario", "CompraDetalles");

            migrationBuilder.DropColumn("Subtotal", "OrdenServicioDetalleProductos");
            migrationBuilder.DropColumn("Subtotal", "FacturaDetalles");
            migrationBuilder.DropColumn("Subtotal", "CompraDetalles");

            migrationBuilder.AlterColumn<int>(
                name: "StockReservado",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "StockMinimo",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "StockActual",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioVenta",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioDescuento",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Abono",
                table: "PagosCartera",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "OrdenServicioDetalleProductos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "OrdenServicioDetalleProductos",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoTotal",
                table: "OrdenesServicio",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Abono",
                table: "OrdenesServicio",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "StockNuevo",
                table: "MovimientosInventario",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StockAnterior",
                table: "MovimientosInventario",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "MovimientosInventario",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoUnitario",
                table: "LotesProducto",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "CantidadInicial",
                table: "LotesProducto",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "CantidadDisponible",
                table: "LotesProducto",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Facturas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Subtotal",
                table: "Facturas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoPendiente",
                table: "Facturas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Iva",
                table: "Facturas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "FacturaDetalles",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "FacturaDetalles",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldDefaultValue: 1m);

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

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "OrdenServicioDetalleProductos",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [PrecioUnitario]",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "FacturaDetalles",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [PrecioUnitario]",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "CompraDetalles",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [CostoUnitario]",
                stored: true);

            migrationBuilder.AddCheckConstraint("CK_Producto_PrecioVenta", "Productos", "[PrecioVenta] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_PrecioDescuento", "Productos", "[PrecioDescuento] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_StockActual", "Productos", "[StockActual] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_StockMinimo", "Productos", "[StockMinimo] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_StockReservado", "Productos", "[StockReservado] >= 0");
            migrationBuilder.AddCheckConstraint("CK_PagoCartera_Abono", "PagosCartera", "[Abono] > 0");
            migrationBuilder.AddCheckConstraint("CK_OrdenServicioDetalleProducto_Cantidad", "OrdenServicioDetalleProductos", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_MovimientoInventario_Cantidad", "MovimientosInventario", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_MovimientoInventario_StockAnterior", "MovimientosInventario", "[StockAnterior] >= 0");
            migrationBuilder.AddCheckConstraint("CK_MovimientoInventario_StockNuevo", "MovimientosInventario", "[StockNuevo] >= 0");
            migrationBuilder.AddCheckConstraint("CK_LoteProducto_CantidadDisponible", "LotesProducto", "[CantidadDisponible] >= 0");
            migrationBuilder.AddCheckConstraint("CK_LoteProducto_CantidadInicial", "LotesProducto", "[CantidadInicial] > 0");
            migrationBuilder.AddCheckConstraint("CK_LoteProducto_CostoUnitario", "LotesProducto", "[CostoUnitario] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_Total", "Facturas", "[Total] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_Subtotal", "Facturas", "[Subtotal] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_SaldoPendiente", "Facturas", "[SaldoPendiente] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_Iva", "Facturas", "[Iva] >= 0");
            migrationBuilder.AddCheckConstraint("CK_FacturaDetalle_Cantidad", "FacturaDetalles", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_FacturaDetalle_PrecioUnitario", "FacturaDetalles", "[PrecioUnitario] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Compra_Total", "Compras", "[Total] >= 0");
            migrationBuilder.AddCheckConstraint("CK_CompraDetalle_Cantidad", "CompraDetalles", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_CompraDetalle_CostoUnitario", "CompraDetalles", "[CostoUnitario] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint("CK_Producto_PrecioVenta", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_PrecioDescuento", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_StockActual", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_StockMinimo", "Productos");
            migrationBuilder.DropCheckConstraint("CK_Producto_StockReservado", "Productos");
            migrationBuilder.DropCheckConstraint("CK_PagoCartera_Abono", "PagosCartera");
            migrationBuilder.DropCheckConstraint("CK_OrdenServicioDetalleProducto_Cantidad", "OrdenServicioDetalleProductos");
            migrationBuilder.DropCheckConstraint("CK_MovimientoInventario_Cantidad", "MovimientosInventario");
            migrationBuilder.DropCheckConstraint("CK_MovimientoInventario_StockAnterior", "MovimientosInventario");
            migrationBuilder.DropCheckConstraint("CK_MovimientoInventario_StockNuevo", "MovimientosInventario");
            migrationBuilder.DropCheckConstraint("CK_LoteProducto_CantidadDisponible", "LotesProducto");
            migrationBuilder.DropCheckConstraint("CK_LoteProducto_CantidadInicial", "LotesProducto");
            migrationBuilder.DropCheckConstraint("CK_LoteProducto_CostoUnitario", "LotesProducto");
            migrationBuilder.DropCheckConstraint("CK_Factura_Total", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_Factura_Subtotal", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_Factura_SaldoPendiente", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_Factura_Iva", "Facturas");
            migrationBuilder.DropCheckConstraint("CK_FacturaDetalle_Cantidad", "FacturaDetalles");
            migrationBuilder.DropCheckConstraint("CK_FacturaDetalle_PrecioUnitario", "FacturaDetalles");
            migrationBuilder.DropCheckConstraint("CK_Compra_Total", "Compras");
            migrationBuilder.DropCheckConstraint("CK_CompraDetalle_Cantidad", "CompraDetalles");
            migrationBuilder.DropCheckConstraint("CK_CompraDetalle_CostoUnitario", "CompraDetalles");

            migrationBuilder.DropColumn("Subtotal", "OrdenServicioDetalleProductos");
            migrationBuilder.DropColumn("Subtotal", "FacturaDetalles");
            migrationBuilder.DropColumn("Subtotal", "CompraDetalles");

            migrationBuilder.AlterColumn<decimal>(
                name: "StockReservado",
                table: "Productos",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "StockReservado",
                table: "Productos",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "StockMinimo",
                table: "Productos",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "StockActual",
                table: "Productos",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioVenta",
                table: "Productos",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioDescuento",
                table: "Productos",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Abono",
                table: "PagosCartera",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "OrdenServicioDetalleProductos",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "OrdenServicioDetalleProductos",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoTotal",
                table: "OrdenesServicio",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Abono",
                table: "OrdenesServicio",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "StockNuevo",
                table: "MovimientosInventario",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "StockAnterior",
                table: "MovimientosInventario",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "MovimientosInventario",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostoUnitario",
                table: "LotesProducto",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CantidadInicial",
                table: "LotesProducto",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "CantidadDisponible",
                table: "LotesProducto",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Facturas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Subtotal",
                table: "Facturas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "SaldoPendiente",
                table: "Facturas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Iva",
                table: "Facturas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "FacturaDetalles",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "FacturaDetalles",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

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

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "OrdenServicioDetalleProductos",
                type: "decimal(10,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [PrecioUnitario]",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "FacturaDetalles",
                type: "decimal(10,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [PrecioUnitario]",
                stored: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "CompraDetalles",
                type: "decimal(10,2)",
                nullable: false,
                computedColumnSql: "[Cantidad] * [CostoUnitario]",
                stored: true);

            migrationBuilder.AddCheckConstraint("CK_Producto_PrecioVenta", "Productos", "[PrecioVenta] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_PrecioDescuento", "Productos", "[PrecioDescuento] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_StockActual", "Productos", "[StockActual] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_StockMinimo", "Productos", "[StockMinimo] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Producto_StockReservado", "Productos", "[StockReservado] >= 0");
            migrationBuilder.AddCheckConstraint("CK_PagoCartera_Abono", "PagosCartera", "[Abono] > 0");
            migrationBuilder.AddCheckConstraint("CK_OrdenServicioDetalleProducto_Cantidad", "OrdenServicioDetalleProductos", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_MovimientoInventario_Cantidad", "MovimientosInventario", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_MovimientoInventario_StockAnterior", "MovimientosInventario", "[StockAnterior] >= 0");
            migrationBuilder.AddCheckConstraint("CK_MovimientoInventario_StockNuevo", "MovimientosInventario", "[StockNuevo] >= 0");
            migrationBuilder.AddCheckConstraint("CK_LoteProducto_CantidadDisponible", "LotesProducto", "[CantidadDisponible] >= 0");
            migrationBuilder.AddCheckConstraint("CK_LoteProducto_CantidadInicial", "LotesProducto", "[CantidadInicial] > 0");
            migrationBuilder.AddCheckConstraint("CK_LoteProducto_CostoUnitario", "LotesProducto", "[CostoUnitario] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_Total", "Facturas", "[Total] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_Subtotal", "Facturas", "[Subtotal] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_SaldoPendiente", "Facturas", "[SaldoPendiente] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Factura_Iva", "Facturas", "[Iva] >= 0");
            migrationBuilder.AddCheckConstraint("CK_FacturaDetalle_Cantidad", "FacturaDetalles", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_FacturaDetalle_PrecioUnitario", "FacturaDetalles", "[PrecioUnitario] >= 0");
            migrationBuilder.AddCheckConstraint("CK_Compra_Total", "Compras", "[Total] >= 0");
            migrationBuilder.AddCheckConstraint("CK_CompraDetalle_Cantidad", "CompraDetalles", "[Cantidad] > 0");
            migrationBuilder.AddCheckConstraint("CK_CompraDetalle_CostoUnitario", "CompraDetalles", "[CostoUnitario] >= 0");
        }
    }
}
