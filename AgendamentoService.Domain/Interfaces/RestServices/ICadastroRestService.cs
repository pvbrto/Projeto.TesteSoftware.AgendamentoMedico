using AgendamentoService.Domain.Entities;

namespace AgendamentoService.Domain.Interfaces.RestServices
{
    public interface ICadastroRestService
    {
        Task<List<MedicoDTO>> GetMedicosByEspecialidade(int especialidadeId);
        Task<ClinicaDTO> GetClinicaById(int clinicaId);
        Task<MedicoDTO> GetMedicoById(int id);
        Task<PacienteDTO> GetPacienteById(int id);
    }
}
