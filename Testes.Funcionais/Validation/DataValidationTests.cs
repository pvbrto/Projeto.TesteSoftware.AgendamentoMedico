using FluentAssertions;
using Testes.Funcionais.Fixtures;

namespace Testes.Funcionais.Validation
{
    public class DataValidationTests
    {
        [Fact]
        public void PacienteValido_DeveConterCamposObrigatorios()
        {
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(paciente);

            pacienteObj["Nome"]?.ToString().Should().NotBeNullOrEmpty();
            pacienteObj["Email"]?.ToString().Should().NotBeNullOrEmpty();
            pacienteObj["Telefone"]?.ToString().Should().NotBeNullOrEmpty();
            pacienteObj["Cpf"]?.ToString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PacienteValido_DeveConterEmailValido()
        {
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(paciente);
            var email = pacienteObj["Email"]?.ToString();

            email.Should().NotBeNullOrEmpty();
            email.Should().Contain("@");
            email.Should().Contain(".");
        }

        [Fact]
        public void PacienteValido_DeveConterCpfFormatado()
        {
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(paciente);
            var cpf = pacienteObj["Cpf"]?.ToString();

            cpf.Should().NotBeNullOrEmpty();
            cpf.Should().MatchRegex(@"\d{3}\.\d{3}\.\d{3}-\d{2}");
        }

        [Fact]
        public void PacienteInvalido_DeveConterDadosIncorretos()
        {
            var pacienteInvalido = TestDataGenerator.Paciente.CreateInvalidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(pacienteInvalido);

            var nome = pacienteObj["Nome"]?.ToString();
            var email = pacienteObj["Email"]?.ToString();

            nome.Should().BeNullOrEmpty();
            email.Should().NotContain("@");
        }

        [Fact]
        public void MedicoValido_DeveConterCamposObrigatorios()
        {
            var medico = TestDataGenerator.Medico.CreateValidMedico();
            var medicoObj = Newtonsoft.Json.Linq.JObject.FromObject(medico);

            medicoObj["Nome"]?.ToString().Should().NotBeNullOrEmpty();
            medicoObj["Email"]?.ToString().Should().NotBeNullOrEmpty();
            medicoObj["Crm"]?.ToString().Should().NotBeNullOrEmpty();
            var especialidadeId = medicoObj["EspecialidadeId"]?.ToObject<int>();
            especialidadeId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ConsultaValida_DeveConterCamposObrigatorios()
        {
            var consulta = TestDataGenerator.Consulta.CreateValidConsulta();
            var consultaObj = Newtonsoft.Json.Linq.JObject.FromObject(consulta);

            var pacienteId = consultaObj["PacienteId"]?.ToObject<int>();
            var medicoId = consultaObj["MedicoId"]?.ToObject<int>();
            var dataHora = consultaObj["DataHora"]?.ToObject<DateTime>();
            
            pacienteId.Should().BeGreaterThan(0);
            medicoId.Should().BeGreaterThan(0);
            dataHora.Should().BeAfter(DateTime.Now);
        }

        [Fact]
        public void ConsultaInvalida_DeveConterDadosIncorretos()
        {
            var consultaInvalida = TestDataGenerator.Consulta.CreateInvalidConsulta();
            var consultaObj = Newtonsoft.Json.Linq.JObject.FromObject(consultaInvalida);

            var dataHora = consultaObj["DataHora"]?.ToObject<DateTime>();
            dataHora.Should().NotBeNull();
            if (dataHora.HasValue)
            {
                dataHora.Value.Should().BeBefore(DateTime.Now);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void TestDataGenerator_DeveGerarDadosDiferentes(int quantidade)
        {
            var pacientes = new List<object>();
            for (int i = 0; i < quantidade; i++)
            {
                pacientes.Add(TestDataGenerator.Paciente.CreateValidPaciente());
            }

            pacientes.Should().HaveCount(quantidade);
            pacientes.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void FiltroConsulta_DeveConterParametrosValidos()
        {
            var filtro = TestDataGenerator.Consulta.CreateConsultaFiltro();
            var filtroObj = Newtonsoft.Json.Linq.JObject.FromObject(filtro);

            var dataInicio = filtroObj["DataInicio"]?.ToObject<DateTime>();
            var dataFim = filtroObj["DataFim"]?.ToObject<DateTime>();

            dataInicio.Should().NotBeNull();
            dataFim.Should().NotBeNull();
            dataInicio.Should().NotBe(default(DateTime));
            dataFim.Should().NotBe(default(DateTime));
            if (dataInicio.HasValue && dataFim.HasValue)
            {
                dataFim.Value.Should().BeAfter(dataInicio.Value);
            }
        }

        [Fact]
        public void JsonSerialization_DeveFuncionarCorretamente()
        {
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(paciente);
            var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            json.Should().NotBeNullOrEmpty();
            json.Should().Contain("Nome");
            json.Should().Contain("Email");
            deserialized.Should().NotBeNull();
        }
    }
}