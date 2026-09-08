using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Auth.DTO
{
    /// <summary>Credenciales del formulario público de inicio de sesión.</summary>
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;
    }
}
