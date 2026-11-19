
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGAT.Application.DTOs.Rol;
using SGAT.Application.Interfaces.Services;
using SGAT.Application.Interfaces.Transactions;
using SGAT.Domain.Entities;

namespace SGAT.Application.Services
{
    public class RolService : IRolService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITransactionManager _transactionManager;
        private readonly IAppLockManager _appLockManager;
        private readonly ILogger<RolService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        private const string RecursoBloqueoAdministradores =
            "SGAT_GESTION_ADMINISTRADORES";

        public RolService(
            RoleManager<ApplicationRole> roleManager,
            ITransactionManager transactionManager,
            IAppLockManager appLockManager,
            ILogger<RolService> logger,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _transactionManager = transactionManager;
            _appLockManager = appLockManager;
            _logger = logger;
            _userManager = userManager;
        }

        // 1. Obtener todos los roles.
        public async Task<IReadOnlyList<RolDto>> ObtenerTodosAsync()
        {
            var lista = await _roleManager.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new RolDto
                {
                    Id = r.Id,
                    Nombre = r.Name!,
                    Activo = r.Activo
                })
                .ToListAsync();

            return lista;
        }

        // 2. Crear un rol.
        public async Task<IdentityResult> CrearAsync(RolCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "NombreRolRequerido",
                        Description = "El nombre del rol es obligatorio."
                    });
            }

            // Limpiar espacios.
            var nombre = dto.Nombre.Trim();

            // Validar longitud.
            if (nombre.Length > 50)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "NombreRolMuyLargo",
                        Description = "El nombre del rol no puede superar los 50 caracteres."
                    });
            }

            // Proteger los nombres reservados.
            var rolesReservados = new[]
            {
                "Administrador",
                "Gerente",
                "Tecnico"
            };

            if (rolesReservados.Contains(
                nombre,
                StringComparer.OrdinalIgnoreCase))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "NombreRolReservado",
                        Description = "Este nombre está reservado para un rol del sistema."
                    });
            }

            await using var transaction =
                await _transactionManager.BeginTransactionAsync();

            try
            {
                // Adquirir el bloqueo exclusivo.
                await _appLockManager.AcquireLockAsync(
                    RecursoBloqueoAdministradores);

                // Comprobar si el rol ya existe.
                var rolExistente =
                    await _roleManager.FindByNameAsync(nombre);

                if (rolExistente != null)
                {
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "RolDuplicado",
                            Description = "Ya existe un rol con ese nombre."
                        });
                }

                // Crear el nuevo rol.
                var nuevoRol = new ApplicationRole
                {
                    Name = nombre,
                    Activo = true
                };

                var resultado =
                    await _roleManager.CreateAsync(nuevoRol);

                if (!resultado.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultado;
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Se creó correctamente el rol {NombreRol}.",
                    nombre);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error al crear el rol {NombreRol}.",
                    nombre);

                throw;
            }
        }

        // 3. Activar un rol.
        public async Task<IdentityResult> ActivarAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "IdRolInvalido",
                        Description = "El identificador del rol no es válido."
                    });
            }

            await using var transaction =
                await _transactionManager.BeginTransactionAsync();

            try
            {
                // Adquirir el bloqueo exclusivo.
                await _appLockManager.AcquireLockAsync(
                    RecursoBloqueoAdministradores);

                // Buscar el rol.
                var rol = await _roleManager.FindByIdAsync(id);

                if (rol == null)
                {
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "RolNoExiste",
                            Description = "El rol no existe."
                        });
                }

                // Comprobar si ya está activo.
                if (rol.Activo)
                {
                    await transaction.CommitAsync();
                    return IdentityResult.Success;
                }

                // Activar el rol.
                rol.Activo = true;

                var resultado = await _roleManager.UpdateAsync(rol);

                if (!resultado.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultado;
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "El rol {NombreRol} fue activado correctamente.",
                    rol.Name);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error al activar el rol con identificador {RolId}.",
                    id);

                throw;
            }
        }

        // 4. Inactivar un rol.
        public async Task<IdentityResult> InactivarAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "IdRolInvalido",
                        Description = "El identificador del rol no es válido."
                    });
            }

            await using var transaction =
                await _transactionManager.BeginTransactionAsync();

            try
            {
                // Adquirir el bloqueo exclusivo.
                await _appLockManager.AcquireLockAsync(
                    RecursoBloqueoAdministradores);

                // Buscar el rol.
                var rol = await _roleManager.FindByIdAsync(id);

                if (rol == null)
                {
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "RolNoExiste",
                            Description = "El rol no existe."
                        });
                }

                // Proteger el rol Administrador.
                if (string.Equals(
                    rol.Name,
                    "Administrador",
                    StringComparison.OrdinalIgnoreCase))
                {
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "RolProtegido",
                            Description = "El rol Administrador no puede inactivarse."
                        });
                }

                // Comprobar si existen usuarios asignados.
                var usuariosAsignados =
                    await _userManager.GetUsersInRoleAsync(rol.Name!);

                if (usuariosAsignados.Any())
                {
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "RolConUsuarios",
                            Description = "No se puede inactivar el rol porque tiene usuarios asignados."
                        });
                }

                // Comprobar si ya está inactivo.
                if (!rol.Activo)
                {
                    await transaction.CommitAsync();
                    return IdentityResult.Success;
                }

                // Inactivar el rol.
                rol.Activo = false;

                var resultado = await _roleManager.UpdateAsync(rol);

                if (!resultado.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultado;
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "El rol {NombreRol} fue inactivado correctamente.",
                    rol.Name);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error al inactivar el rol con identificador {RolId}.",
                    id);

                throw;
            }
        }
    }
}
