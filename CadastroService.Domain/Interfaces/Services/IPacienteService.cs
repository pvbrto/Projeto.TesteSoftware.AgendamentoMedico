using CadastroService.Domain.Entities.Pacientes;

namespace CadastroService.Domain.Interfaces.Services
{
    public interface IPacienteService
    {
        Task<List<Paciente>> GetAll();
        Task<Paciente?> GetById(int id);
        Task<Paciente> Create(Paciente paciente);
        Task<Paciente?> Update(Paciente paciente);
        Task<bool> Delete(int id);
    }
}
