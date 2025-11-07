using CadastroService.Domain.Entities.Especialidades;

namespace CadastroService.Domain.Interfaces.Repositories
{
    public interface IEspecialidadeRepository
    {
        Task<Especialidade> Create(Especialidade especialidade);
        Task<List<Especialidade>> GetAll();
        Task<Especialidade?> GetById(int id);
        Task<Especialidade?> Update(Especialidade especialidade);
        Task<bool> Delete(int id);
    }
}
