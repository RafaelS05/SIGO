namespace SIGO.Api.TiposOficios.Model
{
    /// <summary>
    /// Aggregate root for the TiposOficios slice. This is a domain entity —
    /// repositories return it, and it never crosses the API boundary (map to a DTO first).
    /// </summary>
    public class TipoOficio
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool RequiereRespuestaFlag { get; set; }
    }
}
