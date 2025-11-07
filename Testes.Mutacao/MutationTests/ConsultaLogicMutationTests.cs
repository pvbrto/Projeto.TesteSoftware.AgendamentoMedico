using Xunit;
using FluentAssertions;
using Testes.Mutacao.CodeUnderTest;
using System;

namespace Testes.Mutacao.MutationTests
{
    /// <summary>
    /// Testes de mutação para ConsultaLogic
    /// Objetivo: Detectar mutações em lógica de agendamento e validação de consultas
    /// </summary>
    public class ConsultaLogicMutationTests
    {
        private readonly ConsultaLogic _consultaLogic;

        public ConsultaLogicMutationTests()
        {
            _consultaLogic = new ConsultaLogic();
        }

        #region Testes para ValidarHorarioConsulta - Detectar mutações em operadores relacionais

        [Theory]
        [InlineData(8, 0, true)]   // Exatamente 8:00 - detecta mutação >= para >
        [InlineData(7, 59, false)] // 7:59 - detecta mutação >= para >
        [InlineData(18, 0, true)]  // Exatamente 18:00 - detecta mutação <= para <
        [InlineData(18, 1, false)] // 18:01 - detecta mutação <= para <
        [InlineData(12, 30, true)] // Meio do dia - valor válido
        public void ValidarHorarioConsulta_DeveDetectarMutacoesEmOperadoresRelacionais(int hora, int minuto, bool esperado)
        {
            // Arrange
            var horario = new TimeSpan(hora, minuto, 0);

            // Act
            var resultado = _consultaLogic.ValidarHorarioConsulta(horario);

            // Assert
            resultado.Should().Be(esperado, 
                $"Horário {hora:D2}:{minuto:D2} deve retornar {esperado}");
        }

        [Theory]
        [InlineData(6, 0, false)]  // Antes das 8h
        [InlineData(19, 0, false)] // Depois das 18h
        [InlineData(23, 59, false)] // Final do dia
        public void ValidarHorarioConsulta_DeveRejeitarHorariosForaDoExpediente(int hora, int minuto, bool esperado)
        {
            // Arrange
            var horario = new TimeSpan(hora, minuto, 0);

            // Act
            var resultado = _consultaLogic.ValidarHorarioConsulta(horario);

            // Assert
            resultado.Should().Be(esperado);
        }

        #endregion

        #region Testes para CalcularDuracaoConsulta - Detectar mutações aritméticas

        [Theory]
        [InlineData(8, 0, 8, 30, 30)]   // 30 minutos - detecta mutação - para +
        [InlineData(9, 15, 10, 0, 45)]  // 45 minutos - detecta mutação em cálculo
        [InlineData(14, 30, 15, 30, 60)] // 1 hora - detecta mutação * para /
        [InlineData(16, 0, 16, 15, 15)]  // 15 minutos - valor mínimo
        public void CalcularDuracaoConsulta_DeveDetectarMutacoesAritmeticas(
            int horaInicio, int minutoInicio, int horaFim, int minutoFim, int duracaoEsperada)
        {
            // Arrange
            var inicio = new TimeSpan(horaInicio, minutoInicio, 0);
            var fim = new TimeSpan(horaFim, minutoFim, 0);

            // Act
            var duracao = _consultaLogic.CalcularDuracaoConsulta(inicio, fim);

            // Assert
            duracao.Should().Be(duracaoEsperada, 
                $"Duração entre {inicio} e {fim} deve ser {duracaoEsperada} minutos");
        }

        [Fact]
        public void CalcularDuracaoConsulta_DeveDetectarMutacaoEmHorarioInvalido()
        {
            // Arrange - fim antes do início
            var inicio = new TimeSpan(10, 0, 0);
            var fim = new TimeSpan(9, 0, 0);

            // Act & Assert - detecta mutação em validação de ordem
            var exception = Assert.Throws<ArgumentException>(() => 
                _consultaLogic.CalcularDuracaoConsulta(inicio, fim));
            
            exception.Message.Should().Contain("fim deve ser posterior ao início");
        }

        #endregion

        #region Testes para ValidarIntervaloEntreConsultas - Detectar mutações em constantes

        [Theory]
        [InlineData(15, true)]  // Exatamente 15 minutos - detecta mutação >= para >
        [InlineData(14, false)] // 14 minutos - detecta mutação >= para >
        [InlineData(30, true)]  // 30 minutos - valor válido
        [InlineData(0, false)]  // Zero minutos - detecta mutação em validação
        public void ValidarIntervaloEntreConsultas_DeveDetectarMutacoesEmConstantes(int intervalo, bool esperado)
        {
            // Act
            var resultado = _consultaLogic.ValidarIntervaloEntreConsultas(intervalo);

            // Assert
            resultado.Should().Be(esperado, 
                $"Intervalo de {intervalo} minutos deve retornar {esperado}");
        }

        [Theory]
        [InlineData(-1, false)]  // Negativo
        [InlineData(-10, false)] // Muito negativo
        public void ValidarIntervaloEntreConsultas_DeveRejeitarValoresNegativos(int intervalo, bool esperado)
        {
            // Act
            var resultado = _consultaLogic.ValidarIntervaloEntreConsultas(intervalo);

            // Assert
            resultado.Should().Be(esperado);
        }

        #endregion

        #region Testes para EhDiaUtil - Detectar mutações lógicas

        [Theory]
        [InlineData(DayOfWeek.Monday, true)]    // Segunda - detecta mutação || para &&
        [InlineData(DayOfWeek.Tuesday, true)]   // Terça
        [InlineData(DayOfWeek.Wednesday, true)] // Quarta
        [InlineData(DayOfWeek.Thursday, true)]  // Quinta
        [InlineData(DayOfWeek.Friday, true)]    // Sexta
        [InlineData(DayOfWeek.Saturday, false)] // Sábado - detecta mutação != para ==
        [InlineData(DayOfWeek.Sunday, false)]   // Domingo - detecta mutação != para ==
        public void EhDiaUtil_DeveDetectarMutacoesLogicas(DayOfWeek diaSemana, bool esperado)
        {
            // Act
            var resultado = _consultaLogic.EhDiaUtil(diaSemana);

            // Assert
            resultado.Should().Be(esperado, 
                $"{diaSemana} deve retornar {esperado}");
        }

        #endregion

        #region Testes para CalcularValorConsulta - Detectar mutações complexas

        [Theory]
        [InlineData(30, 100.0, 100.0)]   // 30 min = valor base - detecta mutação <= para <
        [InlineData(29, 100.0, 100.0)]   // 29 min = valor base - detecta mutação <= para <
        [InlineData(31, 100.0, 110.0)]   // 31 min = valor + 10% - detecta mutação > para >=
        [InlineData(45, 100.0, 150.0)]   // 45 min = valor + 50% - detecta mutação em cálculo
        [InlineData(60, 100.0, 200.0)]   // 60 min = valor * 2 - detecta mutação * para /
        public void CalcularValorConsulta_DeveDetectarMutacoesEmCalculos(
            int duracao, double valorBase, double valorEsperado)
        {
            // Act
            var valor = _consultaLogic.CalcularValorConsulta(duracao, valorBase);

            // Assert
            valor.Should().Be(valorEsperado, 
                $"Consulta de {duracao} min com valor base {valorBase:C} deve custar {valorEsperado:C}");
        }

        [Theory]
        [InlineData(0, 100.0)]    // Duração zero
        [InlineData(-1, 100.0)]   // Duração negativa
        [InlineData(30, 0.0)]     // Valor base zero
        [InlineData(30, -50.0)]   // Valor base negativo
        public void CalcularValorConsulta_DeveDetectarMutacoesEmValidacoes(int duracao, double valorBase)
        {
            // Act & Assert - detecta mutações em validações de entrada
            var exception = Assert.Throws<ArgumentException>(() => 
                _consultaLogic.CalcularValorConsulta(duracao, valorBase));
            
            exception.Should().NotBeNull();
        }

        #endregion

        #region Testes para ValidarDataConsulta - Detectar mutações em datas

        [Fact]
        public void ValidarDataConsulta_DeveDetectarMutacaoEmDataPassada()
        {
            // Arrange - data no passado
            var dataPassada = DateTime.Now.AddDays(-1);

            // Act
            var resultado = _consultaLogic.ValidarDataConsulta(dataPassada);

            // Assert - detecta mutação >= para >
            resultado.Should().BeFalse("Data no passado deve ser inválida");
        }

        [Fact]
        public void ValidarDataConsulta_DeveDetectarMutacaoEmDataHoje()
        {
            // Arrange - data de hoje
            var hoje = DateTime.Today;

            // Act
            var resultado = _consultaLogic.ValidarDataConsulta(hoje);

            // Assert - detecta mutação >= para >
            resultado.Should().BeTrue("Data de hoje deve ser válida");
        }

        [Fact]
        public void ValidarDataConsulta_DeveDetectarMutacaoEmDataFutura()
        {
            // Arrange - data no futuro
            var dataFutura = DateTime.Now.AddDays(7);

            // Act
            var resultado = _consultaLogic.ValidarDataConsulta(dataFutura);

            // Assert
            resultado.Should().BeTrue("Data futura deve ser válida");
        }

        [Fact]
        public void ValidarDataConsulta_DeveDetectarMutacaoEmLimiteMaximo()
        {
            // Arrange - data no limite máximo (90 dias)
            var dataLimite = DateTime.Today.AddDays(90);

            // Act
            var resultado = _consultaLogic.ValidarDataConsulta(dataLimite);

            // Assert - detecta mutação <= para <
            resultado.Should().BeTrue("Data no limite de 90 dias deve ser válida");
        }

        [Fact]
        public void ValidarDataConsulta_DeveDetectarMutacaoAlemDoLimite()
        {
            // Arrange - data além do limite (91 dias)
            var dataAlemLimite = DateTime.Today.AddDays(91);

            // Act
            var resultado = _consultaLogic.ValidarDataConsulta(dataAlemLimite);

            // Assert - detecta mutação <= para <
            resultado.Should().BeFalse("Data além de 90 dias deve ser inválida");
        }

        #endregion

        #region Testes para ObterProximoHorarioDisponivel - Detectar mutações em loops

        [Fact]
        public void ObterProximoHorarioDisponivel_DeveDetectarMutacaoEmIncremento()
        {
            // Arrange
            var horariosOcupados = new[] 
            { 
                new TimeSpan(9, 0, 0), 
                new TimeSpan(9, 30, 0) 
            };

            // Act
            var proximoHorario = _consultaLogic.ObterProximoHorarioDisponivel(horariosOcupados);

            // Assert - detecta mutação ++ para -- ou += para -=
            proximoHorario.Should().Be(new TimeSpan(10, 0, 0), 
                "Próximo horário disponível deve ser 10:00");
        }

        [Fact]
        public void ObterProximoHorarioDisponivel_DeveDetectarMutacaoEmCondicaoLoop()
        {
            // Arrange - todos os horários ocupados
            var horariosOcupados = new TimeSpan[20]; // 8h às 18h (intervalos de 30min)
            for (int i = 0; i < 20; i++)
            {
                horariosOcupados[i] = new TimeSpan(8 + (i / 2), (i % 2) * 30, 0);
            }

            // Act & Assert - detecta mutação em condição de parada do loop
            var exception = Assert.Throws<InvalidOperationException>(() => 
                _consultaLogic.ObterProximoHorarioDisponivel(horariosOcupados));
            
            exception.Message.Should().Contain("Não há horários disponíveis");
        }

        #endregion

        #region Testes para ValidarEspecialidade - Detectar mutações em strings

        [Theory]
        [InlineData("Cardiologia", true)]     // Especialidade válida
        [InlineData("Dermatologia", true)]    // Especialidade válida
        [InlineData("Neurologia", true)]      // Especialidade válida
        [InlineData("Ortopedia", true)]       // Especialidade válida
        [InlineData("Pediatria", true)]       // Especialidade válida
        [InlineData("InvalidaEspec", false)]  // Especialidade inválida
        [InlineData("", false)]               // String vazia - detecta mutação IsNullOrEmpty
        [InlineData(null, false)]             // Null - detecta mutação IsNullOrEmpty
        public void ValidarEspecialidade_DeveDetectarMutacoesEmStrings(string especialidade, bool esperado)
        {
            // Act
            var resultado = _consultaLogic.ValidarEspecialidade(especialidade);

            // Assert
            resultado.Should().Be(esperado, 
                $"Especialidade '{especialidade}' deve retornar {esperado}");
        }

        [Theory]
        [InlineData("cardiologia", true)]     // Minúscula - detecta mutação em case sensitivity
        [InlineData("CARDIOLOGIA", true)]     // Maiúscula - detecta mutação em case sensitivity
        [InlineData("CaRdIoLoGiA", true)]     // Mista - detecta mutação em case sensitivity
        public void ValidarEspecialidade_DeveDetectarMutacoesEmCaseSensitivity(string especialidade, bool esperado)
        {
            // Act
            var resultado = _consultaLogic.ValidarEspecialidade(especialidade);

            // Assert
            resultado.Should().Be(esperado, 
                "Validação deve ser case-insensitive");
        }

        #endregion

        #region Testes para CalcularTempoEspera - Detectar mutações em condicionais aninhadas

        [Theory]
        [InlineData("Urgente", 0)]      // Prioridade alta - detecta mutação == para !=
        [InlineData("Normal", 30)]      // Prioridade normal - detecta mutação em valor
        [InlineData("Baixa", 60)]       // Prioridade baixa - detecta mutação em valor
        [InlineData("", 30)]            // String vazia - valor padrão
        [InlineData(null, 30)]          // Null - valor padrão
        public void CalcularTempoEspera_DeveDetectarMutacoesEmCondicionais(string prioridade, int tempoEsperado)
        {
            // Act
            var tempo = _consultaLogic.CalcularTempoEspera(prioridade);

            // Assert
            tempo.Should().Be(tempoEsperado, 
                $"Prioridade '{prioridade}' deve ter tempo de espera {tempoEsperado} minutos");
        }

        #endregion

        #region Testes para ValidarIdadePaciente - Detectar mutações em ranges

        [Theory]
        [InlineData(0, true)]     // Recém-nascido - detecta mutação >= para >
        [InlineData(1, true)]     // Criança
        [InlineData(17, true)]    // Adolescente
        [InlineData(18, true)]    // Adulto jovem
        [InlineData(65, true)]    // Idoso - detecta mutação <= para <
        [InlineData(120, true)]   // Idade máxima - detecta mutação <= para <
        [InlineData(-1, false)]   // Idade inválida - detecta mutação >= para >
        [InlineData(121, false)]  // Idade inválida - detecta mutação <= para <
        public void ValidarIdadePaciente_DeveDetectarMutacoesEmRanges(int idade, bool esperado)
        {
            // Act
            var resultado = _consultaLogic.ValidarIdadePaciente(idade);

            // Assert
            resultado.Should().Be(esperado, 
                $"Idade {idade} deve retornar {esperado}");
        }

        #endregion

        #region Testes para CalcularDesconto - Detectar mutações em múltiplas condições

        [Theory]
        [InlineData(70, 0.15)]    // Idoso - 15% desconto - detecta mutação >= para >
        [InlineData(69, 0.0)]     // Não idoso - detecta mutação >= para >
        [InlineData(5, 0.10)]     // Criança - 10% desconto - detecta mutação <= para <
        [InlineData(6, 0.0)]      // Não criança - detecta mutação <= para <
        [InlineData(25, 0.0)]     // Adulto - sem desconto
        public void CalcularDesconto_DeveDetectarMutacoesEmMultiplasCondicoes(int idade, double descontoEsperado)
        {
            // Act
            var desconto = _consultaLogic.CalcularDesconto(idade);

            // Assert
            desconto.Should().Be(descontoEsperado, 
                $"Idade {idade} deve ter desconto de {descontoEsperado:P}");
        }

        #endregion

        #region Testes Extremos para Detectar Mutações Específicas

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmReturnTrue()
        {
            // Teste específico para detectar mutação "return true" → "return false"
            var resultado = _consultaLogic.ValidarHorarioConsulta(new TimeSpan(10, 0, 0));
            resultado.Should().BeTrue("Horário válido deve retornar true");
        }

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmReturnFalse()
        {
            // Teste específico para detectar mutação "return false" → "return true"
            var resultado = _consultaLogic.ValidarHorarioConsulta(new TimeSpan(6, 0, 0));
            resultado.Should().BeFalse("Horário inválido deve retornar false");
        }

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmConstanteZero()
        {
            // Teste específico para detectar mutação "0" → "1"
            var resultado = _consultaLogic.CalcularTempoEspera("Urgente");
            resultado.Should().Be(0, "Prioridade urgente deve ter tempo zero");
        }

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmConstanteUm()
        {
            // Teste específico para detectar mutação "1" → "0"
            var resultado = _consultaLogic.ValidarIdadePaciente(1);
            resultado.Should().BeTrue("Idade 1 deve ser válida");
        }

        #endregion
    }
}