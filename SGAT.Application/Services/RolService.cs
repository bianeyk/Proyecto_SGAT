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
        //constante para el recurso de bloqueo de administradores
        private const string RecursoBloqueoAdministradores = "SGAT_GESTION_ADMINISTRADORES";
        public async Task<IReadOnlyList<RolDto>> ObtenerTodosAsync()
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

            var lista = new List<RolDto>();

            foreach (var rol in roles)
            {
                lista.Add(new RolDto
                {
                    Id = rol.Id,
                    Nombre = rol.Name!,
                    Activo = rol.Activo
                });
            }

            return lista;
        }

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

            // 1. Limpiar espacios al inicio y al final.
            var nombre = dto.Nombre.Trim();

            // 2. Validar longitud.
            if (nombre.Length > 50)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "NombreRolMuyLargo",
                        Description = "El nombre del rol no puede superar los 50 caracteres."
                    });
            }

            // 3. Proteger los nombres reservados.
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
                // 4. Adquirir el bloqueo compartido.
                await _appLockManager.AcquireLockAsync(
                    RecursoBloqueoAdministradores);

                // 5. Comprobar si el rol ya existe.
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

                // 6. Crear el nuevo rol.
                var nuevoRol = new ApplicationRole
                {
                    Name = nombre,
                    Activo = true
                };

                var resultado = await _roleManager.CreateAsync(nuevoRol);

                if (!resultado.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultado;
                }

                // 7. Confirmar la transacción.
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

        public async Task<IdentityResult> ActivarAsync(string id)
        {
            // 1. Validar el identificador antes de iniciar la transacción.
            if (string.IsNullOrWhiteSpace(id))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "IdRolInvalido",
                        Description = "El identificador del rol no es válido."
                    });
            }

            // 2. Iniciar la transacción.
            await using var transaction =
                await _transactionManager.BeginTransactionAsync();

            try
            {
                // 3. Adquirir el bloqueo compartido.
                await _appLockManager.AcquireLockAsync(
                    RecursoBloqueoAdministradores);

                // 4. Buscar el rol.
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

                // 5. Comprobar si ya está activo.
                if (rol.Activo)
                {
                    await transaction.CommitAsync();
                    return IdentityResult.Success;
                }

                // 6. Activar el rol.
                rol.Activo = true;

                var resultado = await _roleManager.UpdateAsync(rol);

                if (!resultado.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultado;
                }

                // 7. Confirmar la transacción.
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



        public async Task<IdentityResult> InactivarAsync(string id)
        {
            // 1. Validar el identificador antes de iniciar la transacción.
            if (string.IsNullOrWhiteSpace(id))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "IdRolInvalido",
                        Description = "El identificador del rol no es válido."
                    });
            }

            // 2. Iniciar la transacción.
            await using var transaction =
                await _transactionManager.BeginTransactionAsync();

            try
            {
                // 3. Adquirir el bloqueo compartido.
                await _appLockManager.AcquireLockAsync(
                    RecursoBloqueoAdministradores);

                // 4. Buscar el rol.
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

                // 5. Proteger el rol Administrador.
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

                // 6. Comprobar si existen usuarios asignados.
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

                // 7. Si ya está inactivo, no es necesario modificarlo.
                if (!rol.Activo)
                {
                    await transaction.CommitAsync();
                    return IdentityResult.Success;
                }

                // 8. Cambiar el estado del rol.
                rol.Activo = false;

                var resultado = await _roleManager.UpdateAsync(rol);

                if (!resultado.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultado;
                }

                // 9. Confirmar la transacción.
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
