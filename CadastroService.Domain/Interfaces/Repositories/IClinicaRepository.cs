using CadastroService.Domain.Entities.Clinicas;

namespace CadastroService.Domain.Interfaces.Repositories
{
    public interface IClinicaRepository
    {
        Task<List<Clinica>> GetAll();
        Task<Clinica> GetById(int id);
        Task<Clinica> Create(Clinica clinica);
        Task<Clinica> Update(Clinica clinica);
        Task<bool> Delete(int id);
    }
}
