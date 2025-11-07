using CadastroService.Domain.Entities.Especialidades;
using CadastroService.Domain.Interfaces.Repositories;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CadastroService.Application.Services
{
    public class EspecialidadeService : IEspecialidadeService
    {
        private readonly ILogger<EspecialidadeService> _logger;
        private readonly IEspecialidadeRepository _especialidadeRepository;

        public EspecialidadeService(
            ILogger<EspecialidadeService> logger,
            IEspecialidadeRepository especialidadeRepository)
        {
            _logger = logger;
            _especialidadeRepository = especialidadeRepository;
        }

        public async Task<List<Especialidade>> GetAll()
        {
            try
            {
                _logger.LogInformation("Buscando todas as especialidades.");
                return await _especialidadeRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as especialidades.");
                throw;
            }
        }

        public async Task<Especialidade?> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Buscando especialidade com ID {Id}.", id);
                var especialidade = await _especialidadeRepository.GetById(id);

                if (especialidade == null)
                {
                    _logger.LogWarning("Especialidade com ID {Id} não encontrada.", id);
                }

                return especialidade;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar especialidade com ID {Id}.", id);
                throw;
            }
        }

        public async Task<Especialidade> Create(Especialidade especialidade)
        {
            try
            {
                _logger.LogInformation("Criando nova especialidade: {Nome}.", especialidade.Nome);
                return await _especialidadeRepository.Create(especialidade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar especialidade {Nome}.", especialidade.Nome);
                throw;
            }
        }

        public async Task<Especialidade?> Update(Especialidade especialidade)
        {
            try
            {
                _logger.LogInformation("Atualizando especialidade ID {Id}.", especialidade.Id);
                var updated = await _especialidadeRepository.Update(especialidade);

                if (updated == null)
                {
                    _logger.LogWarning("Especialidade com ID {Id} não encontrada para atualização.", especialidade.Id);
                }

                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar especialidade ID {Id}.", especialidade.Id);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deletando logicamente especialidade ID {Id}.", id);
                var success = await _especialidadeRepository.Delete(id);

                if (!success)
                {
                    _logger.LogWarning("Falha ao deletar logicamente especialidade ID {Id}. Ela pode não existir.", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar logicamente especialidade ID {Id}.", id);
                throw;
            }
        }
    }
}
