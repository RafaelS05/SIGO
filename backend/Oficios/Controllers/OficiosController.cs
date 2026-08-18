using Microsoft.AspNetCore.Mvc;
using SIGO.Api.Oficios.DTO;
using SIGO.Api.Oficios.Service;

namespace SIGO.Api.Oficios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OficiosController : ControllerBase
    {
        private readonly IOficioService _service;

        public OficiosController(IOficioService service)
        {
            _service = service;
        }

        // Bandejas y búsquedas: ?estadoId=&prioridadId=&departamentoId=&responsableId=&busqueda=&desde=&hasta=&atrasados=true
        [HttpGet]
        public async Task<ActionResult<List<OficioResponseDto>>> GetAll(
            [FromQuery] int? estadoId,
            [FromQuery] int? prioridadId,
            [FromQuery] int? departamentoId,
            [FromQuery] int? responsableId,
            [FromQuery] string? busqueda,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] bool atrasados,
            CancellationToken cancellationToken)
            => Ok(await _service.GetFilteredAsync(
                estadoId, prioridadId, departamentoId, responsableId,
                busqueda, desde, hasta, atrasados, cancellationToken));

        [HttpGet("{id}")]
        public async Task<ActionResult<OficioResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OficioResponseDto>> Create([FromBody] OficioCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OficioResponseDto>> Update(int id, [FromBody] OficioUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            => await _service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();

        [HttpPost("{id}/estado")]
        public async Task<ActionResult<OficioResponseDto>> CambiarEstado(int id, [FromBody] OficioCambioEstadoDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.CambiarEstadoAsync(id, dto, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("{id}/asignar")]
        public async Task<ActionResult<OficioResponseDto>> Asignar(int id, [FromBody] OficioAsignacionDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.AsignarAsync(id, dto, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("{id}/comentarios")]
        public async Task<ActionResult<OficioResponseDto>> Comentar(int id, [FromBody] OficioComentarioDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.ComentarAsync(id, dto, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("{id}/adjuntos")]
        public async Task<ActionResult<OficioResponseDto>> AgregarAdjunto(int id, IFormFile archivo, [FromForm] int subidoPor, CancellationToken cancellationToken)
        {
            if (archivo is null || archivo.Length == 0)
                return BadRequest("Debe adjuntar un archivo.");

            var result = await _service.AgregarAdjuntoAsync(id, archivo, subidoPor, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("{id}/adjuntos/{idAdjunto}")]
        public async Task<IActionResult> DescargarAdjunto(int id, int idAdjunto, CancellationToken cancellationToken)
        {
            var descarga = await _service.GetAdjuntoDescargaAsync(id, idAdjunto, cancellationToken);
            if (descarga is null || !System.IO.File.Exists(descarga.RutaAbsoluta))
                return NotFound();

            return PhysicalFile(descarga.RutaAbsoluta, descarga.ContentType, descarga.NombreArchivo);
        }
    }
}
