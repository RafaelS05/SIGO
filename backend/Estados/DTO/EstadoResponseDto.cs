namespace SIGO.Api.Estados.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of Estado the API exposes.</summary>
    public class EstadoResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool CerradoFlag { get; set; }
    }
}
