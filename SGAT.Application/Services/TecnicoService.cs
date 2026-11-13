using SGAT.Application.DTOs.Tecnico;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGAT.Application.Services
{
    public class TecnicoService : ITecnicoService
    {
        private readonly ITecnicoRepository _tecnicoRepo;

        public TecnicoService(ITecnicoRepository tecnicoRepo)
        {
            _tecnicoRepo = tecnicoRepo;
        }

        public async Task<IEnumerable<OrdenTecnicoResumenDto>> ObtenerOrdenesAsync(EstadoOrden? estado = null, string? tecnicoId = null)
        {
            var ordenes = await _tecnicoRepo.GetOrdenesAsync(estado, tecnicoId);

            return ordenes.Select(o => new OrdenTecnicoResumenDto
            {
                Id = o.IdOrdenServicio,
                NumeroOrden = o.Codigo,
                Dispositivo = o.Equipo?.CategoriaDispositivo?.Nombre ?? o.Equipo?.Descripcion ?? "Sin especificar",
                Marca = o.Equipo?.Marca?.Nombre ?? "Genérica",
                Modelo = o.Equipo?.Modelo ?? "N/A",
                FallaReportada = o.DiagnosticoTecnico?.FallaReportada ?? o.Diagnostico ?? o.Observaciones,
                Estado = o.EstadoOrden.ToString(),
                EstadoValor = (int)o.EstadoOrden,
                Prioridad = o.Prioridad.ToString(),
                EstadoServicioNombre = o.EstadoServicio?.Nombre ?? "RECIBIDO",
                CostoTotal = o.CostoTotal,
                FechaIngreso = o.FechaIngreso
            });
        }

        public async Task<DetalleOrdenTecnicoDto?> ObtenerDetalleOrdenAsync(int id)
        {
            var o = await _tecnicoRepo.GetDetalleOrdenAsync(id);
            if (o == null) return null;

            // Combinar y ordenar historial de bitácora técnica e historial de servicio
            var notasBitacora = o.Bitacoras.Select(b => new BitacoraTecnicaItemDto
            {
                Id = b.Id,
                Nota = b.Nota,
                FechaRegistro = b.FechaRegistro
            });

            var notasHistorial = o.HistorialesServicio
                .Where(h => !string.IsNullOrWhiteSpace(h.Comentario))
                .Select(h => new BitacoraTecnicaItemDto
                {
                    Id = h.IdHistorialServicio,
                    Nota = $"[{h.EstadoServicio?.Nombre ?? "Estado"}]: {h.Comentario}",
                    FechaRegistro = h.CreatedAt
                });

            // Evitar notas repetidas y ordenar cronológicamente descendente
            var historialUnificado = notasBitacora
                .Concat(notasHistorial)
                .GroupBy(n => n.Nota)
                .Select(g => g.OrderByDescending(x => x.FechaRegistro).First())
                .OrderByDescending(b => b.FechaRegistro)
                .ToList();

            return new DetalleOrdenTecnicoDto
            {
                Id = o.IdOrdenServicio,
                NumeroOrden = o.Codigo,
                Estado = o.EstadoOrden.ToString(),
                EstadoValor = (int)o.EstadoOrden,
                EstadoServicioId = o.IdEstadoServicio,
                EstadoServicioNombre = o.EstadoServicio?.Nombre ?? "RECIBIDO",
                Prioridad = o.Prioridad.ToString(),
                FechaIngreso = o.FechaIngreso,
                FechaPrometida = o.FechaPrometida,
                FechaCierre = o.FechaCierre,
                DiagnosticoInicial = o.Diagnostico,
                Observaciones = o.Observaciones,
                CostoTotal = o.CostoTotal,
                Abono = o.Abono,
                UsuarioCreaNombre = o.UsuarioCrea?.NombreCompleto ?? o.UsuarioCrea?.UserName ?? o.IdUsuarioCrea,
                Cliente = new ClienteTecnicoDto
                {
                    Id = o.Equipo?.Cliente?.IdCliente ?? 0,
                    Nombre = o.Equipo?.Cliente?.Nombre ?? "No registrado",
                    NumDoc = o.Equipo?.Cliente?.NumDoc ?? string.Empty,
                    Telefono = o.Equipo?.Cliente?.Telefono ?? string.Empty,
                    Email = o.Equipo?.Cliente?.Email,
                    Direccion = o.Equipo?.Cliente?.Direccion
                },
                Equipo = new EquipoTecnicoDto
                {
                    Id = o.Equipo?.IdEquipo ?? 0,
                    Categoria = o.Equipo?.CategoriaDispositivo?.Nombre ?? "Sin categoría",
                    Marca = o.Equipo?.Marca?.Nombre ?? "Genérica",
                    Modelo = o.Equipo?.Modelo ?? string.Empty,
                    Serial = o.Equipo?.Serial,
                    Descripcion = o.Equipo?.Descripcion
                },
                DiagnosticoActual = o.DiagnosticoTecnico == null ? null : new DiagnosticoTecnicoDetalleDto
                {
                    Id = o.DiagnosticoTecnico.Id,
                    FallaReportada = o.DiagnosticoTecnico.FallaReportada,
                    DiagnosticoDetallado = o.DiagnosticoTecnico.DiagnosticoDetallado,
                    SolucionAplicada = o.DiagnosticoTecnico.SolucionAplicada,
                    RequiereRepuestos = o.DiagnosticoTecnico.RequiereRepuestos,
                    RepuestosSugeridos = o.DiagnosticoTecnico.RepuestosSugeridos,
                    CostoManoDeObra = o.DiagnosticoTecnico.CostoManoDeObra,
                    CostoTotalEstimado = o.DiagnosticoTecnico.CostoTotalEstimado,
                    FechaInicio = o.DiagnosticoTecnico.FechaInicio,
                    FechaFinalizacion = o.DiagnosticoTecnico.FechaFinalizacion,
                    TecnicoId = o.DiagnosticoTecnico.TecnicoId,
                    TecnicoNombre = o.DiagnosticoTecnico.Tecnico?.NombreCompleto ?? o.DiagnosticoTecnico.Tecnico?.UserName ?? "Técnico Asignado"
                },
                HistorialNotas = historialUnificado,
                ServiciosAplicados = o.Detalles.Select(d => new ServicioAplicadoTecnicoDto
                {
                    Id = d.IdOrdenServicioDetalle,
                    ServicioId = d.IdServicio,
                    ServicioNombre = d.Servicio?.Nombre ?? "Servicio",
                    Descripcion = d.Descripcion,
                    Observacion = d.Observacion,
                    FechaRegistro = d.CreatedAt,
                    UsuarioCrea = d.UsuarioCrea?.NombreCompleto ?? d.UsuarioCrea?.UserName ?? d.IdUsuarioCrea
                }).ToList(),
                RepuestosAplicados = o.DetallesProducto.Select(dp => new RepuestoAplicadoTecnicoDto
                {
                    Id = dp.IdOrdenServicioDetalleProducto,
                    ProductoId = dp.IdProducto,
                    ProductoNombre = dp.Producto?.Nombre ?? "Repuesto",
                    LoteId = dp.IdLoteProducto,
                    Cantidad = dp.Cantidad,
                    PrecioUnitario = dp.PrecioUnitario,
                    Subtotal = dp.Subtotal,
                    Estado = dp.Estado.ToString()
                }).ToList()
            };
        }

        public async Task<bool> IniciarDiagnosticoAsync(IniciarDiagnosticoDto dto)
        {
            var orden = await _tecnicoRepo.GetDetalleOrdenAsync(dto.OrdenId);
            if (orden == null) return false;

            if (orden.DiagnosticoTecnico == null)
            {
                var diag = new DiagnosticoTecnico
                {
                    OrdenId = dto.OrdenId,
                    TecnicoId = dto.TecnicoId,
                    FallaReportada = dto.FallaReportada ?? orden.Diagnostico ?? orden.Observaciones,
                    DiagnosticoDetallado = "Inicio de diagnóstico técnico.",
                    FechaInicio = DateTime.UtcNow
                };
                await _tecnicoRepo.AddDiagnosticoAsync(diag);
            }
            else
            {
                orden.DiagnosticoTecnico.TecnicoId = dto.TecnicoId;
                if (!string.IsNullOrWhiteSpace(dto.FallaReportada))
                {
                    orden.DiagnosticoTecnico.FallaReportada = dto.FallaReportada;
                }
                _tecnicoRepo.UpdateDiagnostico(orden.DiagnosticoTecnico);
            }

            // Sincronizar ambos estados (Enum y Relacional de BD)
            orden.EstadoOrden = EstadoOrden.EnDiagnostico;
            orden.IdEstadoServicio = MapearAEstadoServicioId(EstadoOrden.EnDiagnostico);
            _tecnicoRepo.UpdateOrden(orden);

            var notaTexto = $"Diagnóstico iniciado y asignado al técnico ID '{dto.TecnicoId}'.";

            // Guardar en bitácora técnica
            await _tecnicoRepo.AddBitacoraAsync(new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = notaTexto,
                FechaRegistro = DateTime.UtcNow
            });

            // Sincronizar en historial oficial de servicios del sistema
            await _tecnicoRepo.AddHistorialAsync(new HistorialServicio
            {
                IdOrdenServicio = dto.OrdenId,
                IdEstadoServicio = orden.IdEstadoServicio,
                IdUsuario = dto.TecnicoId,
                Comentario = notaTexto,
                CreatedAt = DateTime.UtcNow
            });

            return await _tecnicoRepo.SaveChangesAsync();
        }

        public async Task<bool> RegistrarDiagnosticoAsync(RegistrarDiagnosticoDto dto)
        {
            var orden = await _tecnicoRepo.GetDetalleOrdenAsync(dto.OrdenId);
            if (orden == null) return false;

            var tecnicoId = orden.DiagnosticoTecnico?.TecnicoId ?? orden.IdUsuarioCrea;

            if (orden.DiagnosticoTecnico == null)
            {
                var diag = new DiagnosticoTecnico
                {
                    OrdenId = dto.OrdenId,
                    TecnicoId = tecnicoId,
                    FallaReportada = orden.Diagnostico ?? orden.Observaciones,
                    DiagnosticoDetallado = dto.DiagnosticoDetallado,
                    RequiereRepuestos = dto.RequiereRepuestos,
                    RepuestosSugeridos = dto.RepuestosSugeridos,
                    CostoManoDeObra = dto.CostoManoDeObra,
                    CostoTotalEstimado = dto.CostoManoDeObra,
                    FechaInicio = DateTime.UtcNow
                };
                await _tecnicoRepo.AddDiagnosticoAsync(diag);
            }
            else
            {
                orden.DiagnosticoTecnico.DiagnosticoDetallado = dto.DiagnosticoDetallado;
                orden.DiagnosticoTecnico.RequiereRepuestos = dto.RequiereRepuestos;
                orden.DiagnosticoTecnico.RepuestosSugeridos = dto.RepuestosSugeridos;
                orden.DiagnosticoTecnico.CostoManoDeObra = dto.CostoManoDeObra;
                orden.DiagnosticoTecnico.CostoTotalEstimado = dto.CostoManoDeObra;
                _tecnicoRepo.UpdateDiagnostico(orden.DiagnosticoTecnico);
            }

            // Sincronizar ambos estados y campo de diagnóstico de la orden
            orden.EstadoOrden = dto.NuevoEstado;
            orden.IdEstadoServicio = MapearAEstadoServicioId(dto.NuevoEstado);
            orden.Diagnostico = dto.DiagnosticoDetallado;
            _tecnicoRepo.UpdateOrden(orden);

            var notaTexto = $"Diagnóstico actualizado: {dto.DiagnosticoDetallado}. Nuevo estado: {dto.NuevoEstado}. Requiere repuestos: {(dto.RequiereRepuestos ? "Sí" : "No")}.";

            // Guardar en bitácora técnica
            await _tecnicoRepo.AddBitacoraAsync(new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = notaTexto,
                FechaRegistro = DateTime.UtcNow
            });

            // Sincronizar en historial oficial de servicios
            await _tecnicoRepo.AddHistorialAsync(new HistorialServicio
            {
                IdOrdenServicio = dto.OrdenId,
                IdEstadoServicio = orden.IdEstadoServicio,
                IdUsuario = tecnicoId,
                Comentario = notaTexto,
                CreatedAt = DateTime.UtcNow
            });

            return await _tecnicoRepo.SaveChangesAsync();
        }

        public async Task<bool> AgregarNotaBitacoraAsync(AgregarNotaBitacoraDto dto)
        {
            var orden = await _tecnicoRepo.GetDetalleOrdenAsync(dto.OrdenId);
            if (orden == null) return false;

            var tecnicoId = orden.DiagnosticoTecnico?.TecnicoId ?? orden.IdUsuarioCrea;

            var nota = new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = dto.Nota,
                FechaRegistro = DateTime.UtcNow
            };

            await _tecnicoRepo.AddBitacoraAsync(nota);

            // Sincronizar en historial oficial
            await _tecnicoRepo.AddHistorialAsync(new HistorialServicio
            {
                IdOrdenServicio = dto.OrdenId,
                IdEstadoServicio = orden.IdEstadoServicio,
                IdUsuario = tecnicoId,
                Comentario = dto.Nota,
                CreatedAt = DateTime.UtcNow
            });

            return await _tecnicoRepo.SaveChangesAsync();
        }

        public async Task<bool> FinalizarReparacionAsync(FinalizarReparacionDto dto)
        {
            var orden = await _tecnicoRepo.GetDetalleOrdenAsync(dto.OrdenId);
            if (orden == null) return false;

            var tecnicoId = orden.DiagnosticoTecnico?.TecnicoId ?? orden.IdUsuarioCrea;

            if (orden.DiagnosticoTecnico != null)
            {
                orden.DiagnosticoTecnico.SolucionAplicada = dto.SolucionAplicada;
                orden.DiagnosticoTecnico.FechaFinalizacion = DateTime.UtcNow;
                orden.DiagnosticoTecnico.CostoTotalEstimado = dto.CostoFinal;
                _tecnicoRepo.UpdateDiagnostico(orden.DiagnosticoTecnico);
            }

            // Sincronizar ambos estados y cierre
            orden.EstadoOrden = EstadoOrden.Finalizado;
            orden.IdEstadoServicio = MapearAEstadoServicioId(EstadoOrden.Finalizado);
            orden.CostoTotal = dto.CostoFinal;
            orden.FechaCierre = DateTime.UtcNow;
            _tecnicoRepo.UpdateOrden(orden);

            var notaTexto = $"Reparación finalizada con éxito. Solución: {dto.SolucionAplicada}. Costo total: {dto.CostoFinal:N2}.";

            await _tecnicoRepo.AddBitacoraAsync(new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = notaTexto,
                FechaRegistro = DateTime.UtcNow
            });

            // Sincronizar en historial oficial
            await _tecnicoRepo.AddHistorialAsync(new HistorialServicio
            {
                IdOrdenServicio = dto.OrdenId,
                IdEstadoServicio = orden.IdEstadoServicio,
                IdUsuario = tecnicoId,
                Comentario = notaTexto,
                CreatedAt = DateTime.UtcNow
            });

            return await _tecnicoRepo.SaveChangesAsync();
        }

        /// <summary>
        /// Mapea el enum EstadoOrden a los IDs oficiales de la tabla EstadosServicio en BD
        /// 1=RECIBIDO, 2=DIAGNOSTICO, 3=EN_REPARACION, 4=LISTO, 5=ENTREGADO, 6=CANCELADO, 7=BODEGA
        /// </summary>
        private static int MapearAEstadoServicioId(EstadoOrden estadoOrden)
        {
            return estadoOrden switch
            {
                EstadoOrden.PendienteRevision => 1, // RECIBIDO
                EstadoOrden.EnDiagnostico => 2,     // DIAGNOSTICO
                EstadoOrden.EsperandoRepuestos => 7,// BODEGA / EN ESPERA
                EstadoOrden.EnReparacion => 3,      // EN_REPARACION
                EstadoOrden.Finalizado => 4,        // LISTO
                EstadoOrden.NoReparable => 6,       // CANCELADO
                _ => 1
            };
        }
    }
}
