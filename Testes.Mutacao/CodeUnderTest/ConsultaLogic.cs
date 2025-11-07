using System;

namespace Testes.Mutacao.CodeUnderTest
{
    /// <summary>
    /// Lógica de negócio para Consultas
    /// Esta classe contém métodos para validação e cálculos relacionados a consultas médicas
    /// </summary>
    public class ConsultaLogic
    {
        /// <summary>
        /// Valida se um horário de consulta está dentro do expediente
        /// Testa mutações em operadores relacionais
        /// </summary>
        public bool ValidarHorarioConsulta(TimeSpan horario)
        {
            // Mutações possíveis: >= → >, <= → <
            return horario >= new TimeSpan(8, 0, 0) && horario <= new TimeSpan(18, 0, 0);
        }

        /// <summary>
        /// Calcula a duração de uma consulta em minutos
        /// Testa mutações aritméticas
        /// </summary>
        public int CalcularDuracaoConsulta(TimeSpan inicio, TimeSpan fim)
        {
            if (fim <= inicio)
                throw new ArgumentException("Horário de fim deve ser posterior ao início");

            // Mutação possível: - → +
            var duracao = fim - inicio;
            return (int)duracao.TotalMinutes;
        }

        /// <summary>
        /// Valida se o intervalo entre consultas é adequado
        /// Testa mutações em constantes
        /// </summary>
        public bool ValidarIntervaloEntreConsultas(int intervaloMinutos)
        {
            // Mutação possível: >= → >, 15 → 14 ou 16
            return intervaloMinutos >= 15;
        }

        /// <summary>
        /// Verifica se um dia da semana é dia útil
        /// Testa mutações lógicas
        /// </summary>
        public bool EhDiaUtil(DayOfWeek diaSemana)
        {
            // Mutações possíveis: != → ==, || → &&
            return diaSemana != DayOfWeek.Saturday && diaSemana != DayOfWeek.Sunday;
        }

        /// <summary>
        /// Calcula o valor de uma consulta baseado na duração
        /// Testa mutações em cálculos complexos
        /// </summary>
        public double CalcularValorConsulta(int duracaoMinutos, double valorBase)
        {
            if (duracaoMinutos <= 0)
                throw new ArgumentException("Duração deve ser positiva");
            
            if (valorBase <= 0)
                throw new ArgumentException("Valor base deve ser positivo");

            // Mutações possíveis: <=, >, *, /
            if (duracaoMinutos <= 30)
                return valorBase;
            else if (duracaoMinutos <= 45)
                return valorBase + (valorBase * 0.1); // 10% adicional
            else
                return valorBase * 2.0;
        }

        /// <summary>
        /// Valida se uma data de consulta é válida
        /// Testa mutações em comparações de data
        /// </summary>
        public bool ValidarDataConsulta(DateTime data)
        {
            var hoje = DateTime.Today;
            var limiteMaximo = hoje.AddDays(90);

            // Mutações possíveis: >=, <=, <, >
            return data >= hoje && data <= limiteMaximo;
        }

        /// <summary>
        /// Obtém o próximo horário disponível
        /// Testa mutações em loops
        /// </summary>
        public TimeSpan ObterProximoHorarioDisponivel(TimeSpan[] horariosOcupados)
        {
            // Mutações possíveis: <=, ++, +=
            for (int hora = 8; hora <= 17; hora++)
            {
                for (int minuto = 0; minuto < 60; minuto += 30)
                {
                    var horario = new TimeSpan(hora, minuto, 0);
                    
                    // Mutação possível: ! → vazio
                    if (!Array.Exists(horariosOcupados, h => h == horario))
                    {
                        return horario;
                    }
                }
            }

            throw new InvalidOperationException("Não há horários disponíveis");
        }

        /// <summary>
        /// Valida uma especialidade médica
        /// Testa mutações em strings
        /// </summary>
        public bool ValidarEspecialidade(string especialidade)
        {
            if (string.IsNullOrEmpty(especialidade))
                return false;

            var especialidadesValidas = new[] 
            { 
                "Cardiologia", "Dermatologia", "Neurologia", 
                "Ortopedia", "Pediatria" 
            };

            // Mutação possível: Contains → !Contains
            return Array.Exists(especialidadesValidas, 
                e => string.Equals(e, especialidade, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Calcula tempo de espera baseado na prioridade
        /// Testa mutações em condicionais aninhadas
        /// </summary>
        public int CalcularTempoEspera(string prioridade)
        {
            // Mutações possíveis: == → !=, valores constantes
            if (string.IsNullOrEmpty(prioridade))
                return 30; // Padrão

            return prioridade.ToUpper() switch
            {
                "URGENTE" => 0,
                "NORMAL" => 30,
                "BAIXA" => 60,
                _ => 30
            };
        }

        /// <summary>
        /// Valida idade do paciente
        /// Testa mutações em ranges
        /// </summary>
        public bool ValidarIdadePaciente(int idade)
        {
            // Mutações possíveis: >=, <=, <, >
            return idade >= 0 && idade <= 120;
        }

        /// <summary>
        /// Calcula desconto baseado na idade
        /// Testa mutações em múltiplas condições
        /// </summary>
        public double CalcularDesconto(int idade)
        {
            // Mutações possíveis: >=, <=, valores de desconto
            if (idade >= 70)
                return 0.15; // 15% para idosos
            else if (idade <= 5)
                return 0.10; // 10% para crianças
            else
                return 0.0;  // Sem desconto
        }
    }
}