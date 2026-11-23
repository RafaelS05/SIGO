using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGO.Api.Auth;
using SIGO.Api.Departamentos.DTO;
using SIGO.Api.Departamentos.Service;

namespace SIGO.Api.Departamentos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentosController : ControllerBase
    {
        private readonly IDepartamentoService _service;

        public DepartamentosController(IDepartamentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartamentoResponseDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await _service.GetAllAsync(cancellationToken));

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartamentoResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        // Catálogos: lectura para todo usuario autenticado; escritura solo Administrador (RBAC).
        [HttpPost]
        [Authorize(Policy = Permisos.AdministrarCatalogos)]
        public async Task<ActionResult<DepartamentoResponseDto>> Create([FromBody] DepartamentoCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permisos.AdministrarCatalogos)]
        public async Task<ActionResult<DepartamentoResponseDto>> Update(int id, [FromBody] DepartamentoUpdateDto dto, CancellationToken cancellationToken)
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
