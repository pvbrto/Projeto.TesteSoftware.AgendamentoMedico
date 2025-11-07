using CadastroService.Domain.Entities.Especialidades;

namespace CadastroService.Domain.Entities.Medicos
{
    public class Medico
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int EspecialidadeId { get; set; }
        public Especialidade Especialidade { get; set; }
        public string CRM { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public bool Ativo { get; set; } = true;
    }
}
