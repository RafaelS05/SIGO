using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Estados.DTO
{
    /// <summary>Inbound contract for updating an existing Estado.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// </remarks>
    public class EstadoUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool CerradoFlag { get; set; }
    }
}
