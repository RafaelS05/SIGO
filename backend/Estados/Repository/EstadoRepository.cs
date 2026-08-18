using Microsoft.EntityFrameworkCore;
using SIGO.Api.Estados.Model;

namespace SIGO.Api.Estados.Repository
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly AppDbContext _context;

        public EstadoRepository(AppDbContext context)
        {
            _context = context;
        }

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<Estado>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Estado>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<Estado?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Estado>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(Estado estado, CancellationToken cancellationToken = default)
        {
            await _context.Set<Estado>().AddAsync(estado, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(Estado estado)
        {
            _context.Set<Estado>().Update(estado);
        }

        public void Remove(Estado estado)
        {
            _context.Set<Estado>().Remove(estado);
        }
    }
}
