using SIGO.Api.Prioridades.Model;

namespace SIGO.Api.Prioridades.Repository
{
    /// <summary>
    /// One interface per aggregate root (Prioridad).
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface IPrioridadRepository
    {
        Task<List<Prioridad>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Prioridad?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(Prioridad prioridad, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(Prioridad prioridad);

        // Stages a delete.
        void Remove(Prioridad prioridad);
    }
}
