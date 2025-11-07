using AgendamentoService.Domain.Enums;

namespace AgendamentoService.Domain.Entities.Consultas
{
    public class ConsultaFiltroDTO
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool Encerradas { get; set; }
        public StatusAgendamento StatusConsulta { get; set; }
        public int? ClinicaId { get; set; }
        public int? PacienteId { get; set; }
        public int? MedicoId { get; set; }
    }
}
