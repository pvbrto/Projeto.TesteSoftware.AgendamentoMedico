using CadastroService.Domain.Entities.Pacientes;

namespace CadastroService.Domain.Interfaces.Repositories
{
    public interface IPacienteRepository
    {
        Task<Paciente> Create(Paciente paciente);
        Task<List<Paciente>> GetAll();
        Task<Paciente?> GetById(int id);
        Task<Paciente?> Update(Paciente paciente);
        Task<bool> Delete(int id);
    }
}
