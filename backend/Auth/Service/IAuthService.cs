using SIGO.Api.Auth.DTO;
using SIGO.Api.Usuarios.DTO;

namespace SIGO.Api.Auth.Service
{
    /// <summary>
    /// Autenticación local (correo + contraseña) y emisión de JWT. Los roles del usuario
    /// viajan como claims; la autorización por endpoint la resuelven las policies de <see cref="Permisos"/>.
    /// </summary>
    public interface IAuthService
    {
        // Devuelve null si las credenciales no son válidas o la cuenta está inactiva.
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);

        Task<UsuarioResponseDto?> GetPerfilAsync(int idUsuario, CancellationToken cancellationToken = default);

        // Alta con credenciales (solo Administrador). Lanza InvalidOperationException si el correo ya existe.
        Task<UsuarioResponseDto> RegistrarAsync(RegistroUsuarioDto dto, CancellationToken cancellationToken = default);

        // false si la contraseña actual no coincide.
        Task<bool> CambiarPasswordAsync(int idUsuario, CambioPasswordDto dto, CancellationToken cancellationToken = default);

        // Restablecimiento administrativo. false si el usuario no existe.
        Task<bool> RestablecerPasswordAsync(int idUsuario, RestablecerPasswordDto dto, CancellationToken cancellationToken = default);
    }
}
