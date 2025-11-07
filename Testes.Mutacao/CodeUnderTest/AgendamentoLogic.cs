using System;
using System.Collections.Generic;
using System.Linq;

namespace Testes.Mutacao.CodeUnderTest
{
    /// <summary>
    /// Lógica de negócio para agendamento de consultas
    /// Esta classe contém métodos complexos para testar mutações avançadas
    /// </summary>
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

        /// <summary>
        /// Valida se um agendamento é possível
        /// Testa mutações em operadores lógicos complexos
        /// </summary>
        public bool ValidarAgendamento(DateTime data, TimeSpan horario, string especialidade, int idadePaciente)
        {
            // Mutações possíveis: && → ||, >= → >, <= → <, == → !=
            return data >= DateTime.Today &&                    // Data não pode ser no passado
                   horario >= new TimeSpan(8, 0, 0) &&         // Horário mínimo 8h
                   horario <= new TimeSpan(18, 0, 0) &&        // Horário máximo 18h
                   !string.IsNullOrEmpty(especialidade) &&     // Especialidade obrigatória
                   _especialidadesDisponiveis.Contains(especialidade) && // Especialidade válida
                   idadePaciente >= 0 &&                       // Idade não pode ser negativa
                   idadePaciente <= 120;                       // Idade máxima razoável
        }

        /// <summary>
        /// Calcula o valor total da consulta com descontos
        /// Testa mutações em operadores aritméticos e condicionais
        /// </summary>
        public double CalcularValorTotal(string especialidade, int idadePaciente, bool temConvenio, int duracaoMinutos)
        {
            if (string.IsNullOrEmpty(especialidade) || !_valoresPorEspecialidade.ContainsKey(especialidade))
                throw new ArgumentException("Especialidade inválida");

            if (duracaoMinutos <= 0)
                throw new ArgumentException("Duração deve ser positiva");

            // Valor base da especialidade
            double valorBase = _valoresPorEspecialidade[especialidade];

            // Ajuste por duração (mutações: /, *, +, -)
            double fatorDuracao = duracaoMinutos / 30.0; // Base: 30 minutos
            double valorComDuracao = valorBase * fatorDuracao;

            // Desconto por idade (mutações: >=, <=, -, +)
            double desconto = 0.0;
            if (idadePaciente >= 65)        // Idoso: 20% desconto
                desconto = 0.20;
            else if (idadePaciente <= 12)   // Criança: 15% desconto
                desconto = 0.15;

            // Desconto por convênio (mutações: &&, ||, !)
            if (temConvenio && desconto > 0)
                desconto += 0.05; // 5% adicional se tem convênio e já tem desconto por idade
            else if (temConvenio)
                desconto = 0.10;  // 10% se só tem convênio

            // Aplicar desconto (mutações: -, +, *, /)
            double valorFinal = valorComDuracao - (valorComDuracao * desconto);

            // Valor mínimo (mutações: <, <=, >, >=)
            return valorFinal < 50.0 ? 50.0 : valorFinal;
        }

        /// <summary>
        /// Encontra próximos horários disponíveis
        /// Testa mutações em loops e coleções
        /// </summary>
        public List<TimeSpan> ObterHorariosDisponiveis(DateTime data, List<TimeSpan> horariosOcupados)
        {
            var horariosDisponiveis = new List<TimeSpan>();
            
            // Horários de funcionamento: 8h às 18h, intervalos de 30 minutos
            // Mutações possíveis: <=, <, >=, >, ++, +=, -=
            for (int hora = 8; hora <= 17; hora++)
            {
                for (int minuto = 0; minuto < 60; minuto += 30)
                {
                    var horario = new TimeSpan(hora, minuto, 0);
                    
                    // Verificar se não está ocupado (mutações: !, Contains)
                    if (!horariosOcupados.Contains(horario))
                    {
                        horariosDisponiveis.Add(horario);
                    }
                }
            }

            // Filtrar apenas horários futuros se for hoje (mutações: >, >=, <, <=)
            if (data.Date == DateTime.Today)
            {
                var agora = DateTime.Now.TimeOfDay;
                horariosDisponiveis = horariosDisponiveis
                    .Where(h => h > agora.Add(TimeSpan.FromMinutes(30))) // 30 min de antecedência
                    .ToList();
            }

            return horariosDisponiveis;
        }

        /// <summary>
        /// Valida conflitos de agendamento
        /// Testa mutações em comparações de data/hora
        /// </summary>
        public bool TemConflito(DateTime novaData, TimeSpan novoHorario, int novaDuracao,
                               DateTime dataExistente, TimeSpan horarioExistente, int duracaoExistente)
        {
            // Se são dias diferentes, não há conflito (mutações: ==, !=)
            if (novaData.Date != dataExistente.Date)
                return false;

            // Calcular intervalos de tempo (mutações: +, -, *, /)
            var novoInicio = novaData.Date.Add(novoHorario);
            var novoFim = novoInicio.AddMinutes(novaDuracao);
            
            var existenteInicio = dataExistente.Date.Add(horarioExistente);
            var existenteFim = existenteInicio.AddMinutes(duracaoExistente);

            // Verificar sobreposição (mutações: <, <=, >, >=, &&, ||)
            return novoInicio < existenteFim && novoFim > existenteInicio;
        }

        /// <summary>
        /// Calcula estatísticas de agendamento
        /// Testa mutações em operações de agregação
        /// </summary>
        public AgendamentoEstatisticas CalcularEstatisticas(List<ConsultaInfo> consultas)
        {
            if (consultas == null || consultas.Count == 0)
                return new AgendamentoEstatisticas();

            var estatisticas = new AgendamentoEstatisticas();

            // Total de consultas (mutações: Count, Length)
            estatisticas.TotalConsultas = consultas.Count;

            // Valor total (mutações: Sum, +, -)
            estatisticas.ValorTotal = consultas.Sum(c => c.Valor);

            // Valor médio (mutações: /, *, Average)
            estatisticas.ValorMedio = estatisticas.ValorTotal / estatisticas.TotalConsultas;

            // Duração média (mutações: Sum, /, Average)
            estatisticas.DuracaoMedia = consultas.Sum(c => c.DuracaoMinutos) / (double)consultas.Count;

            // Consultas por especialidade (mutações: GroupBy, Count)
            estatisticas.ConsultasPorEspecialidade = consultas
                .GroupBy(c => c.Especialidade)
                .ToDictionary(g => g.Key, g => g.Count());

            // Percentual com convênio (mutações: Where, Count, /, *)
            var comConvenio = consultas.Where(c => c.TemConvenio).Count();
            estatisticas.PercentualConvenio = (comConvenio * 100.0) / estatisticas.TotalConsultas;

            // Horário mais comum (mutações: GroupBy, OrderBy, First)
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

        /// <summary>
        /// Valida regras de negócio complexas
        /// Testa mutações em condicionais aninhadas
        /// </summary>
        public string ValidarRegrasNegocio(DateTime data, TimeSpan horario, string especialidade, 
                                         int idadePaciente, bool temConvenio, bool ehRetorno)
        {
            var erros = new List<string>();

            // Validação de data (mutações: <, <=, >, >=)
            if (data < DateTime.Today)
                erros.Add("Data não pode ser no passado");
            else if (data > DateTime.Today.AddDays(90))
                erros.Add("Agendamento máximo de 90 dias");

            // Validação de horário (mutações: <, <=, >, >=, &&, ||)
            if (horario < new TimeSpan(8, 0, 0) || horario > new TimeSpan(18, 0, 0))
                erros.Add("Horário deve ser entre 8h e 18h");

            // Validação de especialidade (mutações: IsNullOrEmpty, Contains, !)
            if (string.IsNullOrEmpty(especialidade))
                erros.Add("Especialidade é obrigatória");
            else if (!_especialidadesDisponiveis.Contains(especialidade))
                erros.Add("Especialidade não disponível");

            // Validação de idade (mutações: <, <=, >, >=)
            if (idadePaciente < 0)
                erros.Add("Idade não pode ser negativa");
            else if (idadePaciente > 120)
                erros.Add("Idade inválida");

            // Regras específicas por idade e especialidade (mutações: &&, ||, ==, !=)
            if (especialidade == "Pediatria" && idadePaciente > 16)
                erros.Add("Pediatria é para pacientes até 16 anos");
            else if (especialidade == "Geriatria" && idadePaciente < 60)
                erros.Add("Geriatria é para pacientes acima de 60 anos");

            // Regras de convênio (mutações: &&, ||, !)
            if (!temConvenio && !ehRetorno && _valoresPorEspecialidade[especialidade] > 200.0)
                erros.Add("Especialidade cara requer convênio para primeira consulta");

            // Retornar resultado (mutações: Count, ==, !=, >, <)
            return erros.Count == 0 ? "Válido" : string.Join("; ", erros);
        }
    }

    /// <summary>
    /// Classe para informações de consulta
    /// </summary>
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

    /// <summary>
    /// Classe para estatísticas de agendamento
    /// </summary>
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