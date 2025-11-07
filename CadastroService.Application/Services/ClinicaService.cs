using CadastroService.Domain.Entities.Clinicas;
using CadastroService.Domain.Interfaces.Repositories;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CadastroService.Application.Services
{
    public class ClinicaService(
        ILogger<ClinicaService> _logger,
        IClinicaRepository _clinicaRepository) : IClinicaService
    {
        public async Task<List<Clinica>> GetAll()
        {
            try
            {
                _logger.LogInformation("Buscando todas as clínicas.");
                return await _clinicaRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as clínicas.");
                throw;
            }
        }

        public async Task<Clinica> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Buscando clínica com ID {Id}.", id);
                var clinica = await _clinicaRepository.GetById(id);

                if (clinica == null)
                {
                    _logger.LogWarning("Clínica com ID {Id} não encontrada.", id);
                }

                return clinica;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clínica com ID {Id}.", id);
                throw;
            }
        }

        public async Task<Clinica> Create(Clinica clinica)
        {
            try
            {
                _logger.LogInformation("Criando nova clínica: {Nome}.", clinica.Nome);
                return await _clinicaRepository.Create(clinica);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar clínica {Nome}.", clinica.Nome);
                throw;
            }
        }

        public async Task<Clinica> Update(Clinica clinica)
        {
            try
            {
                _logger.LogInformation("Atualizando clínica ID {Id}.", clinica.Id);
                var updated = await _clinicaRepository.Update(clinica);

                if (updated == null)
                {
                    _logger.LogWarning("Clínica com ID {Id} não encontrada para atualização.", clinica.Id);
                }

                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar clínica ID {Id}.", clinica.Id);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deletando logicamente clínica ID {Id}.", id);
                var success = await _clinicaRepository.Delete(id);

                if (!success)
                {
                    _logger.LogWarning("Falha ao deletar logicamente clínica ID {Id}. Ela pode não existir.", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar logicamente clínica ID {Id}.", id);
                throw;
            }
        }
    }
}
