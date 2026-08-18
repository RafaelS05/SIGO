using Microsoft.EntityFrameworkCore;
using SIGO.Api.Prioridades.Model;

namespace SIGO.Api.Prioridades.Repository
{
    public class PrioridadRepository : IPrioridadRepository
    {
        private readonly AppDbContext _context;

        public PrioridadRepository(AppDbContext context)
        {
            _context = context;
        }

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<Prioridad>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Prioridad>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<Prioridad?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Prioridad>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(Prioridad prioridad, CancellationToken cancellationToken = default)
        {
            await _context.Set<Prioridad>().AddAsync(prioridad, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(Prioridad prioridad)
        {
            _context.Set<Prioridad>().Update(prioridad);
        }

        public void Remove(Prioridad prioridad)
        {
            _context.Set<Prioridad>().Remove(prioridad);
        }
    }
}
