using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Prioridades.DTO
{
    /// <summary>Inbound contract for creating a Prioridad. Never persisted directly.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// </remarks>
    public class PrioridadCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Nivel { get; set; }

        [Required]
        [Range(1, 8760)]
        public int SlaHoras { get; set; }
    }
}
