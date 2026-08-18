using SIGO.Api.Usuarios.Model;

namespace SIGO.Api.Oficios.Model
{
    /// <summary>
    /// Owned entity of the Oficio aggregate: attachment metadata.
    /// The binary lives on disk (App_Data/adjuntos); only metadata is persisted.
    /// </summary>
    public class OficioAdjunto
    {
        public int Id { get; set; }

        public int IdOficio { get; set; }

        public string NombreArchivo { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;

        public long PesoBytes { get; set; }

        public int Version { get; set; } = 1;

        public int SubidoPor { get; set; }

        public DateTime SubidoEn { get; set; }

        public Usuario? Subidor { get; set; }
    }
}
