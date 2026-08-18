using Microsoft.AspNetCore.Mvc;
using SIGO.Api.Estados.DTO;
using SIGO.Api.Estados.Service;

namespace SIGO.Api.Estados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosController : ControllerBase
    {
        private readonly IEstadoService _service;

        public EstadosController(IEstadoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<EstadoResponseDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await _service.GetAllAsync(cancellationToken));

        [HttpGet("{id}")]
        public async Task<ActionResult<EstadoResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EstadoResponseDto>> Create([FromBody] EstadoCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EstadoResponseDto>> Update(int id, [FromBody] EstadoUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            => await _service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
