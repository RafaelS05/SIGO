using SIGO.Api.Roles.Model;

namespace SIGO.Api.Usuarios.Model
{
    /// <summary>
    /// Owned join entity of the Usuario aggregate (UsuarioRol N:M with Rol).
    /// Composite key (IdUsuario, IdRol) configured in AppDbContext.
    /// </summary>
    public class UsuarioRol
    {
        public int IdUsuario { get; set; }

        public int IdRol { get; set; }

        public Rol? Rol { get; set; }
    }
}
