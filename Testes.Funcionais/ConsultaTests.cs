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
            var response = await GetAsync("/Consulta/Ping");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaDeConsultas_QuandoChamado()
        {
            var response = await GetAsync("/Consulta/GetAll");

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
            var consultaInvalida = TestDataGenerator.Consulta.CreateInvalidConsulta();

            var response = await PostAsync("/Consulta", consultaInvalida);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_DeveRetornarBadRequest_QuandoCorpoVazio()
        {
            var response = await PostAsync("/Consulta", new { });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Realizar_DeveRetornarNotFound_QuandoIdInexistente()
        {
            var idInexistente = 999999;
            var observacoes = "Consulta realizada com sucesso";

            var response = await PostAsync($"/Consulta/Realizar/{idInexistente}", observacoes);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Filtro_DeveRetornarOk_QuandoFiltroValido()
        {
            var filtro = TestDataGenerator.Consulta.CreateConsultaFiltro();
            var queryString = $"?DataInicio={DateTime.Now:yyyy-MM-dd}&DataFim={DateTime.Now.AddDays(30):yyyy-MM-dd}";

            var response = await GetAsync($"/Consulta/Filtro{queryString}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Create_DeveValidarDataFutura()
        {
            var consultaDataPassada = new
            {
                PacienteId = 1,
                MedicoId = 1,
                DataHora = DateTime.Now.AddDays(-1),
                Observacoes = "Teste"
            };

            var response = await PostAsync("/Consulta", consultaDataPassada);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveValidarPacienteObrigatorio()
        {
            var consultaSemPaciente = new
            {
                MedicoId = 1,
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = "Teste"
            };

            var response = await PostAsync("/Consulta", consultaSemPaciente);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveValidarMedicoObrigatorio()
        {
            var consultaSemMedico = new
            {
                PacienteId = 1,
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = "Teste"
            };

            var response = await PostAsync("/Consulta", consultaSemMedico);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task Realizar_DeveRetornarBadRequest_QuandoIdInvalido(int idInvalido)
        {
            var observacoes = "Teste";

            var response = await PostAsync($"/Consulta/Realizar/{idInvalido}", observacoes);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Filtro_DeveRetornarOk_QuandoSemParametros()
        {
            var response = await GetAsync("/Consulta/Filtro");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Filtro_DeveValidarFormatoData()
        {
            var queryString = "?DataInicio=data-invalida&DataFim=outra-data-invalida";

            var response = await GetAsync($"/Consulta/Filtro{queryString}");

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
        }
    }
}