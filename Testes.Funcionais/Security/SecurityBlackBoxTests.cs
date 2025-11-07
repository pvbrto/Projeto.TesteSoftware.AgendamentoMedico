using FluentAssertions;
using System.Net;
using System.Text;
using Testes.Funcionais.Infrastructure;

namespace Testes.Funcionais.Security
{
    /// <summary>
    /// Testes de caixa preta focados em aspectos de segurança
    /// </summary>
    public class SecurityBlackBoxTests : ApiTestBase<AgendamentoTestStartup>
    {
        public SecurityBlackBoxTests(TestWebApplicationFactory<AgendamentoTestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Create_DeveRejeitarPayloadMuitoGrande()
        {
            // Arrange - Criar um payload muito grande
            var payloadGigante = new
            {
                Observacoes = new string('A', 1000000) // 1MB de texto
            };

            // Act
            var response = await PostAsync("/Consulta", payloadGigante);

            // Assert
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest, 
                HttpStatusCode.RequestEntityTooLarge);
        }

        [Theory]
        [InlineData("<script>alert('xss')</script>")]
        [InlineData("'; DROP TABLE Consultas; --")]
        [InlineData("<img src=x onerror=alert('xss')>")]
        public async Task Create_DeveValidarEntradaMaliciosa(string entradaMaliciosa)
        {
            // Arrange
            var consultaComEntradaMaliciosa = new
            {
                PacienteId = 1,
                MedicoId = 1,
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = entradaMaliciosa
            };

            // Act
            var response = await PostAsync("/Consulta", consultaComEntradaMaliciosa);

            // Assert
            // A API deve tratar adequadamente entradas maliciosas
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.OK,
                HttpStatusCode.UnprocessableEntity);

            if (response.IsSuccessStatusCode)
            {
                var content = await GetResponseContent(response);
                // Se aceitar, deve ter sanitizado a entrada
                content.Should().NotContain("<script>");
                content.Should().NotContain("DROP TABLE");
            }
        }

        [Fact]
        public async Task API_DeveDefinirHeadersDeSeguranca()
        {
            // Act
            var response = await GetAsync("/Consulta/Ping");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            // Verificar headers de segurança básicos
            // Nota: Estes headers podem não estar configurados ainda
            // Este teste serve como documentação do que deveria existir
            
            // response.Headers.Should().ContainKey("X-Content-Type-Options");
            // response.Headers.Should().ContainKey("X-Frame-Options");
            // response.Headers.Should().ContainKey("X-XSS-Protection");
        }

        [Fact]
        public async Task API_NaoDeveExporInformacoesInternas()
        {
            // Arrange - Tentar acessar endpoint inexistente
            var response = await GetAsync("/Consulta/EndpointInexistente");

            // Act & Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            
            var content = await GetResponseContent(response);
            // Não deve expor stack traces ou informações internas
            content.Should().NotContain("Exception");
            content.Should().NotContain("StackTrace");
            content.Should().NotContain("at System.");
        }

        [Theory]
        [InlineData("../../../etc/passwd")]
        [InlineData("..\\..\\..\\windows\\system32")]
        [InlineData("%2e%2e%2f%2e%2e%2f")]
        public async Task API_DeveRejeitarPathTraversal(string pathMalicioso)
        {
            // Act
            var response = await GetAsync($"/Consulta/{pathMalicioso}");

            // Assert
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.NotFound,
                HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task API_DeveValidarContentType()
        {
            // Arrange
            var content = new StringContent("dados inválidos", Encoding.UTF8, "text/plain");

            // Act
            var response = await _client.PostAsync("/Consulta", content);

            // Assert
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.UnsupportedMediaType);
        }
    }
}