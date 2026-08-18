namespace SIGO.Api.Prioridades.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of Prioridad the API exposes.</summary>
    public class PrioridadResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Nivel { get; set; }

        public int SlaHoras { get; set; }
    }
}
