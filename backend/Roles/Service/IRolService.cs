using SIGO.Api.Roles.DTO;

namespace SIGO.Api.Roles.Service
{
    /// <summary>
    /// Application service for the Roles slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface IRolService
    {
        Task<List<RolResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<RolResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<RolResponseDto> CreateAsync(RolCreateDto dto, CancellationToken cancellationToken = default);

        Task<RolResponseDto?> UpdateAsync(int id, RolUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
