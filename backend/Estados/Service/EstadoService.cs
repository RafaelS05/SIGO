using SIGO.Api.Estados.DTO;
using SIGO.Api.Estados.Model;
using SIGO.Api.Estados.Repository;

namespace SIGO.Api.Estados.Service
{
    public class EstadoService : IEstadoService
    {
        private readonly IEstadoRepository _repository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repo.

        public EstadoService(IEstadoRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<EstadoResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<EstadoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var estado = await _repository.GetByIdAsync(id, cancellationToken);
            return estado is null ? null : ToResponse(estado);
        }

        public async Task<EstadoResponseDto> CreateAsync(EstadoCreateDto dto, CancellationToken cancellationToken = default)
        {
            var estado = new Estado
            {
                Nombre = dto.Nombre,
                CerradoFlag = dto.CerradoFlag,
            };

            await _repository.AddAsync(estado, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // service commits the unit of work

            return ToResponse(estado);
        }

        public async Task<EstadoResponseDto?> UpdateAsync(int id, EstadoUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var estado = await _repository.GetByIdAsync(id, cancellationToken);
            if (estado is null)
                return null;

            estado.Nombre = dto.Nombre;
            estado.CerradoFlag = dto.CerradoFlag;

            _repository.Update(estado);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(estado);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var estado = await _repository.GetByIdAsync(id, cancellationToken);
            if (estado is null)
                return false;

            _repository.Remove(estado);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static EstadoResponseDto ToResponse(Estado estado) => new()
        {
            Id = estado.Id,
            Nombre = estado.Nombre,
            CerradoFlag = estado.CerradoFlag,
        };
    }
}
