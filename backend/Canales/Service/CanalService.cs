using SIGO.Api.Canales.DTO;
using SIGO.Api.Canales.Model;
using SIGO.Api.Canales.Repository;

namespace SIGO.Api.Canales.Service
{
    public class CanalService : ICanalService
    {
        private readonly ICanalRepository _repository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repo.

        public CanalService(ICanalRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<CanalResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<CanalResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var canal = await _repository.GetByIdAsync(id, cancellationToken);
            return canal is null ? null : ToResponse(canal);
        }

        public async Task<CanalResponseDto> CreateAsync(CanalCreateDto dto, CancellationToken cancellationToken = default)
        {
            var canal = new Canal
            {
                Nombre = dto.Nombre,
            };

            await _repository.AddAsync(canal, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // service commits the unit of work

            return ToResponse(canal);
        }

        public async Task<CanalResponseDto?> UpdateAsync(int id, CanalUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var canal = await _repository.GetByIdAsync(id, cancellationToken);
            if (canal is null)
                return null;

            canal.Nombre = dto.Nombre;

            _repository.Update(canal);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(canal);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var canal = await _repository.GetByIdAsync(id, cancellationToken);
            if (canal is null)
                return false;

            _repository.Remove(canal);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static CanalResponseDto ToResponse(Canal canal) => new()
        {
            Id = canal.Id,
            Nombre = canal.Nombre,
        };
    }
}
