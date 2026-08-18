using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Oficios.DTO
{
    /// <summary>Inbound contract for agregar un comentario a la bitácora del Oficio.</summary>
    public class OficioComentarioDto
    {
        [Required]
        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }
    }
}
