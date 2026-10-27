using SGAT.Application.DTOs.Rol.SGAT.Application.DTOs.Rol;
using SGAT.Application.DTOs.Usuario;

namespace SGAT.Web.ViewModels.Usuario
{
    public class UsuarioEditViewModel
    {
        public UsuarioEditDto Usuario { get; set; } = new();

        public IReadOnlyList<RolDto> RolesDisponibles { get; set; } = new List<RolDto>();
    }
}
