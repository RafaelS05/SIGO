using SIGO.Api.Usuarios.DTO;
using SIGO.Api.Usuarios.Model;
using SIGO.Api.Usuarios.Repository;

namespace SIGO.Api.Usuarios.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repo.

        public UsuarioService(IUsuarioRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<UsuarioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _repository.GetByIdAsync(id, cancellationToken);
            return usuario is null ? null : ToResponse(usuario);
        }

        public async Task<UsuarioResponseDto> CreateAsync(UsuarioCreateDto dto, CancellationToken cancellationToken = default)
        {
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                IdDepartamento = dto.IdDepartamento,
                Cargo = dto.Cargo,
                Activo = dto.Activo,
                OrigenAd = dto.OrigenAd,
                Roles = dto.RolIds.Distinct().Select(rolId => new UsuarioRol { IdRol = rolId }).ToList(),
            };

            await _repository.AddAsync(usuario, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // service commits the unit of work

            var created = await _repository.GetByIdAsync(usuario.Id, cancellationToken);
            return ToResponse(created!);
        }

        public async Task<UsuarioResponseDto?> UpdateAsync(int id, UsuarioUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var usuario = await _repository.GetByIdAsync(id, cancellationToken);
            if (usuario is null)
                return null;

            // Diff the rol assignments before re-attaching the aggregate root.
            var targetRolIds = dto.RolIds.Distinct().ToList();
            foreach (var existente in usuario.Roles.Where(ur => !targetRolIds.Contains(ur.IdRol)).ToList())
                _repository.RemoveRol(new UsuarioRol { IdUsuario = id, IdRol = existente.IdRol });

            foreach (var rolId in targetRolIds.Where(rolId => usuario.Roles.All(ur => ur.IdRol != rolId)))
                await _repository.AddRolAsync(new UsuarioRol { IdUsuario = id, IdRol = rolId }, cancellationToken);

            usuario.Nombre = dto.Nombre;
            usuario.Correo = dto.Correo;
            usuario.IdDepartamento = dto.IdDepartamento;
            usuario.Cargo = dto.Cargo;
            usuario.Activo = dto.Activo;
            usuario.OrigenAd = dto.OrigenAd;
            // PasswordHash y UltimoAcceso se conservan tal cual vienen de la BD: solo el slice Auth los modifica.
            usuario.Roles = new List<UsuarioRol>(); // detach graph so Update() only re-attaches the root
            usuario.Departamento = null;

            _repository.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken); // one commit covers root + rol rows

            var updated = await _repository.GetByIdAsync(id, cancellationToken);
            return ToResponse(updated!);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _repository.GetByIdAsync(id, cancellationToken);
            if (usuario is null)
                return false;

            usuario.Departamento = null;
            _repository.Remove(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static UsuarioResponseDto ToResponse(Usuario usuario) => new()
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            IdDepartamento = usuario.IdDepartamento,
            DepartamentoNombre = usuario.Departamento?.Nombre ?? string.Empty,
            Cargo = usuario.Cargo,
            Activo = usuario.Activo,
            OrigenAd = usuario.OrigenAd,
            TieneCredencial = !string.IsNullOrEmpty(usuario.PasswordHash),
            UltimoAcceso = usuario.UltimoAcceso,
            Roles = usuario.Roles
                .Select(ur => new UsuarioRolResponseDto { IdRol = ur.IdRol, Nombre = ur.Rol?.Nombre ?? string.Empty })
                .ToList(),
        };
    }
}
