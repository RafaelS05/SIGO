using SIGO.Api.Departamentos.DTO;
using SIGO.Api.Departamentos.Model;
using SIGO.Api.Departamentos.Repository;

namespace SIGO.Api.Departamentos.Service
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _repository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repo.

        public DepartamentoService(IDepartamentoRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<DepartamentoResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<DepartamentoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var departamento = await _repository.GetByIdAsync(id, cancellationToken);
            return departamento is null ? null : ToResponse(departamento);
        }

        public async Task<DepartamentoResponseDto> CreateAsync(DepartamentoCreateDto dto, CancellationToken cancellationToken = default)
        {
            var departamento = new Departamento
            {
                Nombre = dto.Nombre,
                Sigla = dto.Sigla,
                Activo = dto.Activo,
            };

            await _repository.AddAsync(departamento, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // service commits the unit of work

            return ToResponse(departamento);
        }

        public async Task<DepartamentoResponseDto?> UpdateAsync(int id, DepartamentoUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var departamento = await _repository.GetByIdAsync(id, cancellationToken);
            if (departamento is null)
                return null;

            departamento.Nombre = dto.Nombre;
            departamento.Sigla = dto.Sigla;
            departamento.Activo = dto.Activo;

            _repository.Update(departamento);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(departamento);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var departamento = await _repository.GetByIdAsync(id, cancellationToken);
            if (departamento is null)
                return false;

            _repository.Remove(departamento);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static DepartamentoResponseDto ToResponse(Departamento departamento) => new()
        {
            Id = departamento.Id,
            Nombre = departamento.Nombre,
            Sigla = departamento.Sigla,
            Activo = departamento.Activo,
        };
    }
}
