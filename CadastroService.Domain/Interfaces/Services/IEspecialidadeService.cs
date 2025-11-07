using CadastroService.Domain.Entities.Especialidades;

namespace CadastroService.Domain.Interfaces.Services
{
    public interface IEspecialidadeService
    {
        Task<List<Especialidade>> GetAll();
        Task<Especialidade?> GetById(int id);
        Task<Especialidade> Create(Especialidade especialidade);
        Task<Especialidade?> Update(Especialidade especialidade);
        Task<bool> Delete(int id);
    }
}
