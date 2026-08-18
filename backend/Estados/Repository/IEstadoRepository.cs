using SIGO.Api.Estados.Model;

namespace SIGO.Api.Estados.Repository
{
    /// <summary>
    /// One interface per aggregate root (Estado).
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface IEstadoRepository
    {
        Task<List<Estado>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Estado?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(Estado estado, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(Estado estado);

        // Stages a delete.
        void Remove(Estado estado);
    }
}
