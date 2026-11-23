namespace SIGO.Api.Estados.DTO
{
    public class EstadoResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool CerradoFlag { get; set; }
    }
}
