using AgendamentoService.Domain.Entities.Consultas;

namespace AgendamentoService.Domain.Interfaces.Repositories
{
    public interface IConsultaRepository
    {
        Task<List<Consulta>> GetAll();
        Task<Consulta?> GetById(int id);
        Task<Consulta> Create(Consulta consulta);
        Task<Consulta?> Update(Consulta consulta);
        Task<bool> Delete(int id);
        Task<Consulta?> GetConsultaNoHorario(int medicoId, int clinicaId, DateTime dataHora);
    }
}
