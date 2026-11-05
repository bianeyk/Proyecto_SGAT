using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SGAT.Application.DTOs.Usuario;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

//Inyección de Dependencias
public class UsuarioService : IUsuarioService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<ApplicationRole> _roleManager;

    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UsuarioService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }
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
        // 1. Validar si el correo ya existe
        var usuarioExistente = await _userManager.FindByEmailAsync(dto.Email);

        if (usuarioExistente != null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "EmailDuplicado",
                    Description = "Ya existe un usuario registrado con este correo electrónico."
                });
        }

        // 2. Validar que se haya seleccionado un rol
        var nombreRol = dto.Rol?.Trim();

        if (string.IsNullOrWhiteSpace(nombreRol))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "RolRequerido",
                    Description = "Debe seleccionar un rol para el usuario."
                });
        }

        // 3. Buscar el rol en Identity
        var rol = await _roleManager.FindByNameAsync(nombreRol);

        if (rol == null)
        {
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
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "RolInactivo",
                    Description = "No se puede asignar un rol que está inactivo."
                });
        }

        // 5. Crear el usuario
        var usuario = new ApplicationUser
        {
            NombreCompleto = dto.NombreCompleto.Trim(),
            Email = dto.Email.Trim().ToLower(),
            UserName = dto.Email.Trim().ToLower(),
            Activo = dto.Activo
        };

        var resultado = await _userManager.CreateAsync(
            usuario,
            dto.Password);

        if (!resultado.Succeeded)
        {
            return resultado;
        }

        // 6. Asignar un único rol
        var resultadoRol = await _userManager.AddToRoleAsync(
            usuario,
            nombreRol);

        if (!resultadoRol.Succeeded)
        {
            // Si falla la asignación, dejamos el usuario inactivo.
            // No eliminamos físicamente el registro.
            usuario.Activo = false;

            await _userManager.UpdateAsync(usuario);

            _logger.LogError(
                "No se pudo asignar el rol {Rol} al usuario {Email}.",
                nombreRol,
                usuario.Email);

            return resultadoRol;
        }

        // 7. Registrar la operación
        _logger.LogInformation(
            "Usuario {Email} creado correctamente con el rol {Rol}.",
            usuario.Email,
            nombreRol);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ActualizarAsync(UsuarioEditDto dto)
    {
        // 1. Buscar el usuario
        var usuario = await _userManager.FindByIdAsync(dto.Id);

        if (usuario == null)
        {
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
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "RolInactivo",
                    Description = "No se puede asignar un rol que está inactivo."
                });
        }

        // 5. Actualizar los datos del usuario
        usuario.NombreCompleto = dto.NombreCompleto.Trim();
        usuario.Activo = dto.Activo;

        var resultadoUsuario = await _userManager.UpdateAsync(usuario);

        if (!resultadoUsuario.Succeeded)
        {
            return resultadoUsuario;
        }

        // 6. Obtener los roles actuales
        var rolesActuales = await _userManager.GetRolesAsync(usuario);

        // 7. Quitar los roles que no corresponden al seleccionado
        var rolesAQuitar = rolesActuales
            .Where(r => !string.Equals(
                r,
                nombreRol,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (rolesAQuitar.Any())
        {
            var resultadoQuitar = await _userManager.RemoveFromRolesAsync(
                usuario,
                rolesAQuitar);

            if (!resultadoQuitar.Succeeded)
            {
                return resultadoQuitar;
            }
        }

        // 8. Comprobar si ya tiene el rol seleccionado
        var tieneRolSeleccionado = rolesActuales.Any(r =>
            string.Equals(
                r,
                nombreRol,
                StringComparison.OrdinalIgnoreCase));

        // 9. Asignar el rol si todavía no lo tiene
        if (!tieneRolSeleccionado)
        {
            var resultadoAgregar = await _userManager.AddToRoleAsync(
                usuario,
                nombreRol);

            if (!resultadoAgregar.Succeeded)
            {
                return resultadoAgregar;
            }
        }

        // 10. Registrar la operación
        _logger.LogInformation(
            "Usuario {Email} actualizado correctamente con el rol {Rol}.",
            usuario.Email,
            nombreRol);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ActivarAsync(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario == null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "UsuarioNoExiste",
                    Description = "El usuario no fue encontrado."
                });
        }

        if (usuario.Activo)
        {
            return IdentityResult.Success;
        }

        usuario.Activo = true;

        var resultado = await _userManager.UpdateAsync(usuario);

        if (!resultado.Succeeded)
        {
            return resultado;
        }

        _logger.LogInformation(
            "Usuario {Email} activado correctamente.",
            usuario.Email);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> InactivarAsync(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);

        if (usuario == null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "UsuarioNoExiste",
                    Description = "El usuario no fue encontrado."
                });
        }

        if (!usuario.Activo)
        {
            return IdentityResult.Success;
        }

        usuario.Activo = false;

        var resultado = await _userManager.UpdateAsync(usuario);

        if (!resultado.Succeeded)
        {
            return resultado;
        }

        _logger.LogInformation(
            "Usuario {Email} inactivado correctamente.",
            usuario.Email);

        return IdentityResult.Success;
    }
}