using CadastroService.Domain.Entities.Medicos;
using CadastroService.Domain.Interfaces.Repositories;
using CadastroService.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CadastroService.Application.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly ILogger<MedicoService> _logger;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IEspecialidadeRepository _especialidadeRepository;

        public MedicoService(
            ILogger<MedicoService> logger,
            IMedicoRepository medicoRepository,
            IEspecialidadeRepository especialidadeRepository)
        {
            _logger = logger;
            _medicoRepository = medicoRepository;
            _especialidadeRepository = especialidadeRepository;
        }

        public async Task<List<Medico>> GetAll()
        {
            try
            {
                _logger.LogInformation("Buscando todos os médicos.");
                return await _medicoRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os médicos.");
                throw;
            }
        }

        public async Task<Medico?> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Buscando médico ID {Id}.", id);
                return await _medicoRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar médico ID {Id}.", id);
                throw;
            }
        }

        public async Task<Medico> Create(Medico medico)
        {
            try
            {
                _logger.LogInformation("Criando médico {Nome}.", medico.Nome);

                var especialidade = await _especialidadeRepository.GetById(medico.EspecialidadeId);
                if (especialidade == null || !especialidade.Ativo)
                    throw new Exception($"Especialidade ID {medico.EspecialidadeId} não encontrada ou inativa.");

                medico.Especialidade = especialidade;
                return await _medicoRepository.Create(medico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar médico {Nome}.", medico.Nome);
                throw;
            }
        }

        public async Task<Medico?> Update(Medico medico)
        {
            try
            {
                _logger.LogInformation("Atualizando médico ID {Id}.", medico.Id);

                var especialidade = await _especialidadeRepository.GetById(medico.EspecialidadeId);
                if (especialidade == null || !especialidade.Ativo)
                    throw new Exception($"Especialidade ID {medico.EspecialidadeId} não encontrada ou inativa.");

                medico.Especialidade = especialidade;

                var updated = await _medicoRepository.Update(medico);
                if (updated == null)
                    _logger.LogWarning("Médico ID {Id} não encontrado para atualização.", medico.Id);

                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar médico ID {Id}.", medico.Id);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deletando médico ID {Id}.", id);
                return await _medicoRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar médico ID {Id}.", id);
                throw;
            }
        }

        public async Task<List<Medico>> GetByEspecialidade(int especialidadeId)
        {
            try
            {
                _logger.LogInformation("Buscando médicos com especialidade ID {Id}.", especialidadeId);
                return await _medicoRepository.GetByEspecialidade(especialidadeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar médicos com especialidade ID {Id}.", especialidadeId);
                throw;
            }
        }
    }
}
