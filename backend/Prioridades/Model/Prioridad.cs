namespace SIGO.Api.Prioridades.Model
{
    /// <summary>
    /// Aggregate root for the Prioridades slice. This is a domain entity —
    /// repositories return it, and it never crosses the API boundary (map to a DTO first).
    /// </summary>
    public class Prioridad
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Nivel { get; set; }

        public int SlaHoras { get; set; }
    }
}
