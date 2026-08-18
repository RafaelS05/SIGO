using SIGO.Api.Prioridades.DTO;

namespace SIGO.Api.Prioridades.Service
{
    /// <summary>
    /// Application service for the Prioridades slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface IPrioridadService
    {
        Task<List<PrioridadResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PrioridadResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<PrioridadResponseDto> CreateAsync(PrioridadCreateDto dto, CancellationToken cancellationToken = default);

        Task<PrioridadResponseDto?> UpdateAsync(int id, PrioridadUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
