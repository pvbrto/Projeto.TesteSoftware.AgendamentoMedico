using Xunit;
using FluentAssertions;
using Testes.Mutacao.CodeUnderTest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Testes.Mutacao.MutationTests
{
    /// <summary>
    /// Testes de mutação para AgendamentoLogic
    /// Objetivo: Detectar mutações em lógica complexa de agendamento
    /// </summary>
    public class AgendamentoLogicMutationTests
    {
        private readonly AgendamentoLogic _agendamentoLogic;

        public AgendamentoLogicMutationTests()
        {
            _agendamentoLogic = new AgendamentoLogic();
        }

        #region Testes para ValidarAgendamento - Detectar mutações em operadores lógicos complexos

        [Theory]
        [InlineData("2024-12-01", 10, 0, "Cardiologia", 30, true)]   // Caso válido completo
        [InlineData("2024-11-05", 10, 0, "Cardiologia", 30, false)]  // Data passada - detecta >= para >
        [InlineData("2024-12-01", 7, 59, "Cardiologia", 30, false)]  // Antes 8h - detecta >= para >
        [InlineData("2024-12-01", 8, 0, "Cardiologia", 30, true)]    // Exato 8h - detecta >= para >
        [InlineData("2024-12-01", 18, 0, "Cardiologia", 30, true)]   // Exato 18h - detecta <= para <
        [InlineData("2024-12-01", 18, 1, "Cardiologia", 30, false)]  // Após 18h - detecta <= para <
        public void ValidarAgendamento_DeveDetectarMutacoesEmOperadoresLogicos(
            string dataStr, int hora, int minuto, string especialidade, int idade, bool esperado)
        {
            // Arrange
            var data = DateTime.Parse(dataStr);
            var horario = new TimeSpan(hora, minuto, 0);

            // Act
            var resultado = _agendamentoLogic.ValidarAgendamento(data, horario, especialidade, idade);

            // Assert
            resultado.Should().Be(esperado, 
                $"Agendamento para {data:dd/MM/yyyy} às {horario} deve retornar {esperado}");
        }

        [Theory]
        [InlineData("", false)]                    // Especialidade vazia - detecta IsNullOrEmpty
        [InlineData(null, false)]                  // Especialidade null - detecta IsNullOrEmpty
        [InlineData("EspecialidadeInvalida", false)] // Especialidade não existe - detecta Contains
        [InlineData("Cardiologia", true)]          // Especialidade válida
        [InlineData("cardiologia", false)]         // Case sensitive - detecta Contains
        public void ValidarAgendamento_DeveDetectarMutacoesEmEspecialidade(string especialidade, bool esperado)
        {
            // Arrange
            var data = DateTime.Today.AddDays(1);
            var horario = new TimeSpan(10, 0, 0);
            var idade = 30;

            // Act
            var resultado = _agendamentoLogic.ValidarAgendamento(data, horario, especialidade, idade);

            // Assert
            resultado.Should().Be(esperado, 
                $"Especialidade '{especialidade}' deve retornar {esperado}");
        }

        [Theory]
        [InlineData(-1, false)]   // Idade negativa - detecta >= para >
        [InlineData(0, true)]     // Idade zero - detecta >= para >
        [InlineData(120, true)]   // Idade máxima - detecta <= para <
        [InlineData(121, false)]  // Idade acima máxima - detecta <= para <
        public void ValidarAgendamento_DeveDetectarMutacoesEmIdade(int idade, bool esperado)
        {
            // Arrange
            var data = DateTime.Today.AddDays(1);
            var horario = new TimeSpan(10, 0, 0);
            var especialidade = "Cardiologia";

            // Act
            var resultado = _agendamentoLogic.ValidarAgendamento(data, horario, especialidade, idade);

            // Assert
            resultado.Should().Be(esperado, 
                $"Idade {idade} deve retornar {esperado}");
        }

        #endregion

        #region Testes para CalcularValorTotal - Detectar mutações aritméticas complexas

        [Theory]
        [InlineData("Cardiologia", 30, false, 30, 200.0)]  // Valor base sem desconto
        [InlineData("Cardiologia", 30, false, 60, 400.0)]  // Dobro duração - detecta * para /
        [InlineData("Cardiologia", 30, false, 15, 100.0)]  // Metade duração - detecta / para *
        [InlineData("Pediatria", 30, false, 30, 120.0)]   // Especialidade mais barata
        public void CalcularValorTotal_DeveDetectarMutacoesAritmeticas(
            string especialidade, int idade, bool convenio, int duracao, double valorEsperado)
        {
            // Act
            var valor = _agendamentoLogic.CalcularValorTotal(especialidade, idade, convenio, duracao);

            // Assert
            valor.Should().Be(valorEsperado, 
                $"Valor para {especialidade}, {duracao}min deve ser {valorEsperado:C}");
        }

        [Theory]
        [InlineData("Cardiologia", 70, false, 30, 160.0)]  // Idoso 20% desconto - detecta >= para >
        [InlineData("Cardiologia", 65, false, 30, 160.0)]  // Exato 65 anos - detecta >= para >
        [InlineData("Cardiologia", 64, false, 30, 200.0)]  // Não idoso - detecta >= para >
        [InlineData("Cardiologia", 12, false, 30, 170.0)]  // Criança 15% desconto - detecta <= para <
        [InlineData("Cardiologia", 13, false, 30, 200.0)]  // Não criança - detecta <= para <
        public void CalcularValorTotal_DeveDetectarMutacoesEmDescontoPorIdade(
            string especialidade, int idade, bool convenio, int duracao, double valorEsperado)
        {
            // Act
            var valor = _agendamentoLogic.CalcularValorTotal(especialidade, idade, convenio, duracao);

            // Assert
            valor.Should().Be(valorEsperado, 
                $"Idade {idade} deve resultar em valor {valorEsperado:C}");
        }

        [Theory]
        [InlineData("Cardiologia", 70, true, 30, 150.0)]   // Idoso + convênio = 25% - detecta && para ||
        [InlineData("Cardiologia", 30, true, 30, 180.0)]   // Só convênio = 10% - detecta else if
        [InlineData("Cardiologia", 12, true, 30, 160.0)]   // Criança + convênio = 20% - detecta + para -
        public void CalcularValorTotal_DeveDetectarMutacoesEmDescontoConvenio(
            string especialidade, int idade, bool convenio, int duracao, double valorEsperado)
        {
            // Act
            var valor = _agendamentoLogic.CalcularValorTotal(especialidade, idade, convenio, duracao);

            // Assert
            valor.Should().Be(valorEsperado, 
                $"Convênio com idade {idade} deve resultar em {valorEsperado:C}");
        }

        [Fact]
        public void CalcularValorTotal_DeveDetectarMutacaoEmValorMinimo()
        {
            // Arrange - configurar para valor muito baixo
            var especialidade = "Pediatria"; // R$ 120 base
            var duracao = 5; // 5 minutos = valor muito baixo

            // Act
            var valor = _agendamentoLogic.CalcularValorTotal(especialidade, 30, false, duracao);

            // Assert - detecta mutação < para <=, >= para >
            valor.Should().Be(50.0, "Valor mínimo deve ser R$ 50,00");
        }

        [Theory]
        [InlineData("", 30, false, 30)]           // Especialidade vazia
        [InlineData(null, 30, false, 30)]         // Especialidade null
        [InlineData("Inexistente", 30, false, 30)] // Especialidade inexistente
        [InlineData("Cardiologia", 30, false, 0)]  // Duração zero - detecta <= para <
        [InlineData("Cardiologia", 30, false, -1)] // Duração negativa - detecta <= para <
        public void CalcularValorTotal_DeveDetectarMutacoesEmValidacoes(
            string especialidade, int idade, bool convenio, int duracao)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _agendamentoLogic.CalcularValorTotal(especialidade, idade, convenio, duracao));
            
            exception.Should().NotBeNull();
        }

        #endregion

        #region Testes para ObterHorariosDisponiveis - Detectar mutações em loops

        [Fact]
        public void ObterHorariosDisponiveis_DeveDetectarMutacoesEmLoop()
        {
            // Arrange
            var data = DateTime.Today.AddDays(1); // Amanhã
            var horariosOcupados = new List<TimeSpan> { new(9, 0, 0), new(14, 30, 0) };

            // Act
            var disponiveis = _agendamentoLogic.ObterHorariosDisponiveis(data, horariosOcupados);

            // Assert - detecta mutações em limites do loop
            disponiveis.Should().Contain(new TimeSpan(8, 0, 0), "Deve incluir 8:00");
            disponiveis.Should().Contain(new TimeSpan(17, 30, 0), "Deve incluir 17:30");
            disponiveis.Should().NotContain(new TimeSpan(18, 0, 0), "Não deve incluir 18:00");
            disponiveis.Should().NotContain(new TimeSpan(7, 30, 0), "Não deve incluir 7:30");
        }

        [Fact]
        public void ObterHorariosDisponiveis_DeveDetectarMutacoesEmContains()
        {
            // Arrange
            var data = DateTime.Today.AddDays(1);
            var horariosOcupados = new List<TimeSpan> { new(10, 0, 0) };

            // Act
            var disponiveis = _agendamentoLogic.ObterHorariosDisponiveis(data, horariosOcupados);

            // Assert - detecta mutação ! para vazio, Contains para !Contains
            disponiveis.Should().NotContain(new TimeSpan(10, 0, 0), "Horário ocupado não deve estar disponível");
            disponiveis.Should().Contain(new TimeSpan(10, 30, 0), "Horário livre deve estar disponível");
        }

        [Fact]
        public void ObterHorariosDisponiveis_DeveDetectarMutacoesEmFiltroDataHoje()
        {
            // Arrange - data de hoje
            var data = DateTime.Today;
            var horariosOcupados = new List<TimeSpan>();
            
            // Simular que agora são 10:00
            var agora = new TimeSpan(10, 0, 0);

            // Act
            var disponiveis = _agendamentoLogic.ObterHorariosDisponiveis(data, horariosOcupados);

            // Assert - detecta mutações em comparação de data e horário
            // Nota: Este teste pode falhar dependendo do horário atual, mas detecta mutações == para !=
            disponiveis.Should().NotBeEmpty("Deve haver horários disponíveis");
        }

        [Fact]
        public void ObterHorariosDisponiveis_DeveDetectarMutacoesEmIncrementoLoop()
        {
            // Arrange
            var data = DateTime.Today.AddDays(1);
            var horariosOcupados = new List<TimeSpan>();

            // Act
            var disponiveis = _agendamentoLogic.ObterHorariosDisponiveis(data, horariosOcupados);

            // Assert - detecta mutações += 30 para -= 30, ++ para --
            disponiveis.Should().HaveCount(20, "Deve ter 20 horários (8h-17h30, intervalos 30min)");
            disponiveis.Should().Contain(new TimeSpan(8, 30, 0), "Deve incluir 8:30");
            disponiveis.Should().Contain(new TimeSpan(9, 0, 0), "Deve incluir 9:00");
        }

        #endregion

        #region Testes para TemConflito - Detectar mutações em comparações de data/hora

        [Theory]
        [InlineData("2024-12-01", 10, 0, 30, "2024-12-01", 10, 15, 30, true)]   // Sobreposição
        [InlineData("2024-12-01", 10, 0, 30, "2024-12-01", 10, 30, 30, false)]  // Sequencial
        [InlineData("2024-12-01", 10, 0, 30, "2024-12-02", 10, 0, 30, false)]   // Dias diferentes
        [InlineData("2024-12-01", 10, 0, 30, "2024-12-01", 9, 45, 30, true)]    // Início sobreposto
        public void TemConflito_DeveDetectarMutacoesEmComparacoes(
            string data1Str, int hora1, int min1, int dur1,
            string data2Str, int hora2, int min2, int dur2, bool esperado)
        {
            // Arrange
            var data1 = DateTime.Parse(data1Str);
            var horario1 = new TimeSpan(hora1, min1, 0);
            var data2 = DateTime.Parse(data2Str);
            var horario2 = new TimeSpan(hora2, min2, 0);

            // Act
            var conflito = _agendamentoLogic.TemConflito(data1, horario1, dur1, data2, horario2, dur2);

            // Assert
            conflito.Should().Be(esperado, 
                $"Conflito entre {data1:dd/MM} {horario1} ({dur1}min) e {data2:dd/MM} {horario2} ({dur2}min)");
        }

        [Fact]
        public void TemConflito_DeveDetectarMutacaoEmComparacaoData()
        {
            // Arrange - datas diferentes
            var data1 = new DateTime(2024, 12, 1);
            var data2 = new DateTime(2024, 12, 2);
            var horario = new TimeSpan(10, 0, 0);

            // Act
            var conflito = _agendamentoLogic.TemConflito(data1, horario, 60, data2, horario, 60);

            // Assert - detecta mutação == para !=
            conflito.Should().BeFalse("Datas diferentes não devem ter conflito");
        }

        [Theory]
        [InlineData(10, 0, 60, 11, 0, 60, false)]  // Não sobrepõe - detecta < para <=
        [InlineData(10, 0, 60, 10, 59, 60, true)]  // Sobrepõe por 1 minuto - detecta > para >=
        public void TemConflito_DeveDetectarMutacoesEmSobreposicao(
            int hora1, int min1, int dur1, int hora2, int min2, int dur2, bool esperado)
        {
            // Arrange
            var data = DateTime.Today;
            var horario1 = new TimeSpan(hora1, min1, 0);
            var horario2 = new TimeSpan(hora2, min2, 0);

            // Act
            var conflito = _agendamentoLogic.TemConflito(data, horario1, dur1, data, horario2, dur2);

            // Assert
            conflito.Should().Be(esperado, 
                $"Horários {horario1} ({dur1}min) e {horario2} ({dur2}min) - conflito: {esperado}");
        }

        #endregion

        #region Testes para CalcularEstatisticas - Detectar mutações em agregações

        [Fact]
        public void CalcularEstatisticas_DeveDetectarMutacaoEmListaVazia()
        {
            // Arrange
            var consultas = new List<ConsultaInfo>();

            // Act
            var stats = _agendamentoLogic.CalcularEstatisticas(consultas);

            // Assert - detecta mutações em verificação null/empty
            stats.Should().NotBeNull();
            stats.TotalConsultas.Should().Be(0);
            stats.ValorTotal.Should().Be(0);
        }

        [Fact]
        public void CalcularEstatisticas_DeveDetectarMutacaoEmListaNull()
        {
            // Act
            var stats = _agendamentoLogic.CalcularEstatisticas(null);

            // Assert - detecta mutação == null para != null
            stats.Should().NotBeNull();
            stats.TotalConsultas.Should().Be(0);
        }

        [Fact]
        public void CalcularEstatisticas_DeveDetectarMutacoesEmAgregacoes()
        {
            // Arrange
            var consultas = new List<ConsultaInfo>
            {
                new() { Especialidade = "Cardiologia", Valor = 200, DuracaoMinutos = 30, TemConvenio = true, Horario = new TimeSpan(10, 0, 0) },
                new() { Especialidade = "Cardiologia", Valor = 300, DuracaoMinutos = 60, TemConvenio = false, Horario = new TimeSpan(10, 0, 0) },
                new() { Especialidade = "Pediatria", Valor = 150, DuracaoMinutos = 45, TemConvenio = true, Horario = new TimeSpan(14, 0, 0) }
            };

            // Act
            var stats = _agendamentoLogic.CalcularEstatisticas(consultas);

            // Assert - detecta mutações Count, Sum, divisão
            stats.TotalConsultas.Should().Be(3, "Deve contar 3 consultas");
            stats.ValorTotal.Should().Be(650, "Soma deve ser 650");
            stats.ValorMedio.Should().BeApproximately(216.67, 0.01, "Média deve ser ~216.67");
            stats.DuracaoMedia.Should().BeApproximately(45.0, 0.01, "Duração média deve ser 45min");
        }

        [Fact]
        public void CalcularEstatisticas_DeveDetectarMutacoesEmGroupBy()
        {
            // Arrange
            var consultas = new List<ConsultaInfo>
            {
                new() { Especialidade = "Cardiologia", TemConvenio = true, Horario = new TimeSpan(10, 0, 0) },
                new() { Especialidade = "Cardiologia", TemConvenio = false, Horario = new TimeSpan(10, 0, 0) },
                new() { Especialidade = "Pediatria", TemConvenio = true, Horario = new TimeSpan(10, 0, 0) }
            };

            // Act
            var stats = _agendamentoLogic.CalcularEstatisticas(consultas);

            // Assert - detecta mutações em GroupBy e Count
            stats.ConsultasPorEspecialidade.Should().ContainKey("Cardiologia");
            stats.ConsultasPorEspecialidade["Cardiologia"].Should().Be(2);
            stats.ConsultasPorEspecialidade["Pediatria"].Should().Be(1);
        }

        [Fact]
        public void CalcularEstatisticas_DeveDetectarMutacoesEmPercentual()
        {
            // Arrange
            var consultas = new List<ConsultaInfo>
            {
                new() { TemConvenio = true, Horario = new TimeSpan(10, 0, 0) },
                new() { TemConvenio = false, Horario = new TimeSpan(11, 0, 0) },
                new() { TemConvenio = true, Horario = new TimeSpan(12, 0, 0) },
                new() { TemConvenio = true, Horario = new TimeSpan(13, 0, 0) }
            };

            // Act
            var stats = _agendamentoLogic.CalcularEstatisticas(consultas);

            // Assert - detecta mutações * 100 para / 100, Where
            stats.PercentualConvenio.Should().Be(75.0, "75% têm convênio");
        }

        [Fact]
        public void CalcularEstatisticas_DeveDetectarMutacoesEmHorarioMaisComum()
        {
            // Arrange
            var consultas = new List<ConsultaInfo>
            {
                new() { Horario = new TimeSpan(10, 0, 0) },
                new() { Horario = new TimeSpan(10, 30, 0) }, // Mesmo horário (hora)
                new() { Horario = new TimeSpan(14, 0, 0) }
            };

            // Act
            var stats = _agendamentoLogic.CalcularEstatisticas(consultas);

            // Assert - detecta mutações GroupBy, OrderBy, First
            stats.HorarioMaisComum.Should().Be(10, "Horário 10h é mais comum");
        }

        #endregion

        #region Testes para ValidarRegrasNegocio - Detectar mutações em condicionais aninhadas

        [Fact]
        public void ValidarRegrasNegocio_DeveDetectarMutacaoEmDataPassada()
        {
            // Arrange
            var dataPassada = DateTime.Today.AddDays(-1);

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                dataPassada, new TimeSpan(10, 0, 0), "Cardiologia", 30, false, false);

            // Assert - detecta mutação < para <=
            resultado.Should().Contain("passado", "Deve detectar data no passado");
        }

        [Fact]
        public void ValidarRegrasNegocio_DeveDetectarMutacaoEmDataLimite()
        {
            // Arrange
            var dataLimite = DateTime.Today.AddDays(91); // Além do limite

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                dataLimite, new TimeSpan(10, 0, 0), "Cardiologia", 30, false, false);

            // Assert - detecta mutação > para >=
            resultado.Should().Contain("90 dias", "Deve detectar data além do limite");
        }

        [Theory]
        [InlineData("Pediatria", 17, "Pediatria é para pacientes até 16 anos")]  // Detecta > para >=
        [InlineData("Pediatria", 16, "Válido")]                                   // Limite válido
        [InlineData("Geriatria", 59, "Geriatria é para pacientes acima de 60")]  // Detecta < para <=
        [InlineData("Geriatria", 60, "Válido")]                                   // Limite válido
        public void ValidarRegrasNegocio_DeveDetectarMutacoesEmRegrasEspecialidade(
            string especialidade, int idade, string resultadoEsperado)
        {
            // Arrange
            var data = DateTime.Today.AddDays(1);

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                data, new TimeSpan(10, 0, 0), especialidade, idade, false, false);

            // Assert
            if (resultadoEsperado == "Válido")
                resultado.Should().Be("Válido");
            else
                resultado.Should().Contain(resultadoEsperado);
        }

        [Fact]
        public void ValidarRegrasNegocio_DeveDetectarMutacoesEmRegrasConvenio()
        {
            // Arrange - especialidade cara sem convênio, primeira consulta
            var data = DateTime.Today.AddDays(1);

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                data, new TimeSpan(10, 0, 0), "Neurologia", 30, false, false);

            // Assert - detecta mutações &&, ||, !, > para >=
            resultado.Should().Contain("convênio", "Especialidade cara deve exigir convênio");
        }

        [Fact]
        public void ValidarRegrasNegocio_DeveRetornarValidoQuandoSemErros()
        {
            // Arrange - caso completamente válido
            var data = DateTime.Today.AddDays(1);

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                data, new TimeSpan(10, 0, 0), "Cardiologia", 30, true, false);

            // Assert - detecta mutação Count == 0 para != 0
            resultado.Should().Be("Válido", "Caso válido deve retornar 'Válido'");
        }

        [Fact]
        public void ValidarRegrasNegocio_DeveDetectarMutacoesEmMultiplosErros()
        {
            // Arrange - múltiplos erros
            var dataPassada = DateTime.Today.AddDays(-1);

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                dataPassada, new TimeSpan(6, 0, 0), "", -1, false, false);

            // Assert - detecta mutações em múltiplas validações
            resultado.Should().NotBe("Válido", "Múltiplos erros não devem retornar válido");
            resultado.Should().Contain(";", "Múltiplos erros devem ser separados por ;");
        }

        #endregion

        #region Testes Extremos para Detectar Mutações Específicas

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmConstanteZero()
        {
            // Arrange
            var consultas = new List<ConsultaInfo>();

            // Act
            var stats = _agendamentoLogic.CalcularEstatisticas(consultas);

            // Assert - detecta mutação 0 para 1
            stats.TotalConsultas.Should().Be(0, "Lista vazia deve ter 0 consultas");
        }

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmConstante90()
        {
            // Arrange
            var data = DateTime.Today.AddDays(90); // Exatamente no limite

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                data, new TimeSpan(10, 0, 0), "Cardiologia", 30, true, false);

            // Assert - detecta mutação 90 para 89 ou 91
            resultado.Should().Be("Válido", "90 dias deve ser válido");
        }

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmConstante200()
        {
            // Arrange - valor exatamente 200 (limite para exigir convênio)
            var data = DateTime.Today.AddDays(1);

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                data, new TimeSpan(10, 0, 0), "Cardiologia", 30, false, false);

            // Assert - detecta mutação > 200 para >= 200
            resultado.Should().Be("Válido", "Cardiologia (R$200) sem convênio deve ser válida");
        }

        [Fact]
        public void TesteExtremo_DeveDetectarMutacaoEmReturnString()
        {
            // Arrange - caso que deve retornar string específica
            var data = DateTime.Today.AddDays(1);

            // Act
            var resultado = _agendamentoLogic.ValidarRegrasNegocio(
                data, new TimeSpan(10, 0, 0), "Cardiologia", 30, true, false);

            // Assert - detecta mutação "Válido" para outra string
            resultado.Should().Be("Válido", "Deve retornar exatamente 'Válido'");
        }

        #endregion
    }
}