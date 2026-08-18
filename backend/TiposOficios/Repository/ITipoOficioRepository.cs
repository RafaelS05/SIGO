using SIGO.Api.TiposOficios.Model;

namespace SIGO.Api.TiposOficios.Repository
{
    /// <summary>
    /// One interface per aggregate root (TipoOficio).
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface ITipoOficioRepository
    {
        Task<List<TipoOficio>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TipoOficio?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(TipoOficio tipoOficio, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(TipoOficio tipoOficio);

        // Stages a delete.
        void Remove(TipoOficio tipoOficio);
    }
}
