namespace SIGO.Api.Departamentos.Model
{
    /// <summary>
    /// Aggregate root for the Departamentos slice. This is a domain entity —
    /// repositories return it, and it never crosses the API boundary (map to a DTO first).
    /// </summary>
    public class Departamento
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Sigla { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}
