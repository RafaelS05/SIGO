using SIGO.Api.Canales.Model;

namespace SIGO.Api.Canales.Repository
{
    /// <summary>
    /// One interface per aggregate root (Canal).
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface ICanalRepository
    {
        Task<List<Canal>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Canal?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(Canal canal, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(Canal canal);

        // Stages a delete.
        void Remove(Canal canal);
    }
}
