using SIGO.Api.TiposOficios.DTO;
using SIGO.Api.TiposOficios.Model;
using SIGO.Api.TiposOficios.Repository;

namespace SIGO.Api.TiposOficios.Service
{
    public class TipoOficioService : ITipoOficioService
    {
        private readonly ITipoOficioRepository _repository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repo.

        public TipoOficioService(ITipoOficioRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<TipoOficioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<TipoOficioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var tipoOficio = await _repository.GetByIdAsync(id, cancellationToken);
            return tipoOficio is null ? null : ToResponse(tipoOficio);
        }

        public async Task<TipoOficioResponseDto> CreateAsync(TipoOficioCreateDto dto, CancellationToken cancellationToken = default)
        {
            var tipoOficio = new TipoOficio
            {
                Nombre = dto.Nombre,
                RequiereRespuestaFlag = dto.RequiereRespuestaFlag,
            };

            await _repository.AddAsync(tipoOficio, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // service commits the unit of work

            return ToResponse(tipoOficio);
        }

        public async Task<TipoOficioResponseDto?> UpdateAsync(int id, TipoOficioUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var tipoOficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (tipoOficio is null)
                return null;

            tipoOficio.Nombre = dto.Nombre;
            tipoOficio.RequiereRespuestaFlag = dto.RequiereRespuestaFlag;

            _repository.Update(tipoOficio);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(tipoOficio);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var tipoOficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (tipoOficio is null)
                return false;

            _repository.Remove(tipoOficio);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static TipoOficioResponseDto ToResponse(TipoOficio tipoOficio) => new()
        {
            Id = tipoOficio.Id,
            Nombre = tipoOficio.Nombre,
            RequiereRespuestaFlag = tipoOficio.RequiereRespuestaFlag,
        };
    }
}
