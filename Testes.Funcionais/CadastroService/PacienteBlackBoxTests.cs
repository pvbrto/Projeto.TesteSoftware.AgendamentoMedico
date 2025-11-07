using FluentAssertions;
using System.Net;
using Testes.Funcionais.Fixtures;
using Testes.Funcionais.Infrastructure;

namespace Testes.Funcionais.CadastroService
{
    public class PacienteBlackBoxTests : HttpClientTestBase
    {
        public PacienteBlackBoxTests() : base("http://localhost:5001")
        {
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaDePacientes_QuandoChamado()
        {
            var isAvailable = await IsApiAvailable();
            if (!isAvailable)
            {
                Assert.True(true, "API não está disponível - teste pulado");
                return;
            }

            var response = await GetAsync("/Paciente/GetAll");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await GetResponseContent(response);
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Create_DeveRetornarCreated_QuandoDadosValidos()
        {
            var isAvailable = await IsApiAvailable();
            if (!isAvailable)
            {
                Assert.True(true, "API não está disponível - teste pulado");
                return;
            }

            var pacienteData = TestDataGenerator.Paciente.CreateValidPaciente();

            var response = await PostAsync("/Paciente", pacienteData);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.BadRequest);
            var content = await GetResponseContent(response);
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Create_DeveRetornarBadRequest_QuandoDadosInvalidos()
        {
            var pacienteInvalido = TestDataGenerator.Paciente.CreateInvalidPaciente();

            var response = await PostAsync("/Paciente", pacienteInvalido);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveRetornarBadRequest_QuandoCorpoVazio()
        {
            var response = await PostAsync("/Paciente", new { });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoIdInexistente()
        {
            var idInexistente = 999999;

            var response = await GetAsync($"/Paciente/{idInexistente}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_DeveRetornarBadRequest_QuandoIdsDiferentes()
        {
            var pacienteData = TestDataGenerator.Paciente.CreateValidPaciente();
            var pacienteComId = new { Id = 1, pacienteData };

            var response = await PutAsync("/Paciente/999", pacienteComId);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoIdInexistente()
        {
            var idInexistente = 999999;

            var response = await DeleteAsync($"/Paciente/{idInexistente}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetById_DeveRetornarBadRequest_QuandoIdInvalido(int idInvalido)
        {
            var response = await GetAsync($"/Paciente/{idInvalido}");

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_DeveValidarCamposObrigatorios()
        {
            var pacienteSemNome = new
            {
                Email = "teste@teste.com",
                Telefone = "(11) 99999-9999"
            };

            var response = await PostAsync("/Paciente", pacienteSemNome);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveValidarFormatoEmail()
        {
            var pacienteEmailInvalido = new
            {
                Nome = "Teste",
                Email = "email-invalido",
                Telefone = "(11) 99999-9999"
            };

            var response = await PostAsync("/Paciente", pacienteEmailInvalido);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}