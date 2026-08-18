using SIGO.Api.Estados.DTO;

namespace SIGO.Api.Estados.Service
{
    /// <summary>
    /// Application service for the Estados slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface IEstadoService
    {
        Task<List<EstadoResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<EstadoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<EstadoResponseDto> CreateAsync(EstadoCreateDto dto, CancellationToken cancellationToken = default);

        Task<EstadoResponseDto?> UpdateAsync(int id, EstadoUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
