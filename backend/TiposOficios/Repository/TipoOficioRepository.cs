using Microsoft.EntityFrameworkCore;
using SIGO.Api.TiposOficios.Model;

namespace SIGO.Api.TiposOficios.Repository
{
    public class TipoOficioRepository : ITipoOficioRepository
    {
        private readonly AppDbContext _context;

        public TipoOficioRepository(AppDbContext context)
        {
            _context = context;
        }

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<TipoOficio>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<TipoOficio>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<TipoOficio?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TipoOficio>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(TipoOficio tipoOficio, CancellationToken cancellationToken = default)
        {
            await _context.Set<TipoOficio>().AddAsync(tipoOficio, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(TipoOficio tipoOficio)
        {
            _context.Set<TipoOficio>().Update(tipoOficio);
        }

        public void Remove(TipoOficio tipoOficio)
        {
            _context.Set<TipoOficio>().Remove(tipoOficio);
        }
    }
}
