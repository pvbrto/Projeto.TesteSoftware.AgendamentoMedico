using CadastroService.Domain.Entities.Medicos;

namespace CadastroService.Domain.Interfaces.Services
{
    public interface IMedicoService
    {
        Task<List<Medico>> GetAll();
        Task<Medico?> GetById(int id);
        Task<Medico> Create(Medico medico);
        Task<Medico?> Update(Medico medico);
        Task<bool> Delete(int id);
        Task<List<Medico>> GetByEspecialidade(int especialidadeId);
    }
}
