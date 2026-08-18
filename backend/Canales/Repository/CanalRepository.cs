using Microsoft.EntityFrameworkCore;
using SIGO.Api.Canales.Model;

namespace SIGO.Api.Canales.Repository
{
    public class CanalRepository : ICanalRepository
    {
        private readonly AppDbContext _context;

        public CanalRepository(AppDbContext context)
        {
            _context = context;
        }

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<Canal>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Canal>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<Canal?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Canal>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(Canal canal, CancellationToken cancellationToken = default)
        {
            await _context.Set<Canal>().AddAsync(canal, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(Canal canal)
        {
            _context.Set<Canal>().Update(canal);
        }

        public void Remove(Canal canal)
        {
            _context.Set<Canal>().Remove(canal);
        }
    }
}
