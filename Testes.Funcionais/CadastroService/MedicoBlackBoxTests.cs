using FluentAssertions;
using System.Net;
using Testes.Funcionais.Fixtures;
using Testes.Funcionais.Infrastructure;

namespace Testes.Funcionais.CadastroService
{
    public class MedicoBlackBoxTests : ApiTestBase<CadastroTestStartup>
    {
        public MedicoBlackBoxTests(TestWebApplicationFactory<CadastroTestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaDeMedicos_QuandoChamado()
        {
            var response = await GetAsync("/Medico/GetAll");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await GetResponseContent(response);
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Create_DeveRetornarCreated_QuandoDadosValidos()
        {
            var medicoData = TestDataGenerator.Medico.CreateValidMedico();

            var response = await PostAsync("/Medico", medicoData);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        }

        [Fact]
        public async Task Create_DeveRetornarBadRequest_QuandoDadosInvalidos()
        {
            var medicoInvalido = TestDataGenerator.Medico.CreateInvalidMedico();

            var response = await PostAsync("/Medico", medicoInvalido);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoIdInexistente()
        {
            var idInexistente = 999999;

            var response = await GetAsync($"/Medico/{idInexistente}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetByEspecialidade_DeveRetornarOk_QuandoEspecialidadeExiste()
        {
            var especialidadeId = 1;

            var response = await GetAsync($"/Medico/GetByEspecialidade/{especialidadeId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoIdInexistente()
        {
            var idInexistente = 999999;

            var response = await DeleteAsync($"/Medico/{idInexistente}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_DeveValidarCrmObrigatorio()
        {
            var medicoSemCrm = new
            {
                Nome = "Dr. Teste",
                Email = "dr.teste@teste.com",
                Telefone = "(11) 99999-9999",
                EspecialidadeId = 1
            };

            var response = await PostAsync("/Medico", medicoSemCrm);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_DeveValidarEspecialidadeObrigatoria()
        {
            var medicoSemEspecialidade = new
            {
                Nome = "Dr. Teste",
                Email = "dr.teste@teste.com",
                Telefone = "(11) 99999-9999",
                Crm = "12345"
            };

            var response = await PostAsync("/Medico", medicoSemEspecialidade);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("ABCDE")]
        public async Task Create_DeveValidarFormatoCrm(string crmInvalido)
        {
            var medicoComCrmInvalido = new
            {
                Nome = "Dr. Teste",
                Email = "dr.teste@teste.com",
                Telefone = "(11) 99999-9999",
                Crm = crmInvalido,
                EspecialidadeId = 1
            };

            var response = await PostAsync("/Medico", medicoComCrmInvalido);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_DeveRetornarBadRequest_QuandoIdsDiferentes()
        {
            var medicoData = TestDataGenerator.Medico.CreateValidMedico();
            var medicoComId = new { Id = 1, medicoData };

            var response = await PutAsync("/Medico/999", medicoComId);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}