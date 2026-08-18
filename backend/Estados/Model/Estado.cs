namespace SIGO.Api.Estados.Model
{
    /// <summary>
    /// Aggregate root for the Estados slice. This is a domain entity —
    /// repositories return it, and it never crosses the API boundary (map to a DTO first).
    /// </summary>
    public class Estado
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool CerradoFlag { get; set; }
    }
}
