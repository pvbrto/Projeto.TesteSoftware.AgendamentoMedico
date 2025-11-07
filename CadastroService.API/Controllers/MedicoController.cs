using CadastroService.Domain.Entities.Medicos;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CadastroService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _medicoService;

        public MedicoController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var medicos = await _medicoService.GetAll();
            return Ok(medicos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medico = await _medicoService.GetById(id);
            if (medico == null)
                return NotFound(new { message = $"Médico com ID {id} não encontrado." });

            return Ok(medico);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Medico medico)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _medicoService.Create(medico);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Medico medico)
        {
            if (id != medico.Id)
                return BadRequest(new { message = "ID da URL não corresponde ao corpo da requisição." });

            try
            {
                var updated = await _medicoService.Update(medico);
                if (updated == null)
                    return NotFound(new { message = $"Médico com ID {id} não encontrado." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _medicoService.Delete(id);
            if (!success)
                return NotFound(new { message = $"Médico com ID {id} não encontrado para exclusão." });

            return NoContent();
        }

        [HttpGet("GetByEspecialidade/{especialidadeId:int}")]
        public async Task<IActionResult> GetByEspecialidade(int especialidadeId)
        {
            var medicos = await _medicoService.GetByEspecialidade(especialidadeId);
            return Ok(medicos);
        }
    }
}
