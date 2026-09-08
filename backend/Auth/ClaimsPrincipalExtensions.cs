using System.Security.Claims;

namespace SIGO.Api.Auth
{
    /// <summary>Lectura tipada de los claims que emite AuthService al firmar el JWT.</summary>
    public static class ClaimsPrincipalExtensions
    {
        public const string ClaimDepartamento = "departamento";

        public static int GetUsuarioId(this ClaimsPrincipal user)
        {
            var valor = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id) ? id : 0;
        }

        public static int GetDepartamentoId(this ClaimsPrincipal user)
        {
            var valor = user.FindFirstValue(ClaimDepartamento);
            return int.TryParse(valor, out var id) ? id : 0;
        }

        public static bool TienePermiso(this ClaimsPrincipal user, string permiso)
            => Permisos.Matriz.TryGetValue(permiso, out var roles) && roles.Any(user.IsInRole);
    }
}
