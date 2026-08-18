using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Oficios.DTO
{
    /// <summary>Inbound contract for changing the estado of an Oficio (queda en bitácora).</summary>
    public class OficioCambioEstadoDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdEstadoDestino { get; set; }

        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }
    }
}
