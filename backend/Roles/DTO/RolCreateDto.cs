using System.ComponentModel.DataAnnotations;

namespace SIGO.Api.Roles.DTO
{
    /// <summary>Inbound contract for creating a Rol. Never persisted directly.</summary>
    /// <remarks>
    /// Every field MUST carry validation attributes — [ApiController] auto-returns 400 on invalid
    /// ModelState, but only validates what is decorated. An undecorated field is silently unvalidated.
    /// </remarks>
    public class RolCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
