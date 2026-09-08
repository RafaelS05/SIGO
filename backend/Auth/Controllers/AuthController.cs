using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGO.Api.Auth.DTO;
using SIGO.Api.Auth.Service;
using SIGO.Api.Usuarios.DTO;

namespace SIGO.Api.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        /// <summary>Inicio de sesión público. 401 si las credenciales no son válidas.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var sesion = await _service.LoginAsync(dto, cancellationToken);
            return sesion is null
                ? Unauthorized(new { title = "Correo o contraseña incorrectos." })
                : Ok(sesion);
        }

        /// <summary>Perfil del usuario autenticado (valida el token al recargar la app).</summary>
        [HttpGet("me")]
        public async Task<ActionResult<UsuarioResponseDto>> Me(CancellationToken cancellationToken)
        {
            var perfil = await _service.GetPerfilAsync(User.GetUsuarioId(), cancellationToken);
            return perfil is null ? Unauthorized() : Ok(perfil);
        }

        /// <summary>Lista de permisos (matriz RBAC) para que el cliente los replique.</summary>
        [HttpGet("permisos")]
        public ActionResult<IReadOnlyDictionary<string, string[]>> Permisos()
            => Ok(Auth.Permisos.Matriz);

        /// <summary>Registro de usuarios: URL restringida al Administrador.</summary>
        [HttpPost("registro")]
        [Authorize(Policy = Auth.Permisos.AdministrarUsuarios)]
        public async Task<ActionResult<UsuarioResponseDto>> Registrar([FromBody] RegistroUsuarioDto dto, CancellationToken cancellationToken)
        {
            var creado = await _service.RegistrarAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(Me), null, creado);
        }

        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambioPasswordDto dto, CancellationToken cancellationToken)
        {
            var ok = await _service.CambiarPasswordAsync(User.GetUsuarioId(), dto, cancellationToken);
            return ok ? NoContent() : BadRequest(new { title = "La contraseña actual no es correcta." });
        }

        [HttpPost("usuarios/{id}/restablecer-password")]
        [Authorize(Policy = Auth.Permisos.AdministrarUsuarios)]
        public async Task<IActionResult> RestablecerPassword(int id, [FromBody] RestablecerPasswordDto dto, CancellationToken cancellationToken)
            => await _service.RestablecerPasswordAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }
}
