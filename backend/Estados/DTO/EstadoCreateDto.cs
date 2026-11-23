using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Estados.DTO
{
    public class EstadoCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool CerradoFlag { get; set; }
    }
}
