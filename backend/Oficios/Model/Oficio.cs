using SIGO.Api.Canales.Model;
using SIGO.Api.Departamentos.Model;
using SIGO.Api.Estados.Model;
using SIGO.Api.Prioridades.Model;
using SIGO.Api.TiposOficios.Model;
using SIGO.Api.Usuarios.Model;

namespace SIGO.Api.Oficios.Model
{
    /// <summary>
    /// Aggregate root for the Oficios slice. Owns responsables, bitácora and adjuntos.
    /// This is a domain entity — repositories return it, and it never crosses
    /// the API boundary (map to a DTO first).
    /// </summary>
    public class Oficio
    {
        public int Id { get; set; }

        public string Numero { get; set; } = string.Empty;

        public int Anio { get; set; }

        public int Consecutivo { get; set; }

        // "Entrante" | "Saliente"
        public string Direccion { get; set; } = "Entrante";

        public int IdTipoOficio { get; set; }

        public int? IdDepartamentoOrigen { get; set; }

        public int IdDepartamentoDestino { get; set; }

        public string Remitente { get; set; } = string.Empty;

        public string Destinatario { get; set; } = string.Empty;

        public string Asunto { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public int IdPrioridad { get; set; }

        public int IdEstadoActual { get; set; }

        public int IdCanal { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime FechaVencimientoSla { get; set; }

        public DateTime? FechaCierre { get; set; }

        public int CreadoPor { get; set; }

        public DateTime CreadoEn { get; set; }

        public TipoOficio? TipoOficio { get; set; }

        public Departamento? DepartamentoOrigen { get; set; }

        public Departamento? DepartamentoDestino { get; set; }

        public Prioridad? Prioridad { get; set; }

        public Estado? EstadoActual { get; set; }

        public Canal? Canal { get; set; }

        public Usuario? Creador { get; set; }

        public List<OficioResponsable> Responsables { get; set; } = new();

        public List<OficioBitacora> Bitacora { get; set; } = new();

        public List<OficioAdjunto> Adjuntos { get; set; } = new();
    }
}
