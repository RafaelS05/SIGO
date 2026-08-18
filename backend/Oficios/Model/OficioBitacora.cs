using SIGO.Api.Estados.Model;
using SIGO.Api.Usuarios.Model;

namespace SIGO.Api.Oficios.Model
{
    /// <summary>
    /// Owned entity of the Oficio aggregate: audit trail. Every state change,
    /// assignment, comment and attachment is recorded with usuario, fecha y motivo.
    /// </summary>
    public class OficioBitacora
    {
        public int Id { get; set; }

        public int IdOficio { get; set; }

        public int? IdEstadoOrigen { get; set; }

        public int? IdEstadoDestino { get; set; }

        public string Comentario { get; set; } = string.Empty;

        public int IdUsuario { get; set; }

        public DateTime FechaEvento { get; set; }

        public Estado? EstadoOrigen { get; set; }

        public Estado? EstadoDestino { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
