using SGAT.Application.DTOs.Cliente;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
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

        public async Task<ClienteDto> CrearAsync(CrearClienteDto dto, string idUsuarioCrea)
        {
            if (await _clienteRepo.NumDocExisteAsync(dto.NumDocumento))
                throw new InvalidOperationException($"Ya existe un cliente con el documento {dto.NumDocumento}.");

            var cliente = new Cliente
            {
                IdUsuario = idUsuarioCrea,
                NumDoc = dto.NumDocumento,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion
            };

            await _clienteRepo.AddAsync(cliente);
            await _clienteRepo.SaveChangesAsync();
            return MapToDto(cliente);
        }

        public async Task<ClienteDto> ActualizarAsync(int id, ActualizarClienteDto dto)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Cliente {id} no encontrado.");

            cliente.Nombre = dto.Nombre;
            cliente.Telefono = dto.Telefono;
            cliente.Email = dto.Email;
            cliente.Direccion = dto.Direccion;

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
            CreatedAt = c.CreatedAt
        };
    }
}
