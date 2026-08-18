using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Estados.DTO
{
    /// <summary>Inbound contract for creating a Estado. Never persisted directly.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// </remarks>
    public class EstadoCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool CerradoFlag { get; set; }
    }
}
