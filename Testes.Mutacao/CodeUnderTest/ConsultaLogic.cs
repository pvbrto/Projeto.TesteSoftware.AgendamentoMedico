using System;

namespace Testes.Mutacao.CodeUnderTest
{
    public class ConsultaLogic
    {
        public bool ValidarHorarioConsulta(TimeSpan horario)
        {
            return horario >= new TimeSpan(8, 0, 0) && horario <= new TimeSpan(18, 0, 0);
        }

        public int CalcularDuracaoConsulta(TimeSpan inicio, TimeSpan fim)
        {
            if (fim <= inicio)
                throw new ArgumentException("Horário de fim deve ser posterior ao início");

            var duracao = fim - inicio;
            return (int)duracao.TotalMinutes;
        }

        public bool ValidarIntervaloEntreConsultas(int intervaloMinutos)
        {
            return intervaloMinutos >= 15;
        }

        public bool EhDiaUtil(DayOfWeek diaSemana)
        {
            return diaSemana != DayOfWeek.Saturday && diaSemana != DayOfWeek.Sunday;
        }

        public double CalcularValorConsulta(int duracaoMinutos, double valorBase)
        {
            if (duracaoMinutos <= 0)
                throw new ArgumentException("Duração deve ser positiva");
            
            if (valorBase <= 0)
                throw new ArgumentException("Valor base deve ser positivo");

            if (duracaoMinutos <= 30)
                return valorBase;
            else if (duracaoMinutos <= 45)
                return valorBase + (valorBase * 0.1);
            else
                return valorBase * 2.0;
        }

        public bool ValidarDataConsulta(DateTime data)
        {
            var hoje = DateTime.Today;
            var limiteMaximo = hoje.AddDays(90);

            return data >= hoje && data <= limiteMaximo;
        }

        public TimeSpan ObterProximoHorarioDisponivel(TimeSpan[] horariosOcupados)
        {
            for (int hora = 8; hora <= 17; hora++)
            {
                for (int minuto = 0; minuto < 60; minuto += 30)
                {
                    var horario = new TimeSpan(hora, minuto, 0);
                    
                    if (!Array.Exists(horariosOcupados, h => h == horario))
                    {
                        return horario;
                    }
                }
            }

            throw new InvalidOperationException("Não há horários disponíveis");
        }

        public bool ValidarEspecialidade(string especialidade)
        {
            if (string.IsNullOrEmpty(especialidade))
                return false;

            var especialidadesValidas = new[] 
            { 
                "Cardiologia", "Dermatologia", "Neurologia", 
                "Ortopedia", "Pediatria" 
            };

            return Array.Exists(especialidadesValidas, 
                e => string.Equals(e, especialidade, StringComparison.OrdinalIgnoreCase));
        }

        public int CalcularTempoEspera(string prioridade)
        {
            if (string.IsNullOrEmpty(prioridade))
                return 30;

            return prioridade.ToUpper() switch
            {
                "URGENTE" => 0,
                "NORMAL" => 30,
                "BAIXA" => 60,
                _ => 30
            };
        }

        public bool ValidarIdadePaciente(int idade)
        {
            return idade >= 0 && idade <= 120;
        }

        public double CalcularDesconto(int idade)
        {
            if (idade >= 70)
                return 0.15;
            else if (idade <= 5)
                return 0.10;
            else
                return 0.0;
        }
    }
}
