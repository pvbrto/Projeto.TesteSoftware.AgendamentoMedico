using CadastroService.Domain.Entities.Medicos;

namespace CadastroService.Domain.Interfaces.Repositories
{
    public interface IMedicoRepository
    {
        Task<Medico> Create(Medico medico);
        Task<List<Medico>> GetAll();
        Task<Medico?> GetById(int id);
        Task<Medico?> Update(Medico medico);
        Task<bool> Delete(int id);
        Task<List<Medico>> GetByEspecialidade(int especialidadeId);
    }
}
