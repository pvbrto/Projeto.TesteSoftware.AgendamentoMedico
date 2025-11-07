using CadastroService.Domain.Entities.Clinicas;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CadastroService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClinicaController(IClinicaService _clinicaService) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var clinicas = await _clinicaService.GetAll();
            return Ok(clinicas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var clinica = await _clinicaService.GetById(id);
            if (clinica == null)
                return NotFound(new { message = $"Clínica com ID {id} não encontrada." });

            return Ok(clinica);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Clinica clinica)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _clinicaService.Create(clinica);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Clinica clinica)
        {
            if (id != clinica.Id)
                return BadRequest(new { message = "ID da URL não corresponde ao corpo da requisição." });

            var updated = await _clinicaService.Update(clinica);
            if (updated == null)
                return NotFound(new { message = $"Clínica com ID {id} não encontrada para atualização." });

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _clinicaService.Delete(id);
            if (!success)
                return NotFound(new { message = $"Clínica com ID {id} não encontrada para exclusão." });

            return Ok();
        }
    }
}
