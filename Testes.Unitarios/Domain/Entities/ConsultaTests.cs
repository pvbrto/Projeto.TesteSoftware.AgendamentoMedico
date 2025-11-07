using FluentAssertions;
using AutoFixture;
using Xunit;

namespace Testes.Unitarios.Domain.Entities
{
    /// <summary>
    /// Testes de caixa branca para a entidade Consulta
    /// Foca na lógica de agendamento, validações de horário e regras de negócio
    /// </summary>
    [Trait("Category", "Domain")]
    [Trait("Type", "Unit")]
    public class ConsultaTests
    {
        private readonly Fixture _fixture;

        public ConsultaTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Construtor_DeveInicializarPropriedadesCorretamente()
        {
            // Arrange
            var pacienteId = 1;
            var medicoId = 2;
            var dataHora = DateTime.Now.AddDays(1);
            var observacoes = "Consulta de rotina";

            // Act
            var consulta = new Consulta
            {
                PacienteId = pacienteId,
                MedicoId = medicoId,
                DataHora = dataHora,
                Observacoes = observacoes,
                Status = StatusConsulta.Agendada
            };

            // Assert
            consulta.PacienteId.Should().Be(pacienteId);
            consulta.MedicoId.Should().Be(medicoId);
            consulta.DataHora.Should().Be(dataHora);
            consulta.Observacoes.Should().Be(observacoes);
            consulta.Status.Should().Be(StatusConsulta.Agendada);
        }

        [Fact]
        public void ValidarDataFutura_DeveRetornarTrue_QuandoDataNoFuturo()
        {
            // Arrange
            var dataFutura = DateTime.Now.AddDays(1);
            var consulta = new Consulta { DataHora = dataFutura };

            // Act
            var resultado = consulta.ValidarDataFutura();

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void ValidarDataFutura_DeveRetornarFalse_QuandoDataNoPassado()
        {
            // Arrange
            var dataPassada = DateTime.Now.AddDays(-1);
            var consulta = new Consulta { DataHora = dataPassada };

            // Act
            var resultado = consulta.ValidarDataFutura();

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void ValidarHorarioComercial_DeveRetornarTrue_QuandoHorarioValido()
        {
            // Arrange - Horários comerciais: 8h às 18h, segunda a sexta
            var dataHoraValida = new DateTime(2024, 11, 8, 14, 30, 0); // Sexta-feira, 14:30
            var consulta = new Consulta { DataHora = dataHoraValida };

            // Act
            var resultado = consulta.ValidarHorarioComercial();

            // Assert
            resultado.Should().BeTrue();
        }

        [Theory]
        [InlineData(2024, 11, 9, 14, 30)] // Sábado
        [InlineData(2024, 11, 10, 14, 30)] // Domingo
        [InlineData(2024, 11, 8, 7, 30)] // Sexta-feira, antes das 8h
        [InlineData(2024, 11, 8, 18, 30)] // Sexta-feira, depois das 18h
        public void ValidarHorarioComercial_DeveRetornarFalse_QuandoHorarioInvalido(
            int ano, int mes, int dia, int hora, int minuto)
        {
            // Arrange
            var dataHoraInvalida = new DateTime(ano, mes, dia, hora, minuto, 0);
            var consulta = new Consulta { DataHora = dataHoraInvalida };

            // Act
            var resultado = consulta.ValidarHorarioComercial();

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void PodeSerCancelada_DeveRetornarTrue_QuandoStatusAgendada()
        {
            // Arrange
            var consulta = new Consulta { Status = StatusConsulta.Agendada };

            // Act
            var resultado = consulta.PodeSerCancelada();

            // Assert
            resultado.Should().BeTrue();
        }

        [Theory]
        [InlineData(StatusConsulta.Realizada)]
        [InlineData(StatusConsulta.Cancelada)]
        [InlineData(StatusConsulta.Faltou)]
        public void PodeSerCancelada_DeveRetornarFalse_QuandoStatusNaoPermiteCancelamento(StatusConsulta status)
        {
            // Arrange
            var consulta = new Consulta { Status = status };

            // Act
            var resultado = consulta.PodeSerCancelada();

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void PodeSerRealizada_DeveRetornarTrue_QuandoStatusAgendadaEDataAtual()
        {
            // Arrange
            var dataAtual = DateTime.Now;
            var consulta = new Consulta 
            { 
                Status = StatusConsulta.Agendada,
                DataHora = dataAtual.AddMinutes(-30) // 30 minutos atrás
            };

            // Act
            var resultado = consulta.PodeSerRealizada(dataAtual);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void PodeSerRealizada_DeveRetornarFalse_QuandoStatusNaoAgendada()
        {
            // Arrange
            var dataAtual = DateTime.Now;
            var consulta = new Consulta 
            { 
                Status = StatusConsulta.Cancelada,
                DataHora = dataAtual.AddMinutes(-30)
            };

            // Act
            var resultado = consulta.PodeSerRealizada(dataAtual);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void PodeSerRealizada_DeveRetornarFalse_QuandoDataMuitoNoFuturo()
        {
            // Arrange
            var dataAtual = DateTime.Now;
            var consulta = new Consulta 
            { 
                Status = StatusConsulta.Agendada,
                DataHora = dataAtual.AddHours(2) // 2 horas no futuro
            };

            // Act
            var resultado = consulta.PodeSerRealizada(dataAtual);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void Cancelar_DeveAlterarStatusParaCancelada_QuandoPermitido()
        {
            // Arrange
            var consulta = new Consulta { Status = StatusConsulta.Agendada };
            var motivoCancelamento = "Paciente não pode comparecer";

            // Act
            var resultado = consulta.Cancelar(motivoCancelamento);

            // Assert
            resultado.Should().BeTrue();
            consulta.Status.Should().Be(StatusConsulta.Cancelada);
            consulta.Observacoes.Should().Contain(motivoCancelamento);
        }

        [Fact]
        public void Cancelar_DeveRetornarFalse_QuandoNaoPermitido()
        {
            // Arrange
            var consulta = new Consulta { Status = StatusConsulta.Realizada };
            var motivoCancelamento = "Teste";

            // Act
            var resultado = consulta.Cancelar(motivoCancelamento);

            // Assert
            resultado.Should().BeFalse();
            consulta.Status.Should().Be(StatusConsulta.Realizada); // Status não deve mudar
        }

        [Fact]
        public void Realizar_DeveAlterarStatusParaRealizada_QuandoPermitido()
        {
            // Arrange
            var dataAtual = DateTime.Now;
            var consulta = new Consulta 
            { 
                Status = StatusConsulta.Agendada,
                DataHora = dataAtual.AddMinutes(-15)
            };

            // Act
            var resultado = consulta.Realizar(dataAtual);

            // Assert
            resultado.Should().BeTrue();
            consulta.Status.Should().Be(StatusConsulta.Realizada);
        }

        [Fact]
        public void CalcularDuracao_DeveRetornarDuracaoPadrao_QuandoNaoEspecificada()
        {
            // Arrange
            var consulta = new Consulta();

            // Act
            var duracao = consulta.CalcularDuracao();

            // Assert
            duracao.Should().Be(TimeSpan.FromMinutes(30)); // Duração padrão
        }

        [Fact]
        public void CalcularDuracao_DeveRetornarDuracaoEspecificada_QuandoDefinida()
        {
            // Arrange
            var duracaoEsperada = TimeSpan.FromMinutes(60);
            var consulta = new Consulta { Duracao = duracaoEsperada };

            // Act
            var duracao = consulta.CalcularDuracao();

            // Assert
            duracao.Should().Be(duracaoEsperada);
        }

        [Fact]
        public void ObterHorarioFim_DeveCalcularCorretamente()
        {
            // Arrange
            var dataInicio = new DateTime(2024, 11, 8, 14, 30, 0);
            var duracao = TimeSpan.FromMinutes(45);
            var consulta = new Consulta 
            { 
                DataHora = dataInicio,
                Duracao = duracao
            };

            // Act
            var horarioFim = consulta.ObterHorarioFim();

            // Assert
            horarioFim.Should().Be(new DateTime(2024, 11, 8, 15, 15, 0));
        }

        [Theory]
        [InlineData(StatusConsulta.Agendada, "Agendada")]
        [InlineData(StatusConsulta.Realizada, "Realizada")]
        [InlineData(StatusConsulta.Cancelada, "Cancelada")]
        [InlineData(StatusConsulta.Faltou, "Paciente faltou")]
        public void ObterDescricaoStatus_DeveRetornarDescricaoCorreta(StatusConsulta status, string descricaoEsperada)
        {
            // Arrange
            var consulta = new Consulta { Status = status };

            // Act
            var descricao = consulta.ObterDescricaoStatus();

            // Assert
            descricao.Should().Be(descricaoEsperada);
        }
    }

    // Enums e classes auxiliares para simular as entidades do domínio
    public enum StatusConsulta
    {
        Agendada = 1,
        Realizada = 2,
        Cancelada = 3,
        Faltou = 4
    }

    public class Consulta
    {
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public DateTime DataHora { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public StatusConsulta Status { get; set; }
        public TimeSpan? Duracao { get; set; }

        public bool ValidarDataFutura()
        {
            return DataHora > DateTime.Now;
        }

        public bool ValidarHorarioComercial()
        {
            // Verifica se é dia útil (segunda a sexta)
            if (DataHora.DayOfWeek == DayOfWeek.Saturday || DataHora.DayOfWeek == DayOfWeek.Sunday)
                return false;

            // Verifica se está no horário comercial (8h às 18h)
            var hora = DataHora.Hour;
            return hora >= 8 && hora < 18;
        }

        public bool PodeSerCancelada()
        {
            return Status == StatusConsulta.Agendada;
        }

        public bool PodeSerRealizada(DateTime dataAtual)
        {
            if (Status != StatusConsulta.Agendada)
                return false;

            // Pode ser realizada até 1 hora após o horário agendado
            var limiteRealizacao = DataHora.AddHours(1);
            return dataAtual >= DataHora && dataAtual <= limiteRealizacao;
        }

        public bool Cancelar(string motivo)
        {
            if (!PodeSerCancelada())
                return false;

            Status = StatusConsulta.Cancelada;
            Observacoes += $" | Cancelada: {motivo}";
            return true;
        }

        public bool Realizar(DateTime dataAtual)
        {
            if (!PodeSerRealizada(dataAtual))
                return false;

            Status = StatusConsulta.Realizada;
            return true;
        }

        public TimeSpan CalcularDuracao()
        {
            return Duracao ?? TimeSpan.FromMinutes(30);
        }

        public DateTime ObterHorarioFim()
        {
            return DataHora.Add(CalcularDuracao());
        }

        public string ObterDescricaoStatus()
        {
            return Status switch
            {
                StatusConsulta.Agendada => "Agendada",
                StatusConsulta.Realizada => "Realizada",
                StatusConsulta.Cancelada => "Cancelada",
                StatusConsulta.Faltou => "Paciente faltou",
                _ => "Status desconhecido"
            };
        }
    }
}