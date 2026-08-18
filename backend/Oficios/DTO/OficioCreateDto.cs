using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Oficios.DTO
{
    /// <summary>Inbound contract for creating an Oficio. Never persisted directly.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// Numero, Anio, Consecutivo, FechaVencimientoSla y estado inicial los calcula el servicio.
    /// </remarks>
    public class OficioCreateDto
    {
        [Required]
        [RegularExpression("Entrante|Saliente")]
        public string Direccion { get; set; } = "Entrante";

        [Required]
        [Range(1, int.MaxValue)]
        public int IdTipoOficio { get; set; }

        [Range(1, int.MaxValue)]
        public int? IdDepartamentoOrigen { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdDepartamentoDestino { get; set; }

        [Required]
        [MaxLength(200)]
        public string Remitente { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Destinatario { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Asunto { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int IdPrioridad { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdCanal { get; set; }

        [Required]
        public DateTime FechaIngreso { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CreadoPor { get; set; }
    }
}
