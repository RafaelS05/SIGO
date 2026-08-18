using Microsoft.EntityFrameworkCore;
using SIGO.Api.Oficios.Model;

namespace SIGO.Api.Oficios.Repository
{
    public class OficioRepository : IOficioRepository
    {
        private readonly AppDbContext _context;

        public OficioRepository(AppDbContext context)
        {
            _context = context;
        }

        // Navs needed to render a bandeja row (estado, prioridad, deptos, responsable activo).
        private IQueryable<Oficio> BaseQuery() =>
            _context.Set<Oficio>()
                .AsNoTracking()
                .Include(o => o.TipoOficio)
                .Include(o => o.DepartamentoOrigen)
                .Include(o => o.DepartamentoDestino)
                .Include(o => o.Prioridad)
                .Include(o => o.EstadoActual)
                .Include(o => o.Canal)
                .Include(o => o.Creador)
                .Include(o => o.Responsables)
                    .ThenInclude(r => r.Usuario);

        // AsNoTracking() always. Returns a materialized list, never IQueryable.
        public async Task<List<Oficio>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await BaseQuery()
                .OrderByDescending(o => o.FechaIngreso)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Oficio>> GetFilteredAsync(
            int? estadoId,
            int? prioridadId,
            int? departamentoId,
            int? responsableId,
            string? busqueda,
            DateTime? desde,
            DateTime? hasta,
            bool soloAtrasados,
            CancellationToken cancellationToken = default)
        {
            var query = BaseQuery();

            if (estadoId is not null)
                query = query.Where(o => o.IdEstadoActual == estadoId);
            if (prioridadId is not null)
                query = query.Where(o => o.IdPrioridad == prioridadId);
            if (departamentoId is not null)
                query = query.Where(o => o.IdDepartamentoDestino == departamentoId);
            if (responsableId is not null)
                query = query.Where(o => o.Responsables.Any(r => r.Activo && r.IdUsuario == responsableId));
            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(o =>
                    o.Numero.Contains(busqueda) ||
                    o.Asunto.Contains(busqueda) ||
                    o.Remitente.Contains(busqueda) ||
                    o.Destinatario.Contains(busqueda));
            if (desde is not null)
                query = query.Where(o => o.FechaIngreso >= desde);
            if (hasta is not null)
                query = query.Where(o => o.FechaIngreso <= hasta);
            if (soloAtrasados)
                query = query.Where(o => o.FechaVencimientoSla < DateTime.Now && !o.EstadoActual!.CerradoFlag);

            return await query
                .OrderByDescending(o => o.FechaIngreso)
                .ToListAsync(cancellationToken);
        }

        // AsNoTracking() on every query — no exceptions. The returned entity is detached; the service's
        // update/delete flow re-attaches it via Update()/Remove() below, so the tracker is never needed here.
        public async Task<Oficio?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await BaseQuery()
                .Include(o => o.Responsables)
                    .ThenInclude(r => r.Asignador)
                .Include(o => o.Bitacora)
                    .ThenInclude(b => b.Usuario)
                .Include(o => o.Bitacora)
                    .ThenInclude(b => b.EstadoOrigen)
                .Include(o => o.Bitacora)
                    .ThenInclude(b => b.EstadoDestino)
                .Include(o => o.Adjuntos)
                    .ThenInclude(a => a.Subidor)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<int> GetMaxConsecutivoAsync(int anio, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Oficio>()
                .AsNoTracking()
                .Where(o => o.Anio == anio)
                .Select(o => (int?)o.Consecutivo)
                .MaxAsync(cancellationToken) ?? 0;
        }

        public async Task AddAsync(Oficio oficio, CancellationToken cancellationToken = default)
        {
            await _context.Set<Oficio>().AddAsync(oficio, cancellationToken);
            // No SaveChangesAsync here — the service owns the commit (Unit-of-Work Concern).
        }

        public void Update(Oficio oficio)
        {
            _context.Set<Oficio>().Update(oficio);
        }

        public void Remove(Oficio oficio)
        {
            _context.Set<Oficio>().Remove(oficio);
        }

        public async Task AddBitacoraAsync(OficioBitacora evento, CancellationToken cancellationToken = default)
        {
            await _context.Set<OficioBitacora>().AddAsync(evento, cancellationToken);
        }

        public async Task AddResponsableAsync(OficioResponsable responsable, CancellationToken cancellationToken = default)
        {
            await _context.Set<OficioResponsable>().AddAsync(responsable, cancellationToken);
        }

        public void UpdateResponsable(OficioResponsable responsable)
        {
            _context.Set<OficioResponsable>().Update(responsable);
        }

        public async Task AddAdjuntoAsync(OficioAdjunto adjunto, CancellationToken cancellationToken = default)
        {
            await _context.Set<OficioAdjunto>().AddAsync(adjunto, cancellationToken);
        }

        public async Task<OficioAdjunto?> GetAdjuntoByIdAsync(int idOficio, int idAdjunto, CancellationToken cancellationToken = default)
        {
            return await _context.Set<OficioAdjunto>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdOficio == idOficio && a.Id == idAdjunto, cancellationToken);
        }
    }
}
