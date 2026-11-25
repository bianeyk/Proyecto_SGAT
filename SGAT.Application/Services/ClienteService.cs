using SGAT.Application.DTOs.Cliente;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SGAT.Application.Services
{
    public class ClienteService:IClienteService
    {
        private readonly IClienteRepository _clienteRepo;

        private static readonly Regex NombreValido =
            new(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-'.]+$", RegexOptions.Compiled);

        private static readonly Regex TelefonoValido =
            new(@"^[0-9]+$", RegexOptions.Compiled);

        private static readonly Regex EmailValido =
            new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                RegexOptions.Compiled);

        private static readonly Regex DireccionValida =
            new(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s#.,\/-]+$", RegexOptions.Compiled);

        public ClienteService(IClienteRepository clienteRepo)
        {
            _clienteRepo = clienteRepo;
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

        public async Task<PagedResult<ClienteDto>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            var result = await _clienteRepo.GetPageAsync(termino, pagina, tamanoPagina);
            return new PagedResult<ClienteDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                ActiveCount = result.ActiveCount,
                InactiveCount = result.InactiveCount
            };
        }

        public async Task<ClienteDto> CrearAsync(CrearClienteDto dto, string idUsuarioCrea)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(idUsuarioCrea))
                throw new ArgumentException("El usuario creador es obligatorio.", nameof(idUsuarioCrea));

            var datos = ValidarDatos(dto.Nombre, dto.Telefono, dto.Email, dto.Direccion);
            var numDocumento = NormalizarDocumento(dto.NumDocumento);

            if (await _clienteRepo.NumDocExisteAsync(numDocumento))
                throw new InvalidOperationException($"Ya existe un cliente con el documento {numDocumento}.");

            var cliente = new Cliente
            {
                IdUsuarioCrea = idUsuarioCrea,
                NumDoc = numDocumento,
                Nombre = datos.Nombre,
                Telefono = datos.Telefono,
                Email = datos.Email,
                Direccion = datos.Direccion,
                EsColaborador = dto.EsColaborador,
                Activo = dto.Activo
            };

            await _clienteRepo.AddAsync(cliente);
            try
            {
                await _clienteRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException(
                    $"Ya existe un cliente con el documento {numDocumento}.");
            }

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
            ArgumentNullException.ThrowIfNull(dto);
            var datos = ValidarDatos(dto.Nombre, dto.Telefono, dto.Email, dto.Direccion);

            var cliente = await _clienteRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Cliente {id} no encontrado.");

            cliente.Nombre = datos.Nombre;
            cliente.Telefono = datos.Telefono;
            cliente.Email = datos.Email;
            cliente.Direccion = datos.Direccion;
            cliente.EsColaborador = dto.EsColaborador;
            cliente.Activo = dto.Activo;
            cliente.UpdatedAt = DateTime.Now;

            _clienteRepo.Update(cliente);
            await _clienteRepo.SaveChangesAsync();
            return MapToDto(cliente);
        }

        private static (string Nombre, string Telefono, string? Email, string? Direccion)
            ValidarDatos(
                string? nombre,
                string? telefono,
                string? email,
                string? direccion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            var nombreLimpio = TextNormalization.CollapseWhitespace(nombre);
            if (nombreLimpio.Length > 250)
                throw new ArgumentException("El nombre no puede superar los 250 caracteres.");

            if (!NombreValido.IsMatch(nombreLimpio))
                throw new ArgumentException("El nombre contiene caracteres no permitidos.");

            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("El teléfono es obligatorio.");

            var telefonoLimpio = telefono.Trim();
            if (telefonoLimpio.Length > 20)
                throw new ArgumentException("El teléfono no puede superar los 20 caracteres.");

            if (!TelefonoValido.IsMatch(telefonoLimpio))
                throw new ArgumentException("El teléfono solo debe contener números.");

            var emailLimpio = TextNormalization.TrimOptional(email);
            if (emailLimpio is not null)
            {
                if (emailLimpio.Length > 150)
                    throw new ArgumentException("El correo no puede superar los 150 caracteres.");

                if (!EmailValido.IsMatch(emailLimpio))
                    throw new ArgumentException("El correo electrónico no tiene un formato válido.");
            }

            var direccionLimpia = TextNormalization.CollapseOptional(direccion);
            if (direccionLimpia is not null)
            {
                if (direccionLimpia.Length > 255)
                    throw new ArgumentException("La dirección no puede superar los 255 caracteres.");

                if (!DireccionValida.IsMatch(direccionLimpia))
                    throw new ArgumentException("La dirección contiene caracteres no permitidos.");
            }

            return (nombreLimpio, telefonoLimpio, emailLimpio, direccionLimpia);
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
