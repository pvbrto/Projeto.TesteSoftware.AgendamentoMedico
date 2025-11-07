using FluentAssertions;
using Testes.Funcionais.Fixtures;

namespace Testes.Funcionais.Validation
{
    /// <summary>
    /// Testes de validação de dados que não dependem das APIs
    /// Estes testes sempre funcionam e validam a lógica de geração de dados
    /// </summary>
    public class DataValidationTests
    {
        [Fact]
        public void PacienteValido_DeveConterCamposObrigatorios()
        {
            // Arrange & Act
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(paciente);

            // Assert
            pacienteObj["Nome"]?.ToString().Should().NotBeNullOrEmpty();
            pacienteObj["Email"]?.ToString().Should().NotBeNullOrEmpty();
            pacienteObj["Telefone"]?.ToString().Should().NotBeNullOrEmpty();
            pacienteObj["Cpf"]?.ToString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PacienteValido_DeveConterEmailValido()
        {
            // Arrange & Act
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(paciente);
            var email = pacienteObj["Email"]?.ToString();

            // Assert
            email.Should().NotBeNullOrEmpty();
            email.Should().Contain("@");
            email.Should().Contain(".");
        }

        [Fact]
        public void PacienteValido_DeveConterCpfFormatado()
        {
            // Arrange & Act
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(paciente);
            var cpf = pacienteObj["Cpf"]?.ToString();

            // Assert
            cpf.Should().NotBeNullOrEmpty();
            cpf.Should().MatchRegex(@"\d{3}\.\d{3}\.\d{3}-\d{2}");
        }

        [Fact]
        public void PacienteInvalido_DeveConterDadosIncorretos()
        {
            // Arrange & Act
            var pacienteInvalido = TestDataGenerator.Paciente.CreateInvalidPaciente();
            var pacienteObj = Newtonsoft.Json.Linq.JObject.FromObject(pacienteInvalido);

            // Assert
            var nome = pacienteObj["Nome"]?.ToString();
            var email = pacienteObj["Email"]?.ToString();

            nome.Should().BeNullOrEmpty(); // Nome deve estar vazio para ser inválido
            email.Should().NotContain("@"); // Email deve estar malformado
        }

        [Fact]
        public void MedicoValido_DeveConterCamposObrigatorios()
        {
            // Arrange & Act
            var medico = TestDataGenerator.Medico.CreateValidMedico();
            var medicoObj = Newtonsoft.Json.Linq.JObject.FromObject(medico);

            // Assert
            medicoObj["Nome"]?.ToString().Should().NotBeNullOrEmpty();
            medicoObj["Email"]?.ToString().Should().NotBeNullOrEmpty();
            medicoObj["Crm"]?.ToString().Should().NotBeNullOrEmpty();
            var especialidadeId = medicoObj["EspecialidadeId"]?.ToObject<int>();
            especialidadeId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ConsultaValida_DeveConterCamposObrigatorios()
        {
            // Arrange & Act
            var consulta = TestDataGenerator.Consulta.CreateValidConsulta();
            var consultaObj = Newtonsoft.Json.Linq.JObject.FromObject(consulta);

            // Assert
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
            // Arrange & Act
            var consultaInvalida = TestDataGenerator.Consulta.CreateInvalidConsulta();
            var consultaObj = Newtonsoft.Json.Linq.JObject.FromObject(consultaInvalida);

            // Assert
            var dataHora = consultaObj["DataHora"]?.ToObject<DateTime>();
            dataHora.Should().NotBeNull();
            if (dataHora.HasValue)
            {
                dataHora.Value.Should().BeBefore(DateTime.Now); // Data deve estar no passado para ser inválida
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void TestDataGenerator_DeveGerarDadosDiferentes(int quantidade)
        {
            // Arrange & Act
            var pacientes = new List<object>();
            for (int i = 0; i < quantidade; i++)
            {
                pacientes.Add(TestDataGenerator.Paciente.CreateValidPaciente());
            }

            // Assert
            pacientes.Should().HaveCount(quantidade);
            pacientes.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void FiltroConsulta_DeveConterParametrosValidos()
        {
            // Arrange & Act
            var filtro = TestDataGenerator.Consulta.CreateConsultaFiltro();
            var filtroObj = Newtonsoft.Json.Linq.JObject.FromObject(filtro);

            // Assert
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
            // Arrange
            var paciente = TestDataGenerator.Paciente.CreateValidPaciente();

            // Act
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(paciente);
            var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            // Assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Contain("Nome");
            json.Should().Contain("Email");
            deserialized.Should().NotBeNull();
        }
    }
}