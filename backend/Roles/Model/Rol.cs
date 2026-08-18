namespace SIGO.Api.Roles.Model
{
    /// <summary>
    /// Aggregate root for the Roles slice. This is a domain entity —
    /// repositories return it, and it never crosses the API boundary (map to a DTO first).
    /// </summary>
    public class Rol
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
