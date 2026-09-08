using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Auth.DTO
{
    /// <summary>El Administrador asigna una contraseña nueva a otro usuario (sin conocer la anterior).</summary>
    public class RestablecerPasswordDto
    {
        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string PasswordNueva { get; set; } = string.Empty;
    }
}
