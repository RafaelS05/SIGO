using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Auth.DTO
{
    /// <summary>
    /// Alta de usuario por parte del Administrador (URL de registro restringida):
    /// datos personales + credenciales + roles.
    /// </summary>
    public class RegistroUsuarioDto
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
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<int> RolIds { get; set; } = new();

        [Required]
        public bool Activo { get; set; } = true;
    }
}
