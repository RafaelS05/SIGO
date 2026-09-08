using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Oficios.DTO
{
    /// <summary>
    /// Inbound contract for agregar un comentario a la bitácora del Oficio.
    /// El autor se toma del usuario autenticado.
    /// </summary>
    public class OficioComentarioDto
    {
        [Required]
        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;
    }
}
