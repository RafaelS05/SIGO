using SIGO.Api.Usuarios.DTO;

namespace SIGO.Api.Auth.DTO
{
    /// <summary>Sesión emitida tras un login correcto: JWT + perfil del usuario (con roles).</summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiraEn { get; set; }

        public UsuarioResponseDto Usuario { get; set; } = new();
    }
}
