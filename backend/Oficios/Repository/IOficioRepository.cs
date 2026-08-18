using SIGO.Api.Oficios.Model;

namespace SIGO.Api.Oficios.Repository
{
    /// <summary>
    /// One interface per aggregate root (Oficio). Responsables, bitácora y adjuntos belong
    /// to this aggregate, so their staging methods live here — never in repos of their own.
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface IOficioRepository
    {
        Task<List<Oficio>> GetAllAsync(CancellationToken cancellationToken = default);

        // Bandejas y búsquedas: filtros por estado, prioridad, departamento destino,
        // responsable activo, texto (número/asunto/remitente), rango de fechas y atrasados por SLA.
        Task<List<Oficio>> GetFilteredAsync(
            int? estadoId,
            int? prioridadId,
            int? departamentoId,
            int? responsableId,
            string? busqueda,
            DateTime? desde,
            DateTime? hasta,
            bool soloAtrasados,
            CancellationToken cancellationToken = default);

        Task<Oficio?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Numeración: consecutivo actual del año (0 si no hay oficios ese año).
        Task<int> GetMaxConsecutivoAsync(int anio, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(Oficio oficio, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(Oficio oficio);

        // Stages a delete.
        void Remove(Oficio oficio);

        // Owned-entity staging (aggregate internals). Service commits.
        Task AddBitacoraAsync(OficioBitacora evento, CancellationToken cancellationToken = default);

        Task AddResponsableAsync(OficioResponsable responsable, CancellationToken cancellationToken = default);

        void UpdateResponsable(OficioResponsable responsable);

        Task AddAdjuntoAsync(OficioAdjunto adjunto, CancellationToken cancellationToken = default);

        Task<OficioAdjunto?> GetAdjuntoByIdAsync(int idOficio, int idAdjunto, CancellationToken cancellationToken = default);
    }
}
