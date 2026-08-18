using SIGO.Api.Departamentos.DTO;

namespace SIGO.Api.Departamentos.Service
{
    /// <summary>
    /// Application service for the Departamentos slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface IDepartamentoService
    {
        Task<List<DepartamentoResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<DepartamentoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<DepartamentoResponseDto> CreateAsync(DepartamentoCreateDto dto, CancellationToken cancellationToken = default);

        Task<DepartamentoResponseDto?> UpdateAsync(int id, DepartamentoUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
