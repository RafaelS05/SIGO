using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Oficios.DTO
{
    /// <summary>
    /// Inbound contract for asignar/reasignar un responsable (historial en bitácora).
    /// Quién asigna se toma del usuario autenticado, nunca del cuerpo.
    /// </summary>
    public class OficioAsignacionDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }

        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;
    }
}
