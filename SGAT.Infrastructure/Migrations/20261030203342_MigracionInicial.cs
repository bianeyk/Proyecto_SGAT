using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGAT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasDispositivo",
                columns: table => new
                {
                    IdCategoriaDispositivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasDispositivo", x => x.IdCategoriaDispositivo);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasProducto",
                columns: table => new
                {
                    IdCategoriaProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasProducto", x => x.IdCategoriaProducto);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Politicas = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.IdEmpresa);
                });

            migrationBuilder.CreateTable(
                name: "EstadosServicio",
                columns: table => new
                {
                    IdEstadoServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosServicio", x => x.IdEstadoServicio);
                    table.CheckConstraint("CK_EstadoServicio_Nombre", "[Nombre] IN ('RECIBIDO','DIAGNOSTICO','EN_REPARACION','LISTO','ENTREGADO','CANCELADO','BODEGA')");
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    IdMarca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.IdMarca);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    IdPermiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.IdPermiso);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.IdProveedor);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescripcionBase = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PrecioBase = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.IdServicio);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    IdAuditoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Tabla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistroId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "varchar(max)", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "varchar(max)", nullable: true),
                    Modulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.IdAuditoria);
                    table.CheckConstraint("CK_Auditoria_Accion", "[Accion] IN ('INSERT','UPDATE','DELETE','LOGIN','LOGOUT')");
                    table.ForeignKey(
                        name: "FK_Auditorias_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioCrea = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    NumDoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EsColaborador = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                    table.ForeignKey(
                        name: "FK_Clientes_AspNetUsers_IdUsuarioCrea",
                        column: x => x.IdUsuarioCrea,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCategoriaProducto = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(max)", nullable: true),
                    Condicion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecioDescuento = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    StockActual = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    StockMinimo = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    StockReservado = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IdProducto);
                    table.CheckConstraint("CK_Producto_Condicion", "[Condicion] IN ('NUEVO','USADO','REMANUFACTURADO')");
                    table.ForeignKey(
                        name: "FK_Productos_CategoriasProducto_IdCategoriaProducto",
                        column: x => x.IdCategoriaProducto,
                        principalTable: "CategoriasProducto",
                        principalColumn: "IdCategoriaProducto");
                });

            migrationBuilder.CreateTable(
                name: "RolePermisos",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IdPermiso = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermisos", x => new { x.RoleId, x.IdPermiso });
                    table.ForeignKey(
                        name: "FK_RolePermisos_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RolePermisos_Permisos_IdPermiso",
                        column: x => x.IdPermiso,
                        principalTable: "Permisos",
                        principalColumn: "IdPermiso");
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    IdCompra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    NumFactura = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Observaciones = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.IdCompra);
                    table.ForeignKey(
                        name: "FK_Compras_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Compras_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor");
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    IdEquipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdCategoriaDispositivo = table.Column<int>(type: "int", nullable: false),
                    IdMarca = table.Column<int>(type: "int", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Serial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.IdEquipo);
                    table.ForeignKey(
                        name: "FK_Equipos_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Equipos_CategoriasDispositivo_IdCategoriaDispositivo",
                        column: x => x.IdCategoriaDispositivo,
                        principalTable: "CategoriasDispositivo",
                        principalColumn: "IdCategoriaDispositivo");
                    table.ForeignKey(
                        name: "FK_Equipos_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente");
                    table.ForeignKey(
                        name: "FK_Equipos_Marcas_IdMarca",
                        column: x => x.IdMarca,
                        principalTable: "Marcas",
                        principalColumn: "IdMarca");
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    IdMovimientoInventario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StockAnterior = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    StockNuevo = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.IdMovimientoInventario);
                    table.CheckConstraint("CK_MovimientoInventario_Tipo", "[Tipo] IN ('COMPRA','VENTA','RESERVA','LIBERACION_RESERVA')");
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor");
                });

            migrationBuilder.CreateTable(
                name: "CompraDetalles",
                columns: table => new
                {
                    IdCompraDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCompra = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, computedColumnSql: "[Cantidad] * [CostoUnitario]", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraDetalles", x => x.IdCompraDetalle);
                    table.ForeignKey(
                        name: "FK_CompraDetalles_Compras_IdCompra",
                        column: x => x.IdCompra,
                        principalTable: "Compras",
                        principalColumn: "IdCompra");
                    table.ForeignKey(
                        name: "FK_CompraDetalles_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicio",
                columns: table => new
                {
                    IdOrdenServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEquipo = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioCrea = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IdEstadoServicio = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Diagnostico = table.Column<string>(type: "varchar(max)", nullable: true),
                    CostoTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Abono = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Prioridad = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "MEDIA"),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaPrometida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicio", x => x.IdOrdenServicio);
                    table.CheckConstraint("CK_OrdenServicio_Prioridad", "[Prioridad] IN ('BAJA','MEDIA','ALTA')");
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_AspNetUsers_IdUsuarioCrea",
                        column: x => x.IdUsuarioCrea,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Equipos_IdEquipo",
                        column: x => x.IdEquipo,
                        principalTable: "Equipos",
                        principalColumn: "IdEquipo");
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_EstadosServicio_IdEstadoServicio",
                        column: x => x.IdEstadoServicio,
                        principalTable: "EstadosServicio",
                        principalColumn: "IdEstadoServicio");
                });

            migrationBuilder.CreateTable(
                name: "LotesProducto",
                columns: table => new
                {
                    IdLoteProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCompraDetalle = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    NumeroLote = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Serial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CantidadInicial = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CantidadDisponible = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    GarantiaProveedorDias = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Observaciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaFinGarantiaProveedor = table.Column<DateTime>(type: "datetime2", nullable: false, computedColumnSql: "DATEADD(DAY, [GarantiaProveedorDias], [FechaIngreso])", stored: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotesProducto", x => x.IdLoteProducto);
                    table.ForeignKey(
                        name: "FK_LotesProducto_CompraDetalles_IdCompraDetalle",
                        column: x => x.IdCompraDetalle,
                        principalTable: "CompraDetalles",
                        principalColumn: "IdCompraDetalle");
                    table.ForeignKey(
                        name: "FK_LotesProducto_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    IdFactura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdOrdenServicio = table.Column<int>(type: "int", nullable: true),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    NumeroFactura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaFactura = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Iva = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    MetodoPago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PENDIENTE"),
                    Observaciones = table.Column<string>(type: "varchar(max)", nullable: true),
                    GarantiaDias = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.IdFactura);
                    table.CheckConstraint("CK_Factura_Estado", "[Estado] IN ('PENDIENTE','PAGADA','ANULADA')");
                    table.CheckConstraint("CK_Factura_MetodoPago", "[MetodoPago] IN ('EFECTIVO','TRANSFERENCIA','TARJETA','OTRO')");
                    table.ForeignKey(
                        name: "FK_Facturas_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Facturas_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente");
                    table.ForeignKey(
                        name: "FK_Facturas_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio");
                });

            migrationBuilder.CreateTable(
                name: "HistorialServicios",
                columns: table => new
                {
                    IdHistorialServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenServicio = table.Column<int>(type: "int", nullable: false),
                    IdEstadoServicio = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Comentario = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialServicios", x => x.IdHistorialServicio);
                    table.ForeignKey(
                        name: "FK_HistorialServicios_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialServicios_EstadosServicio_IdEstadoServicio",
                        column: x => x.IdEstadoServicio,
                        principalTable: "EstadosServicio",
                        principalColumn: "IdEstadoServicio");
                    table.ForeignKey(
                        name: "FK_HistorialServicios_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio");
                });

            migrationBuilder.CreateTable(
                name: "OrdenServicioDetalles",
                columns: table => new
                {
                    IdOrdenServicioDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenServicio = table.Column<int>(type: "int", nullable: false),
                    IdServicio = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioCrea = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Observacion = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioCreaId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenServicioDetalles", x => x.IdOrdenServicioDetalle);
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_AspNetUsers_UsuarioCreaId",
                        column: x => x.UsuarioCreaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio");
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalles_Servicios_IdServicio",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio");
                });

            migrationBuilder.CreateTable(
                name: "GarantiasProveedor",
                columns: table => new
                {
                    IdGarantiaProveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLoteProducto = table.Column<int>(type: "int", nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarantiasProveedor", x => x.IdGarantiaProveedor);
                    table.CheckConstraint("CK_GarantiaProveedor_Estado", "[Estado] IN ('ENVIADO','EN_PROCESO','RESUELTO','RECHAZADO')");
                    table.ForeignKey(
                        name: "FK_GarantiasProveedor_LotesProducto_IdLoteProducto",
                        column: x => x.IdLoteProducto,
                        principalTable: "LotesProducto",
                        principalColumn: "IdLoteProducto");
                    table.ForeignKey(
                        name: "FK_GarantiasProveedor_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor");
                });

            migrationBuilder.CreateTable(
                name: "OrdenServicioDetalleProductos",
                columns: table => new
                {
                    IdOrdenServicioDetalleProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdenServicio = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdLoteProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "RESERVADO"),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, computedColumnSql: "[Cantidad] * [PrecioUnitario]", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenServicioDetalleProductos", x => x.IdOrdenServicioDetalleProducto);
                    table.CheckConstraint("CK_OrdenServicioDetalleProducto_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_OrdenServicioDetalleProducto_Estado", "[Estado] IN ('RESERVADO','ENTREGADO','CANCELADO','DEVUELTO')");
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalleProductos_LotesProducto_IdLoteProducto",
                        column: x => x.IdLoteProducto,
                        principalTable: "LotesProducto",
                        principalColumn: "IdLoteProducto");
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalleProductos_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio");
                    table.ForeignKey(
                        name: "FK_OrdenServicioDetalleProductos_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "PagosCartera",
                columns: table => new
                {
                    IdPagoCartera = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFactura = table.Column<int>(type: "int", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdOrdenServicio = table.Column<int>(type: "int", nullable: true),
                    Abono = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    MetodoPago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosCartera", x => x.IdPagoCartera);
                    table.CheckConstraint("CK_PagoCartera_MetodoPago", "[MetodoPago] IN ('EFECTIVO','TRANSFERENCIA','OTRO')");
                    table.ForeignKey(
                        name: "FK_PagosCartera_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente");
                    table.ForeignKey(
                        name: "FK_PagosCartera_Facturas_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Facturas",
                        principalColumn: "IdFactura");
                    table.ForeignKey(
                        name: "FK_PagosCartera_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio");
                });

            migrationBuilder.CreateTable(
                name: "FacturaDetalles",
                columns: table => new
                {
                    IdFacturaDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFactura = table.Column<int>(type: "int", nullable: false),
                    IdLoteProducto = table.Column<int>(type: "int", nullable: true),
                    IdProducto = table.Column<int>(type: "int", nullable: true),
                    IdOrdenServicioDetalle = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 1m),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false, computedColumnSql: "[Cantidad] * [PrecioUnitario]", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaDetalles", x => x.IdFacturaDetalle);
                    table.CheckConstraint("CK_FacturaDetalle_Tipo", "[Tipo] IN ('PRODUCTO','SERVICIO')");
                    table.ForeignKey(
                        name: "FK_FacturaDetalles_Facturas_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Facturas",
                        principalColumn: "IdFactura");
                    table.ForeignKey(
                        name: "FK_FacturaDetalles_LotesProducto_IdLoteProducto",
                        column: x => x.IdLoteProducto,
                        principalTable: "LotesProducto",
                        principalColumn: "IdLoteProducto");
                    table.ForeignKey(
                        name: "FK_FacturaDetalles_OrdenServicioDetalles_IdOrdenServicioDetalle",
                        column: x => x.IdOrdenServicioDetalle,
                        principalTable: "OrdenServicioDetalles",
                        principalColumn: "IdOrdenServicioDetalle");
                    table.ForeignKey(
                        name: "FK_FacturaDetalles_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "GarantiasCliente",
                columns: table => new
                {
                    IdGarantiaCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFacturaDetalle = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    GarantiaDias = table.Column<int>(type: "int", nullable: false),
                    FechaFinGarantia = table.Column<DateTime>(type: "datetime2", nullable: false, computedColumnSql: "DATEADD(DAY, [GarantiaDias], [FechaInicio])", stored: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVA"),
                    Observaciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarantiasCliente", x => x.IdGarantiaCliente);
                    table.CheckConstraint("CK_GarantiaCliente_Estado", "[Estado] IN ('ACTIVA','VENCIDA','ANULADA','PROCESO')");
                    table.ForeignKey(
                        name: "FK_GarantiasCliente_FacturaDetalles_IdFacturaDetalle",
                        column: x => x.IdFacturaDetalle,
                        principalTable: "FacturaDetalles",
                        principalColumn: "IdFacturaDetalle");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_IdUsuario",
                table: "Auditorias",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasDispositivo_Nombre",
                table: "CategoriasDispositivo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasProducto_Nombre",
                table: "CategoriasProducto",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdUsuarioCrea",
                table: "Clientes",
                column: "IdUsuarioCrea");

            migrationBuilder.CreateIndex(
                name: "UQ_Cliente_NumDoc",
                table: "Clientes",
                column: "NumDoc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalles_IdCompra",
                table: "CompraDetalles",
                column: "IdCompra");

            migrationBuilder.CreateIndex(
                name: "IX_CompraDetalles_IdProducto",
                table: "CompraDetalles",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_IdProveedor",
                table: "Compras",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_IdUsuario",
                table: "Compras",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Nit",
                table: "Empresas",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_IdCategoriaDispositivo",
                table: "Equipos",
                column: "IdCategoriaDispositivo");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_IdCliente",
                table: "Equipos",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_IdMarca",
                table: "Equipos",
                column: "IdMarca");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_IdUsuario",
                table: "Equipos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_Serial",
                table: "Equipos",
                column: "Serial",
                unique: true,
                filter: "[Serial] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosServicio_Nombre",
                table: "EstadosServicio",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalles_IdFactura",
                table: "FacturaDetalles",
                column: "IdFactura");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalles_IdLoteProducto",
                table: "FacturaDetalles",
                column: "IdLoteProducto");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalles_IdOrdenServicioDetalle",
                table: "FacturaDetalles",
                column: "IdOrdenServicioDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaDetalles_IdProducto",
                table: "FacturaDetalles",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdCliente",
                table: "Facturas",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdOrdenServicio",
                table: "Facturas",
                column: "IdOrdenServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdUsuario",
                table: "Facturas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_NumeroFactura",
                table: "Facturas",
                column: "NumeroFactura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasCliente_IdFacturaDetalle",
                table: "GarantiasCliente",
                column: "IdFacturaDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasProveedor_IdLoteProducto",
                table: "GarantiasProveedor",
                column: "IdLoteProducto");

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasProveedor_IdProveedor",
                table: "GarantiasProveedor",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialServicios_IdEstadoServicio",
                table: "HistorialServicios",
                column: "IdEstadoServicio");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialServicios_IdOrdenServicio",
                table: "HistorialServicios",
                column: "IdOrdenServicio");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialServicios_IdUsuario",
                table: "HistorialServicios",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_LotesProducto_IdCompraDetalle",
                table: "LotesProducto",
                column: "IdCompraDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_LotesProducto_IdProducto",
                table: "LotesProducto",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Marcas_Nombre",
                table: "Marcas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdProducto",
                table: "MovimientosInventario",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdProveedor",
                table: "MovimientosInventario",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_Codigo",
                table: "OrdenesServicio",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdEquipo",
                table: "OrdenesServicio",
                column: "IdEquipo");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdEstadoServicio",
                table: "OrdenesServicio",
                column: "IdEstadoServicio");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdUsuarioCrea",
                table: "OrdenesServicio",
                column: "IdUsuarioCrea");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalleProductos_IdLoteProducto",
                table: "OrdenServicioDetalleProductos",
                column: "IdLoteProducto");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalleProductos_IdOrdenServicio",
                table: "OrdenServicioDetalleProductos",
                column: "IdOrdenServicio");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalleProductos_IdProducto",
                table: "OrdenServicioDetalleProductos",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_IdOrdenServicio",
                table: "OrdenServicioDetalles",
                column: "IdOrdenServicio");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_IdServicio",
                table: "OrdenServicioDetalles",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioDetalles_UsuarioCreaId",
                table: "OrdenServicioDetalles",
                column: "UsuarioCreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCartera_IdCliente",
                table: "PagosCartera",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCartera_IdFactura",
                table: "PagosCartera",
                column: "IdFactura");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCartera_IdOrdenServicio",
                table: "PagosCartera",
                column: "IdOrdenServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Codigo",
                table: "Permisos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Codigo",
                table: "Productos",
                column: "Codigo",
                unique: true,
                filter: "[Codigo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdCategoriaProducto",
                table: "Productos",
                column: "IdCategoriaProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_Nit",
                table: "Proveedores",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermisos_IdPermiso",
                table: "RolePermisos",
                column: "IdPermiso");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "GarantiasCliente");

            migrationBuilder.DropTable(
                name: "GarantiasProveedor");

            migrationBuilder.DropTable(
                name: "HistorialServicios");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "OrdenServicioDetalleProductos");

            migrationBuilder.DropTable(
                name: "PagosCartera");

            migrationBuilder.DropTable(
                name: "RolePermisos");

            migrationBuilder.DropTable(
                name: "FacturaDetalles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "LotesProducto");

            migrationBuilder.DropTable(
                name: "OrdenServicioDetalles");

            migrationBuilder.DropTable(
                name: "CompraDetalles");

            migrationBuilder.DropTable(
                name: "OrdenesServicio");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "EstadosServicio");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "CategoriasProducto");

            migrationBuilder.DropTable(
                name: "CategoriasDispositivo");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
