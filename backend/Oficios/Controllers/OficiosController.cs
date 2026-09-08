using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGO.Api.Auth;
using SIGO.Api.Oficios.DTO;
using SIGO.Api.Oficios.Service;

namespace SIGO.Api.Oficios.Controllers
{
    /// <summary>
    /// RBAC: cada acción declara la policy de <see cref="Permisos"/> que exige. Además, las
    /// lecturas de quien no tiene <see cref="Permisos.VerTodosLosOficios"/> se acotan a su
    /// departamento o a los oficios donde figura como responsable (alcance de datos).
    /// </summary>
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
        {
            if (!User.TienePermiso(Permisos.VerTodosLosOficios))
            {
                var miId = User.GetUsuarioId();
                var miDepto = User.GetDepartamentoId();

                // "Mi bandeja" solo sobre uno mismo; cualquier otra consulta queda acotada al propio departamento.
                if (responsableId.HasValue && responsableId.Value != miId)
                    return Forbid();
                if (!responsableId.HasValue)
                {
                    if (departamentoId.HasValue && departamentoId.Value != miDepto)
                        return Forbid();
                    departamentoId = miDepto;
                }
            }

            return Ok(await _service.GetFilteredAsync(
                estadoId, prioridadId, departamentoId, responsableId,
                busqueda, desde, hasta, atrasados, cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OficioResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound();
            return PuedeVer(result) ? Ok(result) : Forbid();
        }

        [HttpPost]
        [Authorize(Policy = Permisos.CrearOficios)]
        public async Task<ActionResult<OficioResponseDto>> Create([FromBody] OficioCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, User.GetUsuarioId(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permisos.EditarOficios)]
        public async Task<ActionResult<OficioResponseDto>> Update(int id, [FromBody] OficioUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!await PuedeVerAsync(id, cancellationToken))
                return Forbid();
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permisos.EliminarOficios)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            => await _service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();

        [HttpPost("{id}/estado")]
        [Authorize(Policy = Permisos.TramitarOficios)]
        public async Task<ActionResult<OficioResponseDto>> CambiarEstado(int id, [FromBody] OficioCambioEstadoDto dto, CancellationToken cancellationToken)
        {
            if (!await PuedeVerAsync(id, cancellationToken))
                return Forbid();
            var result = await _service.CambiarEstadoAsync(id, dto, User.GetUsuarioId(), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("{id}/asignar")]
        [Authorize(Policy = Permisos.AsignarOficios)]
        public async Task<ActionResult<OficioResponseDto>> Asignar(int id, [FromBody] OficioAsignacionDto dto, CancellationToken cancellationToken)
        {
            if (!await PuedeVerAsync(id, cancellationToken))
                return Forbid();
            var result = await _service.AsignarAsync(id, dto, User.GetUsuarioId(), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("{id}/comentarios")]
        [Authorize(Policy = Permisos.TramitarOficios)]
        public async Task<ActionResult<OficioResponseDto>> Comentar(int id, [FromBody] OficioComentarioDto dto, CancellationToken cancellationToken)
        {
            if (!await PuedeVerAsync(id, cancellationToken))
                return Forbid();
            var result = await _service.ComentarAsync(id, dto, User.GetUsuarioId(), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("{id}/adjuntos")]
        [Authorize(Policy = Permisos.TramitarOficios)]
        public async Task<ActionResult<OficioResponseDto>> AgregarAdjunto(int id, IFormFile archivo, CancellationToken cancellationToken)
        {
            if (archivo is null || archivo.Length == 0)
                return BadRequest(new { title = "Debe adjuntar un archivo." });
            if (!await PuedeVerAsync(id, cancellationToken))
                return Forbid();

            var result = await _service.AgregarAdjuntoAsync(id, archivo, User.GetUsuarioId(), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("{id}/adjuntos/{idAdjunto}")]
        public async Task<IActionResult> DescargarAdjunto(int id, int idAdjunto, CancellationToken cancellationToken)
        {
            if (!await PuedeVerAsync(id, cancellationToken))
                return Forbid();

            var descarga = await _service.GetAdjuntoDescargaAsync(id, idAdjunto, cancellationToken);
            if (descarga is null || !System.IO.File.Exists(descarga.RutaAbsoluta))
                return NotFound();

            return PhysicalFile(descarga.RutaAbsoluta, descarga.ContentType, descarga.NombreArchivo);
        }

        // ---- Alcance de datos ----

        private bool PuedeVer(OficioResponseDto oficio)
        {
            if (User.TienePermiso(Permisos.VerTodosLosOficios))
                return true;

            var miId = User.GetUsuarioId();
            var miDepto = User.GetDepartamentoId();
            return oficio.IdDepartamentoDestino == miDepto
                || oficio.IdDepartamentoOrigen == miDepto
                || oficio.CreadoPor == miId
                || oficio.Responsables.Any(r => r.IdUsuario == miId);
        }

        private async Task<bool> PuedeVerAsync(int id, CancellationToken cancellationToken)
        {
            if (User.TienePermiso(Permisos.VerTodosLosOficios))
                return true;
            var oficio = await _service.GetByIdAsync(id, cancellationToken);
            return oficio is null || PuedeVer(oficio); // null → el action responde 404
        }
    }
}
