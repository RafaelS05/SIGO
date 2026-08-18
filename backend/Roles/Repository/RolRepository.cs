using Microsoft.EntityFrameworkCore;
using SIGO.Api.Roles.Model;

namespace SIGO.Api.Roles.Repository
{
    public class RolRepository : IRolRepository
    {
        private readonly AppDbContext _context;

        public RolRepository(AppDbContext context)
        {
            _context = context;
        }

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<Rol>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Rol>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<Rol?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Rol>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(Rol rol, CancellationToken cancellationToken = default)
        {
            await _context.Set<Rol>().AddAsync(rol, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(Rol rol)
        {
            _context.Set<Rol>().Update(rol);
        }

        public void Remove(Rol rol)
        {
            _context.Set<Rol>().Remove(rol);
        }
    }
}
