using AgendamentoService.Domain.Enums;

namespace AgendamentoService.Domain.Entities.Consultas
{
    public class Consulta
    {
        public int Id { get; set; }

        public int PacienteId { get; set; }
        public PacienteDTO Paciente { get; set; }

        public int MedicoId { get; set; }
        public MedicoDTO Medico { get; set; }

        public int ClinicaId { get; set; }
        public ClinicaDTO Clinica { get; set; }

        public DateTime DataHora { get; set; }
        public string Observacoes { get; set; }
        public StatusAgendamento Status { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public bool Ativo { get; set; } = true;

        public Consulta()
        {
        }

        public bool PodeSerEncerrado()
        {
            return Status == StatusAgendamento.EmConsulta || Status == StatusAgendamento.Agendado;
        }
    }
}
