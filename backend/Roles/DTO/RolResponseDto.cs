namespace SIGO.Api.Roles.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of Rol the API exposes.</summary>
    public class RolResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
