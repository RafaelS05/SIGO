using SIGO.Api.Departamentos.Model;

namespace SIGO.Api.Departamentos.Repository
{
    /// <summary>
    /// One interface per aggregate root (Departamento).
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface IDepartamentoRepository
    {
        Task<List<Departamento>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Departamento?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(Departamento departamento, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(Departamento departamento);

        // Stages a delete.
        void Remove(Departamento departamento);
    }
}
