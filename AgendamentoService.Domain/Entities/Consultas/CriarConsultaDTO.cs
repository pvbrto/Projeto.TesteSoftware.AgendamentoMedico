namespace AgendamentoService.Domain.Entities.Consultas
{
    public class CriarConsultaDTO
    {
        public int PacienteId { get; set; }
        public int ClinicaId { get; set; }
        public int MedicoId { get; set; }
        public DateTime DataHora { get; set; }
    }
}
