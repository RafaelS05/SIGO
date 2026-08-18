using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Usuarios.DTO
{
    /// <summary>Inbound contract for creating a Usuario. Never persisted directly.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// </remarks>
    public class UsuarioCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int IdDepartamento { get; set; }

        [Required]
        [MaxLength(150)]
        public string Cargo { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        [Required]
        public bool OrigenAd { get; set; }

        [Required]
        [MinLength(1)]
        public List<int> RolIds { get; set; } = new();
    }
}
