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
        //Consultar si existe un correo.
        var usuarioExistente = await _userManager.FindByEmailAsync(dto.Email);

        if (usuarioExistente != null)
        {
            var error = new IdentityError
            {
                Code = "EmailDuplicado",
                Description = "Ya existe un usuario registrado con este correo electrónico."
            };

            return IdentityResult.Failed(error);
        }

        //validar administrador seleccione al menos un rol al crear un usuario
        if (dto.Roles == null || !dto.Roles.Any())
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "RolRequerido",
                    Description = "Debe seleccionar al menos un rol para el usuario."
                });
        }

        //valisa que el rol exista
        if (dto.Roles.Any())
        {
            foreach (var rol in dto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(rol))
                {
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "RolNoExiste",
                            Description = $"El rol '{rol}' no existe."
                        });
                }
            }
        }
        //Luego de las validaciones Crear un objeto en memoria Crear el objeto usuario
        var usuario = new ApplicationUser
        {
            NombreCompleto = dto.NombreCompleto.Trim(),
            Email = dto.Email.Trim().ToLower(),
            UserName = dto.Email.Trim().ToLower(),
            Activo = dto.Activo
        };
        // Crear el usuario en Identity
        var resultado = await _userManager.CreateAsync(usuario, dto.Password);

        // Si hubo errores, devolverlos
        if (!resultado.Succeeded)
        {
            return resultado;
        }

        // Asignar los roles seleccionados
        var resultadoRoles = await _userManager.AddToRolesAsync(usuario, dto.Roles);

        if (!resultadoRoles.Succeeded)
        {
            // Aquí hablaremos de esto enseguida
            return resultadoRoles;
        }
        //ILogger ensajes se guarden
        _logger.LogInformation("Usuario {Email} creado correctamente con los roles: {Roles}", usuario.Email,string.Join(", ", dto.Roles));

        return IdentityResult.Success;
        // Continuaremos aquí...
    }

    public async Task<IdentityResult> ActualizarAsync(UsuarioEditDto dto)
    {
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
        //validar roles
        if (dto.Roles == null || !dto.Roles.Any())
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "RolRequerido",
                    Description = "Debe seleccionar al menos un rol."
                });
        }

        foreach (var rol in dto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(rol))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "RolNoExiste",
                        Description = $"El rol '{rol}' no existe."
                    });
            }
        }
        //Actualizamos los datos del usuario
        usuario.NombreCompleto = dto.NombreCompleto.Trim();
        usuario.Activo = dto.Activo;
        //Guardamos cambios UserManager se encarga de actualizar el registro de AspNetUsers
        var resultado = await _userManager.UpdateAsync(usuario);

        if (!resultado.Succeeded)
        {
            return resultado;
        }
        // Obtener los roles actuales del usuario
        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        var rolesAQuitar = rolesActuales.Except(dto.Roles).ToList();
        var rolesAAgregar = dto.Roles.Except(rolesActuales).ToList();

        //quitar roles
        if (rolesAQuitar.Any())
        {
            var resultadoQuitar = await _userManager.RemoveFromRolesAsync(usuario, rolesAQuitar);

            if (!resultadoQuitar.Succeeded)
            {
                return resultadoQuitar;
            }
        }
        //Agregar Roles 
        if (rolesAAgregar.Any())
        {
            var resultadoAgregar = await _userManager.AddToRolesAsync(usuario, rolesAAgregar);

            if (!resultadoAgregar.Succeeded)
            {
                return resultadoAgregar;
            }
        }
        //Registrar el cambio
        _logger.LogInformation("Usuario {Email} actualizado correctamente.",usuario.Email);
        //retornamos
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