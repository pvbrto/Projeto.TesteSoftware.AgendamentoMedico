using FluentAssertions;
using System.Diagnostics;
using System.Net;
using Testes.Funcionais.Infrastructure;

namespace Testes.Funcionais.Performance
{
    /// <summary>
    /// Testes de caixa preta focados em performance e comportamento sob carga
    /// </summary>
    public class PerformanceBlackBoxTests : ApiTestBase<AgendamentoTestStartup>
    {
        public PerformanceBlackBoxTests(TestWebApplicationFactory<AgendamentoTestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_DeveResponderRapidamente_QuandoChamadoMultiplasVezes()
        {
            // Arrange
            var stopwatch = new Stopwatch();
            var temposResposta = new List<long>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Restart();
                var response = await GetAsync("/Consulta/GetAll");
                stopwatch.Stop();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                temposResposta.Add(stopwatch.ElapsedMilliseconds);
            }

            // Assert
            var tempoMedio = temposResposta.Average();
            tempoMedio.Should().BeLessThan(5000, "API deve responder em menos de 5 segundos em média");

            var tempoMaximo = temposResposta.Max();
            tempoMaximo.Should().BeLessThan(10000, "Nenhuma resposta deve demorar mais de 10 segundos");
        }

        [Fact]
        public async Task Ping_DeveResponderRapidamente()
        {
            // Arrange
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            var response = await GetAsync("/Consulta/Ping");
            stopwatch.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Ping deve responder em menos de 1 segundo");
        }

        [Fact]
        public async Task ChamadaSimultanea_DeveManterConsistencia()
        {
            // Arrange
            var tasks = new List<Task<HttpResponseMessage>>();

            // Act - Fazer 5 chamadas simultâneas
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(GetAsync("/Consulta/GetAll"));
            }

            var responses = await Task.WhenAll(tasks);

            // Assert
            foreach (var response in responses)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task Filtro_DeveManterPerformance_ComParametrosComplexos()
        {
            // Arrange
            var stopwatch = new Stopwatch();
            var queryString = $"?DataInicio={DateTime.Now.AddMonths(-6):yyyy-MM-dd}&DataFim={DateTime.Now.AddMonths(6):yyyy-MM-dd}";

            // Act
            stopwatch.Start();
            var response = await GetAsync($"/Consulta/Filtro{queryString}");
            stopwatch.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Filtro complexo deve responder em menos de 5 segundos");
        }
    }
}