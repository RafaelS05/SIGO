using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Oficios.DTO
{
    /// <summary>Inbound contract for asignar/reasignar un responsable (historial en bitácora).</summary>
    public class OficioAsignacionDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int AsignadoPor { get; set; }

        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;
    }
}
