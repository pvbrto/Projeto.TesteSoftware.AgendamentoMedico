using System;
using System.Collections.Generic;
using System.Linq;

namespace Testes.Mutacao.CodeUnderTest
{
    public class AgendamentoLogic
    {
        private readonly List<string> _especialidadesDisponiveis = new()
        {
            "Cardiologia", "Dermatologia", "Neurologia", 
            "Ortopedia", "Pediatria", "Psiquiatria", "Geriatria"
        };

        private readonly Dictionary<string, double> _valoresPorEspecialidade = new()
        {
            { "Cardiologia", 200.0 },
            { "Dermatologia", 150.0 },
            { "Neurologia", 250.0 },
            { "Ortopedia", 180.0 },
            { "Pediatria", 120.0 },
            { "Psiquiatria", 220.0 },
            { "Geriatria", 180.0 }
        };

        public bool ValidarAgendamento(DateTime data, TimeSpan horario, string especialidade, int idadePaciente)
        {
            return data >= DateTime.Today &&
                   horario >= new TimeSpan(8, 0, 0) &&
                   horario <= new TimeSpan(18, 0, 0) &&
                   !string.IsNullOrEmpty(especialidade) &&
                   _especialidadesDisponiveis.Contains(especialidade) &&
                   idadePaciente >= 0 &&
                   idadePaciente <= 120;
        }

        public double CalcularValorTotal(string especialidade, int idadePaciente, bool temConvenio, int duracaoMinutos)
        {
            if (string.IsNullOrEmpty(especialidade) || !_valoresPorEspecialidade.ContainsKey(especialidade))
                throw new ArgumentException("Especialidade inválida");

            if (duracaoMinutos <= 0)
                throw new ArgumentException("Duração deve ser positiva");

            double valorBase = _valoresPorEspecialidade[especialidade];

            double fatorDuracao = duracaoMinutos / 30.0;
            double valorComDuracao = valorBase * fatorDuracao;

            double desconto = 0.0;
            if (idadePaciente >= 65)
                desconto = 0.20;
            else if (idadePaciente <= 12)
                desconto = 0.15;

            if (temConvenio && desconto > 0)
                desconto += 0.05;
            else if (temConvenio)
                desconto = 0.10;

            double valorFinal = valorComDuracao - (valorComDuracao * desconto);

            return valorFinal < 50.0 ? 50.0 : valorFinal;
        }

        public List<TimeSpan> ObterHorariosDisponiveis(DateTime data, List<TimeSpan> horariosOcupados)
        {
            var horariosDisponiveis = new List<TimeSpan>();
            
            for (int hora = 8; hora <= 17; hora++)
            {
                for (int minuto = 0; minuto < 60; minuto += 30)
                {
                    var horario = new TimeSpan(hora, minuto, 0);
                    
                    if (!horariosOcupados.Contains(horario))
                    {
                        horariosDisponiveis.Add(horario);
                    }
                }
            }

            if (data.Date == DateTime.Today)
            {
                var agora = DateTime.Now.TimeOfDay;
                horariosDisponiveis = horariosDisponiveis
                    .Where(h => h > agora.Add(TimeSpan.FromMinutes(30)))
                    .ToList();
            }

            return horariosDisponiveis;
        }

        public bool TemConflito(DateTime novaData, TimeSpan novoHorario, int novaDuracao,
                               DateTime dataExistente, TimeSpan horarioExistente, int duracaoExistente)
        {
            if (novaData.Date != dataExistente.Date)
                return false;

            var novoInicio = novaData.Date.Add(novoHorario);
            var novoFim = novoInicio.AddMinutes(novaDuracao);
            
            var existenteInicio = dataExistente.Date.Add(horarioExistente);
            var existenteFim = existenteInicio.AddMinutes(duracaoExistente);

            return novoInicio < existenteFim && novoFim > existenteInicio;
        }

        public AgendamentoEstatisticas CalcularEstatisticas(List<ConsultaInfo> consultas)
        {
            if (consultas == null || consultas.Count == 0)
                return new AgendamentoEstatisticas();

            var estatisticas = new AgendamentoEstatisticas();

            estatisticas.TotalConsultas = consultas.Count;

            estatisticas.ValorTotal = consultas.Sum(c => c.Valor);

            estatisticas.ValorMedio = estatisticas.ValorTotal / estatisticas.TotalConsultas;

            estatisticas.DuracaoMedia = consultas.Sum(c => c.DuracaoMinutos) / (double)consultas.Count;

            estatisticas.ConsultasPorEspecialidade = consultas
                .GroupBy(c => c.Especialidade)
                .ToDictionary(g => g.Key, g => g.Count());

            var comConvenio = consultas.Where(c => c.TemConvenio).Count();
            estatisticas.PercentualConvenio = (comConvenio * 100.0) / estatisticas.TotalConsultas;

            if (consultas.Any())
            {
                estatisticas.HorarioMaisComum = consultas
                    .GroupBy(c => c.Horario.Hours)
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;
            }

            return estatisticas;
        }

        public string ValidarRegrasNegocio(DateTime data, TimeSpan horario, string especialidade, 
                                         int idadePaciente, bool temConvenio, bool ehRetorno)
        {
            var erros = new List<string>();

            if (data < DateTime.Today)
                erros.Add("Data não pode ser no passado");
            else if (data > DateTime.Today.AddDays(90))
                erros.Add("Agendamento máximo de 90 dias");

            if (horario < new TimeSpan(8, 0, 0) || horario > new TimeSpan(18, 0, 0))
                erros.Add("Horário deve ser entre 8h e 18h");

            if (string.IsNullOrEmpty(especialidade))
                erros.Add("Especialidade é obrigatória");
            else if (!_especialidadesDisponiveis.Contains(especialidade))
                erros.Add("Especialidade não disponível");

            if (idadePaciente < 0)
                erros.Add("Idade não pode ser negativa");
            else if (idadePaciente > 120)
                erros.Add("Idade inválida");

            if (especialidade == "Pediatria" && idadePaciente > 16)
                erros.Add("Pediatria é para pacientes até 16 anos");
            else if (especialidade == "Geriatria" && idadePaciente < 60)
                erros.Add("Geriatria é para pacientes acima de 60 anos");

            if (!temConvenio && !ehRetorno && _valoresPorEspecialidade[especialidade] > 200.0)
                erros.Add("Especialidade cara requer convênio para primeira consulta");

            return erros.Count == 0 ? "Válido" : string.Join("; ", erros);
        }
    }

    public class ConsultaInfo
    {
        public DateTime Data { get; set; }
        public TimeSpan Horario { get; set; }
        public string Especialidade { get; set; } = string.Empty;
        public double Valor { get; set; }
        public int DuracaoMinutos { get; set; }
        public bool TemConvenio { get; set; }
        public int IdadePaciente { get; set; }
    }

    public class AgendamentoEstatisticas
    {
        public int TotalConsultas { get; set; }
        public double ValorTotal { get; set; }
        public double ValorMedio { get; set; }
        public double DuracaoMedia { get; set; }
        public Dictionary<string, int> ConsultasPorEspecialidade { get; set; } = new();
        public double PercentualConvenio { get; set; }
        public int HorarioMaisComum { get; set; }
    }
}
