namespace SIGO.Api.Oficios.DTO
{
    /// <summary>Outbound-only: lo que el controller necesita para servir la descarga autenticada.</summary>
    public class OficioAdjuntoDescargaDto
    {
        public string RutaAbsoluta { get; set; } = string.Empty;

        public string NombreArchivo { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;
    }
}
