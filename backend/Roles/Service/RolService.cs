using SIGO.Api.Roles.DTO;
using SIGO.Api.Roles.Model;
using SIGO.Api.Roles.Repository;

namespace SIGO.Api.Roles.Service
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _repository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repo.

        public RolService(IRolRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<RolResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<RolResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var rol = await _repository.GetByIdAsync(id, cancellationToken);
            return rol is null ? null : ToResponse(rol);
        }

        public async Task<RolResponseDto> CreateAsync(RolCreateDto dto, CancellationToken cancellationToken = default)
        {
            var rol = new Rol
            {
                Nombre = dto.Nombre,
            };

            await _repository.AddAsync(rol, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // service commits the unit of work

            return ToResponse(rol);
        }

        public async Task<RolResponseDto?> UpdateAsync(int id, RolUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var rol = await _repository.GetByIdAsync(id, cancellationToken);
            if (rol is null)
                return null;

            rol.Nombre = dto.Nombre;

            _repository.Update(rol);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(rol);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var rol = await _repository.GetByIdAsync(id, cancellationToken);
            if (rol is null)
                return false;

            _repository.Remove(rol);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static RolResponseDto ToResponse(Rol rol) => new()
        {
            Id = rol.Id,
            Nombre = rol.Nombre,
        };
    }
}
