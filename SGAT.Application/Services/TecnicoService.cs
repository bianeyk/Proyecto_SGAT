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
                FechaIngreso = o.FechaIngreso
            });
        }

        public async Task<DetalleOrdenTecnicoDto?> ObtenerDetalleOrdenAsync(int id)
        {
            var o = await _tecnicoRepo.GetDetalleOrdenAsync(id);
            if (o == null) return null;

            return new DetalleOrdenTecnicoDto
            {
                Id = o.IdOrdenServicio,
                NumeroOrden = o.Codigo,
                Estado = o.EstadoOrden.ToString(),
                EstadoValor = (int)o.EstadoOrden,
                Prioridad = o.Prioridad.ToString(),
                FechaIngreso = o.FechaIngreso,
                FechaPrometida = o.FechaPrometida,
                Observaciones = o.Observaciones,
                Cliente = new ClienteTecnicoDto
                {
                    Id = o.Equipo?.Cliente?.IdCliente ?? 0,
                    Nombre = o.Equipo?.Cliente?.Nombre ?? "No registrado",
                    NumDoc = o.Equipo?.Cliente?.NumDoc ?? string.Empty,
                    Telefono = o.Equipo?.Cliente?.Telefono ?? string.Empty,
                    Email = o.Equipo?.Cliente?.Email
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
                HistorialNotas = o.Bitacoras
                    .OrderByDescending(b => b.FechaRegistro)
                    .Select(b => new BitacoraTecnicaItemDto
                    {
                        Id = b.Id,
                        Nota = b.Nota,
                        FechaRegistro = b.FechaRegistro
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

            orden.EstadoOrden = EstadoOrden.EnDiagnostico;
            _tecnicoRepo.UpdateOrden(orden);

            await _tecnicoRepo.AddBitacoraAsync(new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = $"Diagnóstico iniciado y asignado al técnico ID '{dto.TecnicoId}'.",
                FechaRegistro = DateTime.UtcNow
            });

            return await _tecnicoRepo.SaveChangesAsync();
        }

        public async Task<bool> RegistrarDiagnosticoAsync(RegistrarDiagnosticoDto dto)
        {
            var orden = await _tecnicoRepo.GetDetalleOrdenAsync(dto.OrdenId);
            if (orden == null) return false;

            if (orden.DiagnosticoTecnico == null)
            {
                var diag = new DiagnosticoTecnico
                {
                    OrdenId = dto.OrdenId,
                    TecnicoId = orden.IdUsuarioCrea,
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

            orden.EstadoOrden = dto.NuevoEstado;
            orden.Diagnostico = dto.DiagnosticoDetallado;
            _tecnicoRepo.UpdateOrden(orden);

            await _tecnicoRepo.AddBitacoraAsync(new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = $"Diagnóstico actualizado: {dto.DiagnosticoDetallado}. Nuevo estado: {dto.NuevoEstado}. Requiere repuestos: {(dto.RequiereRepuestos ? "Sí" : "No")}.",
                FechaRegistro = DateTime.UtcNow
            });

            return await _tecnicoRepo.SaveChangesAsync();
        }

        public async Task<bool> AgregarNotaBitacoraAsync(AgregarNotaBitacoraDto dto)
        {
            var orden = await _tecnicoRepo.GetDetalleOrdenAsync(dto.OrdenId);
            if (orden == null) return false;

            var nota = new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = dto.Nota,
                FechaRegistro = DateTime.UtcNow
            };

            await _tecnicoRepo.AddBitacoraAsync(nota);
            return await _tecnicoRepo.SaveChangesAsync();
        }

        public async Task<bool> FinalizarReparacionAsync(FinalizarReparacionDto dto)
        {
            var orden = await _tecnicoRepo.GetDetalleOrdenAsync(dto.OrdenId);
            if (orden == null) return false;

            if (orden.DiagnosticoTecnico != null)
            {
                orden.DiagnosticoTecnico.SolucionAplicada = dto.SolucionAplicada;
                orden.DiagnosticoTecnico.FechaFinalizacion = DateTime.UtcNow;
                orden.DiagnosticoTecnico.CostoTotalEstimado = dto.CostoFinal;
                _tecnicoRepo.UpdateDiagnostico(orden.DiagnosticoTecnico);
            }

            orden.EstadoOrden = EstadoOrden.Finalizado;
            orden.CostoTotal = dto.CostoFinal;
            orden.FechaCierre = DateTime.UtcNow;
            _tecnicoRepo.UpdateOrden(orden);

            await _tecnicoRepo.AddBitacoraAsync(new BitacoraTecnica
            {
                OrdenId = dto.OrdenId,
                Nota = $"Reparación finalizada con éxito. Solución: {dto.SolucionAplicada}. Costo total: {dto.CostoFinal:N2}.",
                FechaRegistro = DateTime.UtcNow
            });

            return await _tecnicoRepo.SaveChangesAsync();
        }
    }
}
