using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Auth.DTO
{
    /// <summary>Cambio de contraseña del usuario autenticado (exige la contraseña actual).</summary>
    public class CambioPasswordDto
    {
        [Required]
        [MaxLength(128)]
        public string PasswordActual { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string PasswordNueva { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(PasswordNueva), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
