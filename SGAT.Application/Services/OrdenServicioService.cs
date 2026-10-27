//using SGAT.Application.DTOs.OrdenServicio;
//using SGAT.Application.Interfaces.Repositories;
//using SGAT.Application.Interfaces.Services;
//using SGAT.Domain.Entities;
//using SGAT.SGAT.Domain.Enums;

//namespace SGAT.Application.Services;

//public class OrdenServicioService : IOrdenServicioService
//{
//    private readonly IOrdenServicioRepository _ordenRepo;

//    public OrdenServicioService(IOrdenServicioRepository ordenRepo)
//    {
//        _ordenRepo = ordenRepo;
//    }

//    public async Task<IEnumerable<OrdenServicioDto>> GetTodasAsync()
//    {
//        var ordenes = await _ordenRepo.GetAllAsync();
//        return ordenes.Select(MapToDto);
//    }

//    public async Task<OrdenServicioDto?> GetByIdAsync(int id)
//    {
//        var orden = await _ordenRepo.GetConDetallesAsync(id);
//        return orden is null ? null : MapToDto(orden);
//    }

//    public async Task<IEnumerable<OrdenServicioDto>> GetByTecnicoAsync(string idUsuario)
//    {
//        var ordenes = await _ordenRepo.GetByTecnicoAsync(idUsuario);
//        return ordenes.Select(MapToDto);
//    }
//    public async Task<OrdenServicioDto> CrearAsync(CrearOrdenServicioDto dto, string idUsuarioCrea)
//    {
//        var codigo = await _ordenRepo.GenerarCodigoAsync();

//        var orden = new OrdenServicio
//        {
//            IdEquipo = dto.IdEquipo,
//            IdUsuarioCrea = idUsuarioCrea,
//            IdEstadoServicio = 1, // RECIBIDO por defecto
//            Codigo = codigo,
//            Diagnostico = dto.Diagnostico,
//            Prioridad = Enum.Parse<PrioridadEnum>(dto.Prioridad!),
//            FechaPrometida = dto.FechaPrometida,
//            Observaciones = dto.Observaciones
//        };

//        await _ordenRepo.AddAsync(orden);
//        await _ordenRepo.SaveChangesAsync();
//        return MapToDto(orden);
//    }

//    public async Task<bool> CambiarEstadoAsync(int id, CambiarEstadoDto dto, string idUsuario)
//    {
//        var orden = await _ordenRepo.GetByIdAsync(id)
//            ?? throw new KeyNotFoundException($"Orden {id} no encontrada.");

//        orden.IdEstadoServicio = dto.IdEstadoServicio;

//        if (dto.IdEstadoServicio == 5) // ENTREGADO
//            orden.FechaCierre = DateTime.Now;

//        _ordenRepo.Update(orden);
//        return await _ordenRepo.SaveChangesAsync();
//    }

//    private static OrdenServicioDto MapToDto(OrdenServicio o) => new()
//    {
//        IdOrdenServicio = o.IdOrdenServicio,
//        Codigo = o.Codigo,
//        Estado = o.EstadoServicio?.Nombre ?? string.Empty,
//        Prioridad = o.Prioridad.ToString(),
//        CostoTotal = o.CostoTotal,
//        Abono = o.Abono,
//        FechaIngreso = o.FechaIngreso,
//        FechaPrometida = o.FechaPrometida,
//        Diagnostico = o.Diagnostico,
//        Cliente = o.Equipo?.Cliente?.Nombre ?? string.Empty,
//        Equipo = o.Equipo != null ? $"{o.Equipo.Marca?.Nombre} {o.Equipo.Modelo}" : string.Empty,
//        TecnicoAsignado = o.UsuarioCrea?.NombreCompleto ?? string.Empty
//    };
//}