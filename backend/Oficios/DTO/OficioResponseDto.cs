namespace SIGO.Api.Oficios.DTO
{
    /// <summary>Outbound contract returned to clients. The only shape of Oficio the API exposes.</summary>
    public class OficioResponseDto
    {
        public int Id { get; set; }

        public string Numero { get; set; } = string.Empty;

        public int Anio { get; set; }

        public string Direccion { get; set; } = string.Empty;

        public int IdTipoOficio { get; set; }

        public string TipoOficioNombre { get; set; } = string.Empty;

        public int? IdDepartamentoOrigen { get; set; }

        public string DepartamentoOrigenNombre { get; set; } = string.Empty;

        public int IdDepartamentoDestino { get; set; }

        public string DepartamentoDestinoNombre { get; set; } = string.Empty;

        public string Remitente { get; set; } = string.Empty;

        public string Destinatario { get; set; } = string.Empty;

        public string Asunto { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public int IdPrioridad { get; set; }

        public string PrioridadNombre { get; set; } = string.Empty;

        public int IdEstadoActual { get; set; }

        public string EstadoNombre { get; set; } = string.Empty;

        public bool EstadoCerrado { get; set; }

        public int IdCanal { get; set; }

        public string CanalNombre { get; set; } = string.Empty;

        public DateTime FechaIngreso { get; set; }

        public DateTime FechaVencimientoSla { get; set; }

        public DateTime? FechaCierre { get; set; }

        public bool Atrasado { get; set; }

        public int CreadoPor { get; set; }

        public string CreadoPorNombre { get; set; } = string.Empty;

        public DateTime CreadoEn { get; set; }

        public string ResponsableActual { get; set; } = string.Empty;

        public int? IdResponsableActual { get; set; }

        public List<OficioResponsableResponseDto> Responsables { get; set; } = new();

        public List<OficioBitacoraResponseDto> Bitacora { get; set; } = new();

        public List<OficioAdjuntoResponseDto> Adjuntos { get; set; } = new();
    }

    /// <summary>Outbound shape of one asignación (historial de responsables).</summary>
    public class OficioResponsableResponseDto
    {
        public int Id { get; set; }

        public int IdUsuario { get; set; }

        public string UsuarioNombre { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime AsignadoEn { get; set; }

        public string AsignadoPorNombre { get; set; } = string.Empty;
    }

    /// <summary>Outbound shape of one evento de bitácora.</summary>
    public class OficioBitacoraResponseDto
    {
        public int Id { get; set; }

        public string EstadoOrigenNombre { get; set; } = string.Empty;

        public string EstadoDestinoNombre { get; set; } = string.Empty;

        public string Comentario { get; set; } = string.Empty;

        public string UsuarioNombre { get; set; } = string.Empty;

        public DateTime FechaEvento { get; set; }
    }

    /// <summary>Outbound shape of one adjunto (metadata only — download via endpoint).</summary>
    public class OficioAdjuntoResponseDto
    {
        public int Id { get; set; }

        public string NombreArchivo { get; set; } = string.Empty;

        public long PesoBytes { get; set; }

        public int Version { get; set; }

        public string SubidoPorNombre { get; set; } = string.Empty;

        public DateTime SubidoEn { get; set; }
    }
}
