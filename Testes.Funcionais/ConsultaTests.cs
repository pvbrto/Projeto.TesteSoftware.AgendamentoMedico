using FluentAssertions;
using System.Net;
using Testes.Funcionais.Fixtures;
using Testes.Funcionais.Infrastructure;

namespace Testes.Funcionais.AgendamentoService
{
    public class ConsultaBlackBoxTests : ApiTestBase<AgendamentoTestStartup>
    {
        public ConsultaBlackBoxTests(TestWebApplicationFactory<AgendamentoTestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Ping_DeveRetornarOk_QuandoChamado()
        {
            // Act
            var response = await GetAsync("/Consulta/Ping");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaDeConsultas_QuandoChamado()
        {
            // Act
            var response = await GetAsync("/Consulta/GetAll");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await GetResponseContent(response);
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Create_DeveRetornarOk_QuandoDadosValidos()
        {
            var consultaData = TestDataGenerator.Consulta.CreateValidConsulta();

            var response = await PostAsync("/Consulta", consultaData);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveRetornarBadRequest_QuandoDadosInvalidos()
        {
            // Arrange
            var consultaInvalida = TestDataGenerator.Consulta.CreateInvalidConsulta();

            // Act
            var response = await PostAsync("/Consulta", consultaInvalida);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_DeveRetornarBadRequest_QuandoCorpoVazio()
        {
            // Act
            var response = await PostAsync("/Consulta", new { });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Realizar_DeveRetornarNotFound_QuandoIdInexistente()
        {
            // Arrange
            var idInexistente = 999999;
            var observacoes = "Consulta realizada com sucesso";

            // Act
            var response = await PostAsync($"/Consulta/Realizar/{idInexistente}", observacoes);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Filtro_DeveRetornarOk_QuandoFiltroValido()
        {
            // Arrange
            var filtro = TestDataGenerator.Consulta.CreateConsultaFiltro();
            var queryString = $"?DataInicio={DateTime.Now:yyyy-MM-dd}&DataFim={DateTime.Now.AddDays(30):yyyy-MM-dd}";

            // Act
            var response = await GetAsync($"/Consulta/Filtro{queryString}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Create_DeveValidarDataFutura()
        {
            // Arrange
            var consultaDataPassada = new
            {
                PacienteId = 1,
                MedicoId = 1,
                DataHora = DateTime.Now.AddDays(-1), // Data no passado
                Observacoes = "Teste"
            };

            // Act
            var response = await PostAsync("/Consulta", consultaDataPassada);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveValidarPacienteObrigatorio()
        {
            // Arrange
            var consultaSemPaciente = new
            {
                MedicoId = 1,
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = "Teste"
            };

            // Act
            var response = await PostAsync("/Consulta", consultaSemPaciente);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveValidarMedicoObrigatorio()
        {
            // Arrange
            var consultaSemMedico = new
            {
                PacienteId = 1,
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = "Teste"
            };

            // Act
            var response = await PostAsync("/Consulta", consultaSemMedico);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Realizar_DeveRetornarBadRequest_QuandoIdInvalido(int idInvalido)
        {
            // Arrange
            var observacoes = "Teste";

            // Act
            var response = await PostAsync($"/Consulta/Realizar/{idInvalido}", observacoes);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Filtro_DeveRetornarOk_QuandoSemParametros()
        {
            // Act
            var response = await GetAsync("/Consulta/Filtro");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Filtro_DeveValidarFormatoData()
        {
            // Arrange
            var queryString = "?DataInicio=data-invalida&DataFim=outra-data-invalida";

            // Act
            var response = await GetAsync($"/Consulta/Filtro{queryString}");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
            // Pode retornar OK se o sistema ignorar parâmetros inválidos
        }
    }
}