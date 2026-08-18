using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.TiposOficios.DTO
{
    /// <summary>Inbound contract for updating an existing TipoOficio.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// </remarks>
    public class TipoOficioUpdateDto
    {
        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool RequiereRespuestaFlag { get; set; }
    }
}
