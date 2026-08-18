using SIGO.Api.Oficios.DTO;

namespace SIGO.Api.Oficios.Service
{
    /// <summary>
    /// Application service for the Oficios slice. Speaks DTOs to the outside,
    /// entities to the repositories, and owns the transaction (calls SaveChangesAsync).
    /// </summary>
    public interface IOficioService
    {
        Task<List<OficioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Bandejas: Mi bandeja (responsableId), Departamento, Atrasados, búsquedas y filtros.
        Task<List<OficioResponseDto>> GetFilteredAsync(
            int? estadoId,
            int? prioridadId,
            int? departamentoId,
            int? responsableId,
            string? busqueda,
            DateTime? desde,
            DateTime? hasta,
            bool soloAtrasados,
            CancellationToken cancellationToken = default);

        Task<OficioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<OficioResponseDto> CreateAsync(OficioCreateDto dto, CancellationToken cancellationToken = default);

        Task<OficioResponseDto?> UpdateAsync(int id, OficioUpdateDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<OficioResponseDto?> CambiarEstadoAsync(int id, OficioCambioEstadoDto dto, CancellationToken cancellationToken = default);

        Task<OficioResponseDto?> AsignarAsync(int id, OficioAsignacionDto dto, CancellationToken cancellationToken = default);

        Task<OficioResponseDto?> ComentarAsync(int id, OficioComentarioDto dto, CancellationToken cancellationToken = default);

        Task<OficioResponseDto?> AgregarAdjuntoAsync(int id, IFormFile archivo, int subidoPor, CancellationToken cancellationToken = default);

        Task<OficioAdjuntoDescargaDto?> GetAdjuntoDescargaAsync(int id, int idAdjunto, CancellationToken cancellationToken = default);
    }
}
