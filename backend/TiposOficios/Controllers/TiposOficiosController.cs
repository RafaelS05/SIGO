using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGO.Api.Auth;
using SIGO.Api.TiposOficios.DTO;
using SIGO.Api.TiposOficios.Service;

namespace SIGO.Api.TiposOficios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposOficiosController : ControllerBase
    {
        private readonly ITipoOficioService _service;

        public TiposOficiosController(ITipoOficioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TipoOficioResponseDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await _service.GetAllAsync(cancellationToken));

        [HttpGet("{id}")]
        public async Task<ActionResult<TipoOficioResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        // Catálogos: lectura para todo usuario autenticado; escritura solo Administrador (RBAC).
        [HttpPost]
        [Authorize(Policy = Permisos.AdministrarCatalogos)]
        public async Task<ActionResult<TipoOficioResponseDto>> Create([FromBody] TipoOficioCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permisos.AdministrarCatalogos)]
        public async Task<ActionResult<TipoOficioResponseDto>> Update(int id, [FromBody] TipoOficioUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permisos.AdministrarCatalogos)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            => await _service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
