namespace SIGO.Api.Departamentos.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of Departamento the API exposes.</summary>
    public class DepartamentoResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Sigla { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}
