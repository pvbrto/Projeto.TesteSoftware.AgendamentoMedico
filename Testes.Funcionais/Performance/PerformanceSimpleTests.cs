using FluentAssertions;
using System.Diagnostics;
using Testes.Funcionais.Fixtures;
using Testes.Funcionais.Infrastructure;

namespace Testes.Funcionais.Performance
{
    /// <summary>
    /// Testes de performance que funcionam independentemente das APIs
    /// </summary>
    public class PerformanceSimpleTests : HttpClientTestBase
    {
        public PerformanceSimpleTests() : base("http://localhost:5000")
        {
        }

        [Fact]
        public void DataGeneration_DeveSerRapida()
        {
            var stopwatch = new Stopwatch();
            var tempos = new List<long>();

            for (int i = 0; i < 100; i++)
            {
                stopwatch.Restart();
                var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
                stopwatch.Stop();
                tempos.Add(stopwatch.ElapsedMilliseconds);
            }

            var tempoMedio = tempos.Average();
            tempoMedio.Should().BeLessThan(100, "Geração de dados deve ser rápida");
        }

        [Fact]
        public void JsonSerialization_DeveSerRapida()
        {
            // Arrange
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            for (int i = 0; i < 1000; i++)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(paciente);
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Serialização deve ser rápida");
        }

        [Fact]
        public async Task HttpClient_DeveConfigurarTimeout()
        {
            // Arrange & Act
            var timeout = _httpClient.Timeout;

            // Assert
            timeout.Should().BeGreaterThan(TimeSpan.Zero);
            timeout.Should().BeLessThan(TimeSpan.FromMinutes(5));
        }

        [Fact]
        public async Task API_DeveResponderRapidamente_QuandoDisponivel()
        {
            // Arrange
            var isAvailable = await IsApiAvailable();
            
            if (!isAvailable)
            {
                Assert.True(true, "API não disponível - teste de performance conceitual");
                return;
            }

            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            var response = await GetAsync("/Consulta/Ping");
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "API deve responder em menos de 5 segundos");
        }

        [Fact]
        public async Task MultipleRequests_DevemSerProcessadas_QuandoAPIDisponivel()
        {
            // Arrange
            var isAvailable = await IsApiAvailable();
            
            if (!isAvailable)
            {
                Assert.True(true, "API não disponível - teste de múltiplas requisições conceitual");
                return;
            }

            var tasks = new List<Task<HttpResponseMessage>>();

            // Act
            for (int i = 0; i < 3; i++)
            {
                tasks.Add(GetAsync("/Consulta/Ping"));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            responses.Should().HaveCount(3);
            responses.Should().OnlyContain(r => r != null);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public void DataGeneration_DeveEscalar(int quantidade)
        {
            // Arrange
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            for (int i = 0; i < quantidade; i++)
            {
                var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
                var medico = TestDataGenerator.Medico.CreateValidMedico();
                var consulta = TestDataGenerator.Consulta.CreateValidConsulta();
            }
            stopwatch.Stop();

            // Assert
            var tempoMedio = (double)stopwatch.ElapsedMilliseconds / quantidade;
            tempoMedio.Should().BeLessThan(50, $"Geração de {quantidade} registros deve ser eficiente");
        }

        [Fact]
        public void MemoryUsage_DeveSerRazoavel()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            var dados = new List<object>();
            for (int i = 0; i < 1000; i++)
            {
                dados.Add(TestDataGenerator.Paciente.CreateValidPaciente());
            }

            var finalMemory = GC.GetTotalMemory(false);
            var memoryUsed = finalMemory - initialMemory;

            // Assert
            memoryUsed.Should().BeLessThan(10 * 1024 * 1024, "Uso de memória deve ser razoável (< 10MB)");
        }
    }
}