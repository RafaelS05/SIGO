using SIGO.Api.Roles.Model;

namespace SIGO.Api.Roles.Repository
{
    /// <summary>
    /// One interface per aggregate root (Rol).
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface IRolRepository
    {
        Task<List<Rol>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Rol?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(Rol rol, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(Rol rol);

        // Stages a delete.
        void Remove(Rol rol);
    }
}
