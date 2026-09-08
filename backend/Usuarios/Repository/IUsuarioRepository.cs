using SIGO.Api.Usuarios.Model;

namespace SIGO.Api.Usuarios.Repository
{
    /// <summary>
    /// One interface per aggregate root (Usuario). UsuarioRol belongs to this aggregate,
    /// so its staging methods live here — never in a repo of its own.
    /// Rules: returns domain entities (never DTOs); async by default; never exposes IQueryable;
    /// does NOT call SaveChangesAsync — the service layer commits the unit of work.
    /// </summary>
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Login: búsqueda por correo (índice único, comparación sin distinguir mayúsculas).
        Task<Usuario?> GetByCorreoAsync(string correo, CancellationToken cancellationToken = default);

        // Stages an insert on the tracked context. Service commits.
        Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);

        // Stages an update. Synchronous because it only marks state — no I/O.
        void Update(Usuario usuario);

        // Stages a delete.
        void Remove(Usuario usuario);

        // Stages one rol assignment for the aggregate. Service commits.
        Task AddRolAsync(UsuarioRol usuarioRol, CancellationToken cancellationToken = default);

        // Stages the removal of one rol assignment.
        void RemoveRol(UsuarioRol usuarioRol);
    }
}
