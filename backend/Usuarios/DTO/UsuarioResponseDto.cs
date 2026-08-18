namespace SIGO.Api.Usuarios.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of Usuario the API exposes.</summary>
    public class UsuarioResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public int IdDepartamento { get; set; }

        public string DepartamentoNombre { get; set; } = string.Empty;

        public string Cargo { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public bool OrigenAd { get; set; }

        public List<UsuarioRolResponseDto> Roles { get; set; } = new();
    }

    /// <summary>Outbound shape of one rol assigned to a usuario.</summary>
    public class UsuarioRolResponseDto
    {
        public int IdRol { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
