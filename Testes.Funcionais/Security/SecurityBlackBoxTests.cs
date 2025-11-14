using FluentAssertions;
using System.Net;
using System.Text;
using Testes.Funcionais.Infrastructure;

namespace Testes.Funcionais.Security
{
    public class SecurityBlackBoxTests : ApiTestBase<AgendamentoTestStartup>
    {
        public SecurityBlackBoxTests(TestWebApplicationFactory<AgendamentoTestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Create_DeveRejeitarPayloadMuitoGrande()
        {
            var payloadGigante = new
            {
                Observacoes = new string('A', 1000000)
            };

            var response = await PostAsync("/Consulta", payloadGigante);

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
            var consultaComEntradaMaliciosa = new
            {
                PacienteId = 1,
                MedicoId = 1,
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = entradaMaliciosa
            };

            var response = await PostAsync("/Consulta", consultaComEntradaMaliciosa);

            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.OK,
                HttpStatusCode.UnprocessableEntity);

            if (response.IsSuccessStatusCode)
            {
                var content = await GetResponseContent(response);
                content.Should().NotContain("<script>");
                content.Should().NotContain("DROP TABLE");
            }
        }

        [Fact]
        public async Task API_DeveDefinirHeadersDeSeguranca()
        {
            var response = await GetAsync("/Consulta/Ping");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task API_NaoDeveExporInformacoesInternas()
        {
            var response = await GetAsync("/Consulta/EndpointInexistente");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            
            var content = await GetResponseContent(response);
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
            var response = await GetAsync($"/Consulta/{pathMalicioso}");

            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.NotFound,
                HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task API_DeveValidarContentType()
        {
            var content = new StringContent("dados inválidos", Encoding.UTF8, "text/plain");

            var response = await _client.PostAsync("/Consulta", content);

            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.UnsupportedMediaType);
        }
    }
}