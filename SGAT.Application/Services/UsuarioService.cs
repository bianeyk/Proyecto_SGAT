using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SGAT.Application.DTOs.Usuario;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Transactions;
using System.Data;

//Inyección de Dependencias
public class UsuarioService : IUsuarioService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<ApplicationRole> _roleManager;

    private readonly ILogger<UsuarioService> _logger;//registra eventos y errores
    private readonly ITransactionManager _transactionManager;
    private readonly IAppLockManager _appLockManager;

    public UsuarioService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UsuarioService> logger,
        ITransactionManager transactionManager,
        IAppLockManager appLockManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _transactionManager = transactionManager;
        _appLockManager = appLockManager;
    }
    private const string RecursoBloqueoAdministradores = "SGAT_GESTION_ADMINISTRADORES";
    //METODOS
    public async Task<IReadOnlyList<UsuarioListDto>> ObtenerTodosAsync()
    {
        //Obtendremos todos los usuarios
        var usuarios = await _userManager.Users.ToListAsync();
        //Creamos la lista que devolveremos
        var lista = new List<UsuarioListDto>();
        //Recorremos cada usuario
        foreach (var usuario in usuarios)
        {
            //Obtener los roles
            var roles = await _userManager.GetRolesAsync(usuario);
            //Crear el DTO manualmente
            var dto = new UsuarioListDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email!,
                Activo = usuario.Activo,
                Roles = roles.ToList()
            };
            //Agregar el DTO
            lista.Add(dto);
        }

        return lista;
    }

    public async Task<UsuarioDetalleDto?> ObtenerPorIdAsync(string id)
    {
        //Buscar el usuario
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario == null)
        {
            return null;
        }
        //Obtener los roles
        var roles = await _userManager.GetRolesAsync(usuario);
        //Crear el DTO
        var dto = new UsuarioDetalleDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email!,
            Activo = usuario.Activo,
            CreatedAt = usuario.CreatedAt,
            LastLoginAt = usuario.LastLoginAt,
            Roles = roles.ToList()
        };
        return dto;
    }


    public async Task<IdentityResult> CrearAsync(UsuarioCreateDto dto)
    {
        // 1. Validar que el DTO exista.
        if (dto == null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "DatosInvalidos",
                    Description = "Los datos del usuario son obligatorios."
                });
        }

        // 2. Validar los datos obligatorios.
        if (string.IsNullOrWhiteSpace(dto.NombreCompleto) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password) ||
            string.IsNullOrWhiteSpace(dto.Rol))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "DatosIncompletos",
                    Description = "Debe completar todos los campos obligatorios."
                });
        }

        // 3. Normalizar los datos.
        var nombreCompleto = dto.NombreCompleto.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        var nombreRol = dto.Rol.Trim();

        // 4. Iniciar la transacción.
        await using var transaction =
            await _transactionManager.BeginTransactionAsync();

        try
        {
            // 5. Adquirir el bloqueo exclusivo.
            await _appLockManager.AcquireLockAsync(
                RecursoBloqueoAdministradores);

            // 6. Verificar si el correo ya existe.
            var usuarioExistente =
                await _userManager.FindByEmailAsync(email);

            if (usuarioExistente != null)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "EmailDuplicado",
                        Description = "Ya existe un usuario registrado con este correo electrónico."
                    });
            }

            // 7. Buscar el rol.
            var rol = await _roleManager.FindByNameAsync(nombreRol);

            if (rol == null)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolNoExiste",
                        Description = $"El rol '{nombreRol}' no existe."
                    });
            }

            // 8. Verificar que el rol esté activo.
            if (!rol.Activo)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolInactivo",
                        Description = "No se puede asignar un rol que está inactivo."
                    });
            }

            // 9. Construir la entidad manualmente.
            var usuario = new ApplicationUser
            {
                NombreCompleto = nombreCompleto,
                Email = email,
                UserName = email,
                Activo = true
            };

            // 10. Crear el usuario mediante Identity.
            var resultadoUsuario = await _userManager.CreateAsync(
                usuario,
                dto.Password);

            if (!resultadoUsuario.Succeeded)
            {
                await transaction.RollbackAsync();
                return resultadoUsuario;
            }

            // 11. Asignar un único rol.
            var resultadoRol = await _userManager.AddToRoleAsync(
                usuario,
                nombreRol);

            if (!resultadoRol.Succeeded)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    "No se pudo asignar el rol {Rol} al usuario {Email}.",
                    nombreRol,
                    usuario.Email);

                return resultadoRol;
            }

            // 12. Confirmar la transacción.
            await transaction.CommitAsync();

            // 13. Registrar la operación.
            _logger.LogInformation(
                "Usuario {Email} creado correctamente con el rol {Rol}.",
                usuario.Email,
                nombreRol);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(
                ex,
                "Error al crear el usuario con correo {Email}.",
                email);

            throw;
        }
    }

    public async Task<IdentityResult> ActualizarAsync(UsuarioEditDto dto)
    {
        // Validaciones iniciales
        if (dto == null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "DatosInvalidos",
                    Description = "Los datos del usuario son obligatorios."
                });
        }
        // Validar que se haya seleccionado un rol
        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "IdUsuarioInvalido",
                    Description = "El identificador del usuario no es válido."
                });
        }
        // Validar que se haya ingresado un nombre completo
        if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "NombreRequerido",
                    Description = "El nombre completo es obligatorio."
                });
        }
        //inicio de transacción para proteger la operación de actualización de usuario y roles
        await using var transaction =
       await _transactionManager.BeginTransactionAsync();

        try
        {
            
            // Obtener el bloqueo exclusivo para gestionar administradores.
            await _appLockManager.AcquireLockAsync(RecursoBloqueoAdministradores);
            // 1. Buscar el usuario
            var usuario = await _userManager.FindByIdAsync(dto.Id);

            if (usuario == null)
            {
                //transacción iniciada, si ocurre un error se hace rollback
                await transaction.RollbackAsync();
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "UsuarioNoExiste",
                        Description = "El usuario no fue encontrado."
                    });
            }

            // 2. Validar el rol seleccionado
            var nombreRol = dto.Rol?.Trim();

            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                //transacción iniciada, si ocurre un error se hace rollback
                await transaction.RollbackAsync();
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolRequerido",
                        Description = "Debe seleccionar un rol."
                    });
            }

            // 3. Buscar el rol
            var rol = await _roleManager.FindByNameAsync(nombreRol);

            if (rol == null)
            {
                //transacción iniciada, si ocurre un error se hace rollback
                await transaction.RollbackAsync();
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolNoExiste",
                        Description = $"El rol '{nombreRol}' no existe."
                    });
            }

            // 4. Validar que el rol esté activo
            if (!rol.Activo)
            {
                //transacción iniciada, si ocurre un error se hace rollback
                await transaction.RollbackAsync();
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolInactivo",
                        Description = "No se puede asignar un rol que está inactivo."
                    });
            }

            // 5. Obtener los roles actuales
            var rolesActuales = await _userManager.GetRolesAsync(usuario);

            // 6. Proteger al último Administrador activo
            var esAdministradorActual =
                usuario.Activo &&
                rolesActuales.Contains(
                    "Administrador",
                    StringComparer.OrdinalIgnoreCase);

            var pierdeCondicionAdministrador =
                !dto.Activo ||
                !string.Equals(
                    nombreRol,
                    "Administrador",
                    StringComparison.OrdinalIgnoreCase);

            if (esAdministradorActual && pierdeCondicionAdministrador)
            {
                var administradores = await _userManager
                    .GetUsersInRoleAsync("Administrador");

                var administradoresActivos = administradores
                    .Count(a => a.Activo);

                if (administradoresActivos <= 1)
                {
                    await transaction.RollbackAsync();
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "UltimoAdministrador",
                            Description = "No se puede quitar el rol o inactivar al último administrador activo."
                        });
                }
            }

            // 7. Detectar si cambia el rol
            var tieneRolSeleccionado = rolesActuales.Any(r =>
                string.Equals(
                    r,
                    nombreRol,
                    StringComparison.OrdinalIgnoreCase));

            var rolesAQuitar = rolesActuales
                .Where(r => !string.Equals(
                    r,
                    nombreRol,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            var cambioRol = rolesAQuitar.Any() || !tieneRolSeleccionado;


            // 9. Actualizar datos del usuario
            usuario.NombreCompleto = dto.NombreCompleto.Trim();
            var cambioEstadoActivo = usuario.Activo != dto.Activo;
            usuario.Activo = dto.Activo;

            var resultadoUsuario = await _userManager.UpdateAsync(usuario);

            if (!resultadoUsuario.Succeeded)
            {
                await transaction.RollbackAsync();
                return resultadoUsuario;
            }

            // 10. Quitar roles anteriores
            if (rolesAQuitar.Any())
            {
                var resultadoQuitar = await _userManager.RemoveFromRolesAsync(
                    usuario,
                    rolesAQuitar);

                if (!resultadoQuitar.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoQuitar;
                }
            }

            // 11. Asignar el rol seleccionado
            if (!tieneRolSeleccionado)
            {
                var resultadoAgregar = await _userManager.AddToRoleAsync(
                    usuario,
                    nombreRol);

                if (!resultadoAgregar.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoAgregar;
                }
            }

            // 12. Actualizar Security Stamp si cambió el rol
            if (cambioRol || cambioEstadoActivo)
            {
                var resultadoStamp = await _userManager
                    .UpdateSecurityStampAsync(usuario);

                if (!resultadoStamp.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return resultadoStamp;
                }
            }

            // 13. Confirmar todos los cambios
            await transaction.CommitAsync();

            // 14. Registrar la operación
            _logger.LogInformation(
                "Usuario {Email} actualizado correctamente con el rol {Rol}.",
                usuario.Email,
                nombreRol);

            return IdentityResult.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }




    public async Task<IdentityResult> ActivarAsync(string id)
    {
        // 1. Validar el identificador.
        if (string.IsNullOrWhiteSpace(id))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "IdUsuarioInvalido",
                    Description = "El identificador del usuario no es válido."
                });
        }

        // 2. Iniciar la transacción.
        await using var transaction =
            await _transactionManager.BeginTransactionAsync();

        try
        {
            // 3. Adquirir el bloqueo exclusivo.
            await _appLockManager.AcquireLockAsync(
                RecursoBloqueoAdministradores);

            // 4. Buscar el usuario.
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "UsuarioNoExiste",
                        Description = "El usuario no fue encontrado."
                    });
            }

            // 5. Comprobar si el usuario ya está activo.
            if (usuario.Activo)
            {
                await transaction.CommitAsync();
                return IdentityResult.Success;
            }

            // 6. Obtener los roles actuales.
            var rolesActuales = await _userManager.GetRolesAsync(usuario);

            // 7. Comprobar que tenga exactamente un rol.
            if (rolesActuales.Count != 1)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "CantidadRolesInvalida",
                        Description = "El usuario debe tener exactamente un rol asignado para poder activarse."
                    });
            }

            // 8. Buscar el rol asignado.
            var nombreRol = rolesActuales.Single();

            var rol = await _roleManager.FindByNameAsync(nombreRol);

            if (rol == null)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolNoExiste",
                        Description = "El rol asignado al usuario no existe."
                    });
            }

            // 9. Comprobar que el rol esté activo.
            if (!rol.Activo)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolInactivo",
                        Description = "No se puede activar un usuario cuyo rol está inactivo."
                    });
            }

            // 10. Activar el usuario.
            usuario.Activo = true;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                await transaction.RollbackAsync();
                return resultado;
            }

            // 11. Actualizar el Security Stamp.
            var resultadoStamp =
                await _userManager.UpdateSecurityStampAsync(usuario);

            if (!resultadoStamp.Succeeded)
            {
                await transaction.RollbackAsync();
                return resultadoStamp;
            }

            // 12. Confirmar la transacción.
            await transaction.CommitAsync();

            // 13. Registrar la operación.
            _logger.LogInformation(
                "Usuario {Email} activado correctamente.",
                usuario.Email);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(
                ex,
                "Error al activar el usuario con identificador {UsuarioId}.",
                id);

            throw;
        }
    }


    public async Task<IdentityResult> InactivarAsync(string id)
    {
        // 1. Validar el identificador.
        if (string.IsNullOrWhiteSpace(id))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "IdUsuarioInvalido",
                    Description = "El identificador del usuario no es válido."
                });
        }

        // 2. Iniciar la transacción.
        await using var transaction =
            await _transactionManager.BeginTransactionAsync();

        try
        {
            // 3. Adquirir el bloqueo exclusivo.
            await _appLockManager.AcquireLockAsync(
                RecursoBloqueoAdministradores);

            // 4. Buscar el usuario.
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "UsuarioNoExiste",
                        Description = "El usuario no fue encontrado."
                    });
            }

            // 5. Comprobar si ya está inactivo.
            if (!usuario.Activo)
            {
                await transaction.CommitAsync();
                return IdentityResult.Success;
            }

            // 6. Obtener los roles actuales.
            var rolesActuales =
                await _userManager.GetRolesAsync(usuario);

            // 7. Comprobar que tenga exactamente un rol.
            if (rolesActuales.Count != 1)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "CantidadRolesInvalida",
                        Description = "El usuario debe tener exactamente un rol asignado para poder inactivarse."
                    });
            }

            // 8. Buscar el rol asignado.
            var nombreRol = rolesActuales.Single();

            var rol = await _roleManager.FindByNameAsync(nombreRol);

            if (rol == null)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolNoExiste",
                        Description = "El rol asignado al usuario no existe."
                    });
            }

            // 9. Comprobar que el rol esté activo.
            if (!rol.Activo)
            {
                await transaction.RollbackAsync();

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolInactivo",
                        Description = "No se puede inactivar un usuario cuyo rol está inactivo."
                    });
            }

            // 10. Proteger al último administrador activo.
            if (string.Equals(
                nombreRol,
                "Administrador",
                StringComparison.OrdinalIgnoreCase))
            {
                var administradores = await _userManager
                    .GetUsersInRoleAsync("Administrador");

                var administradoresActivos = administradores
                    .Count(a => a.Activo);

                if (administradoresActivos <= 1)
                {
                    await transaction.RollbackAsync();

                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "UltimoAdministrador",
                            Description = "No se puede inactivar al último administrador activo."
                        });
                }
            }

            // 11. Inactivar el usuario.
            usuario.Activo = false;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                await transaction.RollbackAsync();
                return resultado;
            }

            // 12. Actualizar el Security Stamp.
            var resultadoStamp =
                await _userManager.UpdateSecurityStampAsync(usuario);

            if (!resultadoStamp.Succeeded)
            {
                await transaction.RollbackAsync();
                return resultadoStamp;
            }

            // 13. Confirmar la transacción.
            await transaction.CommitAsync();

            // 14. Registrar la operación.
            _logger.LogInformation(
                "Usuario {Email} inactivado correctamente.",
                usuario.Email);

            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(
                ex,
                "Error al inactivar el usuario con identificador {UsuarioId}.",
                id);

            throw;
        }
    }

}