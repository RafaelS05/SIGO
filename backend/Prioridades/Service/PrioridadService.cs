using SIGO.Api.Prioridades.DTO;
using SIGO.Api.Prioridades.Model;
using SIGO.Api.Prioridades.Repository;

namespace SIGO.Api.Prioridades.Service
{
    public class PrioridadService : IPrioridadService
    {
        private readonly IPrioridadRepository _repository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repo.

        public PrioridadService(IPrioridadRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<PrioridadResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<PrioridadResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var prioridad = await _repository.GetByIdAsync(id, cancellationToken);
            return prioridad is null ? null : ToResponse(prioridad);
        }

        public async Task<PrioridadResponseDto> CreateAsync(PrioridadCreateDto dto, CancellationToken cancellationToken = default)
        {
            var prioridad = new Prioridad
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                SlaHoras = dto.SlaHoras,
            };

            await _repository.AddAsync(prioridad, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // service commits the unit of work

            return ToResponse(prioridad);
        }

        public async Task<PrioridadResponseDto?> UpdateAsync(int id, PrioridadUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var prioridad = await _repository.GetByIdAsync(id, cancellationToken);
            if (prioridad is null)
                return null;

            prioridad.Nombre = dto.Nombre;
            prioridad.Nivel = dto.Nivel;
            prioridad.SlaHoras = dto.SlaHoras;

            _repository.Update(prioridad);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(prioridad);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var prioridad = await _repository.GetByIdAsync(id, cancellationToken);
            if (prioridad is null)
                return false;

            _repository.Remove(prioridad);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static PrioridadResponseDto ToResponse(Prioridad prioridad) => new()
        {
            Id = prioridad.Id,
            Nombre = prioridad.Nombre,
            Nivel = prioridad.Nivel,
            SlaHoras = prioridad.SlaHoras,
        };
    }
}
