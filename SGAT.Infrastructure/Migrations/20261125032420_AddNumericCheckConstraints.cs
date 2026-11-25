using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNumericCheckConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Producto_PrecioDescuento",
                table: "Productos",
                sql: "[PrecioDescuento] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Producto_PrecioVenta",
                table: "Productos",
                sql: "[PrecioVenta] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Producto_StockActual",
                table: "Productos",
                sql: "[StockActual] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Producto_StockMinimo",
                table: "Productos",
                sql: "[StockMinimo] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Producto_StockReservado",
                table: "Productos",
                sql: "[StockReservado] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PagoCartera_Abono",
                table: "PagosCartera",
                sql: "[Abono] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MovimientoInventario_Cantidad",
                table: "MovimientosInventario",
                sql: "[Cantidad] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MovimientoInventario_StockAnterior",
                table: "MovimientosInventario",
                sql: "[StockAnterior] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MovimientoInventario_StockNuevo",
                table: "MovimientosInventario",
                sql: "[StockNuevo] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LoteProducto_CantidadDisponible",
                table: "LotesProducto",
                sql: "[CantidadDisponible] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LoteProducto_CantidadInicial",
                table: "LotesProducto",
                sql: "[CantidadInicial] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LoteProducto_CostoUnitario",
                table: "LotesProducto",
                sql: "[CostoUnitario] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LoteProducto_GarantiaProveedorDias",
                table: "LotesProducto",
                sql: "[GarantiaProveedorDias] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Factura_GarantiaDias",
                table: "Facturas",
                sql: "[GarantiaDias] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Factura_Iva",
                table: "Facturas",
                sql: "[Iva] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Factura_SaldoPendiente",
                table: "Facturas",
                sql: "[SaldoPendiente] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Factura_Subtotal",
                table: "Facturas",
                sql: "[Subtotal] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Factura_Total",
                table: "Facturas",
                sql: "[Total] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FacturaDetalle_Cantidad",
                table: "FacturaDetalles",
                sql: "[Cantidad] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FacturaDetalle_PrecioUnitario",
                table: "FacturaDetalles",
                sql: "[PrecioUnitario] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Compra_Total",
                table: "Compras",
                sql: "[Total] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CompraDetalle_Cantidad",
                table: "CompraDetalles",
                sql: "[Cantidad] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CompraDetalle_CostoUnitario",
                table: "CompraDetalles",
                sql: "[CostoUnitario] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Producto_PrecioDescuento",
                table: "Productos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Producto_PrecioVenta",
                table: "Productos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Producto_StockActual",
                table: "Productos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Producto_StockMinimo",
                table: "Productos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Producto_StockReservado",
                table: "Productos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PagoCartera_Abono",
                table: "PagosCartera");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MovimientoInventario_Cantidad",
                table: "MovimientosInventario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MovimientoInventario_StockAnterior",
                table: "MovimientosInventario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MovimientoInventario_StockNuevo",
                table: "MovimientosInventario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LoteProducto_CantidadDisponible",
                table: "LotesProducto");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LoteProducto_CantidadInicial",
                table: "LotesProducto");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LoteProducto_CostoUnitario",
                table: "LotesProducto");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LoteProducto_GarantiaProveedorDias",
                table: "LotesProducto");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Factura_GarantiaDias",
                table: "Facturas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Factura_Iva",
                table: "Facturas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Factura_SaldoPendiente",
                table: "Facturas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Factura_Subtotal",
                table: "Facturas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Factura_Total",
                table: "Facturas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FacturaDetalle_Cantidad",
                table: "FacturaDetalles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FacturaDetalle_PrecioUnitario",
                table: "FacturaDetalles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Compra_Total",
                table: "Compras");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CompraDetalle_Cantidad",
                table: "CompraDetalles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CompraDetalle_CostoUnitario",
                table: "CompraDetalles");

        }
    }
}
