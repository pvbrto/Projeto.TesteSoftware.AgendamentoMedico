using AgendamentoService.Domain.Entities.Consultas;
using AgendamentoService.Domain.Entities.Exceptions;
using AgendamentoService.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgendamentoService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsultaController(IConsultaService _consultaService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriarConsultaDTO dto)
        {
            try
            {
                var result = await _consultaService.Create(dto);
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                if (ex.Code == HttpStatusCode.NotFound)
                    return NotFound(ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Realizar/{id:int}")]
        public async Task<IActionResult> Realizar([FromRoute] int id, [FromBody] string observacoes)
        {
            try
            {
                var result = await _consultaService.RealizarConsulta(id, observacoes);
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                if (ex.Code == HttpStatusCode.NotFound)
                    return NotFound(ex.Message);
                else if (ex.Code == HttpStatusCode.UnprocessableEntity)
                    return UnprocessableEntity(ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Filtro")]
        public async Task<IActionResult> Filtro([FromQuery] ConsultaFiltroDTO filtro)
        {
            try
            {
                var result = await _consultaService.GetFiltro(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _consultaService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok();
        }
    }
}
