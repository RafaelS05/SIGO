namespace SIGO.Api.TiposOficios.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of TipoOficio the API exposes.</summary>
    public class TipoOficioResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool RequiereRespuestaFlag { get; set; }
    }
}
