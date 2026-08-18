using Microsoft.EntityFrameworkCore;
using SIGO.Api.Departamentos.Model;

namespace SIGO.Api.Departamentos.Repository
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly AppDbContext _context;

        public DepartamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<Departamento>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Departamento>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<Departamento?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Departamento>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(Departamento departamento, CancellationToken cancellationToken = default)
        {
            await _context.Set<Departamento>().AddAsync(departamento, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(Departamento departamento)
        {
            _context.Set<Departamento>().Update(departamento);
        }

        public void Remove(Departamento departamento)
        {
            _context.Set<Departamento>().Remove(departamento);
        }
    }
}
