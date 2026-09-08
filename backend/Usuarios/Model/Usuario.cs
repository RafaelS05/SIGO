using SIGO.Api.Departamentos.Model;

namespace SIGO.Api.Usuarios.Model
{
    /// <summary>
    /// Aggregate root for the Usuarios slice. This is a domain entity —
    /// repositories return it, and it never crosses the API boundary (map to a DTO first).
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public int IdDepartamento { get; set; }

        public string Cargo { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public bool OrigenAd { get; set; }

        /// <summary>Hash PBKDF2 (ASP.NET Identity PasswordHasher). Null = cuenta sin credencial local.</summary>
        public string? PasswordHash { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        public Departamento? Departamento { get; set; }

        public List<UsuarioRol> Roles { get; set; } = new();
    }
}
