using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGO.Api.Auth;
using SIGO.Api.Canales.DTO;
using SIGO.Api.Canales.Service;

namespace SIGO.Api.Canales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanalesController : ControllerBase
    {
        private readonly ICanalService _service;

        public CanalesController(ICanalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<CanalResponseDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await _service.GetAllAsync(cancellationToken));

        [HttpGet("{id}")]
        public async Task<ActionResult<CanalResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        // Catálogos: lectura para todo usuario autenticado; escritura solo Administrador (RBAC).
        [HttpPost]
        [Authorize(Policy = Permisos.AdministrarCatalogos)]
        public async Task<ActionResult<CanalResponseDto>> Create([FromBody] CanalCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permisos.AdministrarCatalogos)]
        public async Task<ActionResult<CanalResponseDto>> Update(int id, [FromBody] CanalUpdateDto dto, CancellationToken cancellationToken)
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
