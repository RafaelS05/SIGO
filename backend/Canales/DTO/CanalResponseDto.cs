namespace SIGO.Api.Canales.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of Canal the API exposes.</summary>
    public class CanalResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
