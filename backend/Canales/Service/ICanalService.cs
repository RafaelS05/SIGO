using SIGO.Api.Canales.DTO;

namespace SIGO.Api.Canales.Service
{
    /// <summary>
    /// Application service for the Canales slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface ICanalService
    {
        Task<List<CanalResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<CanalResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<CanalResponseDto> CreateAsync(CanalCreateDto dto, CancellationToken cancellationToken = default);

        Task<CanalResponseDto?> UpdateAsync(int id, CanalUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
