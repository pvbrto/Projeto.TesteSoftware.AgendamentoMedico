using CadastroService.Domain.Entities.Pacientes;
using CadastroService.Domain.Interfaces.Repositories;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CadastroService.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly ILogger<PacienteService> _logger;
        private readonly IPacienteRepository _pacienteRepository;

        public PacienteService(
            ILogger<PacienteService> logger,
            IPacienteRepository pacienteRepository)
        {
            _logger = logger;
            _pacienteRepository = pacienteRepository;
        }

        public async Task<List<Paciente>> GetAll()
        {
            try
            {
                _logger.LogInformation("Buscando todos os pacientes.");
                return await _pacienteRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os pacientes.");
                throw;
            }
        }

        public async Task<Paciente?> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Buscando paciente com ID {Id}.", id);
                var paciente = await _pacienteRepository.GetById(id);

                if (paciente == null)
                {
                    _logger.LogWarning("Paciente com ID {Id} não encontrado.", id);
                }

                return paciente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar paciente com ID {Id}.", id);
                throw;
            }
        }

        public async Task<Paciente> Create(Paciente paciente)
        {
            try
            {
                _logger.LogInformation("Criando novo paciente: {Nome}.", paciente.Nome);
                return await _pacienteRepository.Create(paciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar paciente {Nome}.", paciente.Nome);
                throw;
            }
        }

        public async Task<Paciente?> Update(Paciente paciente)
        {
            try
            {
                _logger.LogInformation("Atualizando paciente ID {Id}.", paciente.Id);
                var updated = await _pacienteRepository.Update(paciente);

                if (updated == null)
                {
                    _logger.LogWarning("Paciente com ID {Id} não encontrado para atualização.", paciente.Id);
                }

                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar paciente ID {Id}.", paciente.Id);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deletando logicamente paciente ID {Id}.", id);
                var success = await _pacienteRepository.Delete(id);

                if (!success)
                {
                    _logger.LogWarning("Falha ao deletar logicamente paciente ID {Id}. Ele pode não existir.", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar logicamente paciente ID {Id}.", id);
                throw;
            }
        }
    }
}
