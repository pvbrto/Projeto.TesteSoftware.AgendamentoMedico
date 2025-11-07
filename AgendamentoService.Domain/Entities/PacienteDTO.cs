namespace AgendamentoService.Domain.Entities
{
    public class PacienteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
