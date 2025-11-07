using CadastroService.Domain.Entities.Especialidades;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CadastroService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EspecialidadeController : ControllerBase
    {
        private readonly IEspecialidadeService _especialidadeService;

        public EspecialidadeController(IEspecialidadeService especialidadeService)
        {
            _especialidadeService = especialidadeService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var especialidades = await _especialidadeService.GetAll();
            return Ok(especialidades);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var especialidade = await _especialidadeService.GetById(id);
            if (especialidade == null)
                return NotFound(new { message = $"Especialidade com ID {id} não encontrada." });

            return Ok(especialidade);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Especialidade especialidade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _especialidadeService.Create(especialidade);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Especialidade especialidade)
        {
            if (id != especialidade.Id)
                return BadRequest(new { message = "ID da URL não corresponde ao corpo da requisição." });

            var updated = await _especialidadeService.Update(especialidade);
            if (updated == null)
                return NotFound(new { message = $"Especialidade com ID {id} não encontrada para atualização." });

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _especialidadeService.Delete(id);
            if (!success)
                return NotFound(new { message = $"Especialidade com ID {id} não encontrada para exclusão." });

            return NoContent();
        }
    }
}
