using SIGO.Api.Usuarios.Model;

namespace SIGO.Api.Oficios.Model
{
    /// <summary>
    /// Owned entity of the Oficio aggregate: one responsable assignment.
    /// Reassignments never delete rows — the previous one is flagged inactive (historial).
    /// </summary>
    public class OficioResponsable
    {
        public int Id { get; set; }

        public int IdOficio { get; set; }

        public int IdUsuario { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime AsignadoEn { get; set; }

        public int AsignadoPor { get; set; }

        public Usuario? Usuario { get; set; }

        public Usuario? Asignador { get; set; }
    }
}
