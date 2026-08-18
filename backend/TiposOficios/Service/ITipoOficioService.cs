using SIGO.Api.TiposOficios.DTO;

namespace SIGO.Api.TiposOficios.Service
{
    /// <summary>
    /// Application service for the TiposOficios slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface ITipoOficioService
    {
        Task<List<TipoOficioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TipoOficioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<TipoOficioResponseDto> CreateAsync(TipoOficioCreateDto dto, CancellationToken cancellationToken = default);

        Task<TipoOficioResponseDto?> UpdateAsync(int id, TipoOficioUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
