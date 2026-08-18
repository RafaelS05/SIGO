using Microsoft.EntityFrameworkCore;
using SIGO.Api.Usuarios.Model;

namespace SIGO.Api.Usuarios.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .Include(u => u.Departamento)
                .Include(u => u.Roles)
                    .ThenInclude(ur => ur.Rol)
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .Include(u => u.Departamento)
                .Include(u => u.Roles)
                    .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            await _context.Set<Usuario>().AddAsync(usuario, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(Usuario usuario)
        {
            _context.Set<Usuario>().Update(usuario);
        }

        public void Remove(Usuario usuario)
        {
            _context.Set<Usuario>().Remove(usuario);
        }

        public async Task AddRolAsync(UsuarioRol usuarioRol, CancellationToken cancellationToken = default)
        {
            await _context.Set<UsuarioRol>().AddAsync(usuarioRol, cancellationToken);
        }

        public void RemoveRol(UsuarioRol usuarioRol)
        {
            _context.Set<UsuarioRol>().Remove(usuarioRol);
        }
    }
}
