using CadastroService.Domain.Entities.Pacientes;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CadastroService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PacienteController(IPacienteService _pacienteService) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var pacientes = await _pacienteService.GetAll();
            return Ok(pacientes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var paciente = await _pacienteService.GetById(id);
            if (paciente == null)
                return NotFound(new { message = $"Paciente com ID {id} não encontrado." });

            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Paciente paciente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _pacienteService.Create(paciente);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Paciente paciente)
        {
            if (id != paciente.Id)
                return BadRequest(new { message = "ID da URL não corresponde ao corpo da requisição." });

            var updated = await _pacienteService.Update(paciente);
            if (updated == null)
                return NotFound(new { message = $"Paciente com ID {id} não encontrado para atualização." });

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _pacienteService.Delete(id);
            if (!success)
                return NotFound(new { message = $"Paciente com ID {id} não encontrado para exclusão." });

            return Ok();
        }
    }
}
