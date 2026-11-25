using SGAT.Application.DTOs.Cliente;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGAT.Application.Services
{
    public class ClienteService:IClienteService
    {
        private readonly IClienteRepository _clienteRepo;

        public ClienteService(IClienteRepository clienteRepo)
        {
            _clienteRepo = clienteRepo;
        }

        public async Task<IEnumerable<ClienteDto>> GetTodosAsync()
        {
            var clientes = await _clienteRepo.GetAllAsync();
            return clientes.Select(MapToDto);
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id);
            return cliente is null ? null : MapToDto(cliente);
        }

        public async Task<ClienteDto?> GetByNumDocAsync(string numDoc)
        {
            var cliente = await _clienteRepo.GetByNumDocAsync(numDoc);
            return cliente is null ? null : MapToDto(cliente);
        }

        public async Task<IEnumerable<ClienteDto>> BuscarAsync(string termino)
        {
            var clientes = await _clienteRepo.BuscarAsync(termino);
            return clientes.Select(MapToDto);
        }

        public async Task<PagedResult<ClienteDto>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            var result = await _clienteRepo.GetPageAsync(termino, pagina, tamanoPagina);
            return new PagedResult<ClienteDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount
            };
        }

        public async Task<ClienteDto> CrearAsync(CrearClienteDto dto, string idUsuarioCrea)
        {
            var numDocumento = NormalizarDocumento(dto.NumDocumento);

            if (await _clienteRepo.NumDocExisteAsync(numDocumento))
                throw new InvalidOperationException($"Ya existe un cliente con el documento {numDocumento}.");

            var cliente = new Cliente
            {
                IdUsuarioCrea = idUsuarioCrea,
                NumDoc = numDocumento,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                EsColaborador = dto.EsColaborador,
                Activo = dto.Activo
            };

            await _clienteRepo.AddAsync(cliente);
            await _clienteRepo.SaveChangesAsync();
            return MapToDto(cliente);
        }

        private static string NormalizarDocumento(string? documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("El número de documento es obligatorio.");

            var limpio = documento.Trim();

            if (limpio.Length > 15)
                throw new ArgumentException("El documento no puede superar los 15 caracteres.");

            if (!limpio.All(char.IsDigit))
                throw new ArgumentException("El documento solo debe contener números.");

            return limpio;
        }

        public async Task<ClienteDto> ActualizarAsync(int id, ActualizarClienteDto dto)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Cliente {id} no encontrado.");

            cliente.Nombre = dto.Nombre;
            cliente.Telefono = dto.Telefono;
            cliente.Email = dto.Email;
            cliente.Direccion = dto.Direccion;
            cliente.EsColaborador = dto.EsColaborador;
            cliente.Activo = dto.Activo;
            cliente.UpdatedAt = DateTime.Now;

            _clienteRepo.Update(cliente);
            await _clienteRepo.SaveChangesAsync();
            return MapToDto(cliente);
        }

        // ── Mapper interno ──────────────────────────────────────────
        private static ClienteDto MapToDto(Cliente c) => new()
        {
            IdCliente = c.IdCliente,
            NumDocumento = c.NumDoc,
            Nombre = c.Nombre,
            Telefono = c.Telefono,
            Email = c.Email,
            Direccion = c.Direccion,
            EsColaborador = c.EsColaborador,
            Activo = c.Activo,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}
