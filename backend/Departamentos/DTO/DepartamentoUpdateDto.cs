using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Departamentos.DTO
{
    /// <summary>Inbound contract for updating an existing Departamento.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// </remarks>
    public class DepartamentoUpdateDto
    {
        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Sigla { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;
    }
}
