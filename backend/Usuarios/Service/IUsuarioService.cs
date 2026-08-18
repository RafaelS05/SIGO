using SIGO.Api.Usuarios.DTO;

namespace SIGO.Api.Usuarios.Service
{
    /// <summary>
    /// Application service for the Usuarios slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface IUsuarioService
    {
        Task<List<UsuarioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<UsuarioResponseDto> CreateAsync(UsuarioCreateDto dto, CancellationToken cancellationToken = default);

        Task<UsuarioResponseDto?> UpdateAsync(int id, UsuarioUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
