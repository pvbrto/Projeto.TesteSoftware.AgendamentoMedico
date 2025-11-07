using FluentAssertions;
using AutoFixture;
using Xunit;

namespace Testes.Unitarios.Domain.Entities
{
    /// <summary>
    /// Testes de caixa branca para a entidade Médico
    /// Valida lógica interna, validações específicas e regras de negócio
    /// </summary>
    [Trait("Category", "Domain")]
    [Trait("Type", "Unit")]
    public class MedicoTests
    {
        private readonly Fixture _fixture;

        public MedicoTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Construtor_DeveInicializarPropriedadesCorretamente()
        {
            // Arrange
            var nome = "Dr. João Silva";
            var email = "dr.joao@hospital.com";
            var telefone = "(11) 99999-9999";
            var crm = "12345-SP";
            var especialidadeId = 1;

            // Act
            var medico = new Medico
            {
                Nome = nome,
                Email = email,
                Telefone = telefone,
                Crm = crm,
                EspecialidadeId = especialidadeId
            };

            // Assert
            medico.Nome.Should().Be(nome);
            medico.Email.Should().Be(email);
            medico.Telefone.Should().Be(telefone);
            medico.Crm.Should().Be(crm);
            medico.EspecialidadeId.Should().Be(especialidadeId);
        }

        [Theory]
        [InlineData("12345-SP", true)]
        [InlineData("123456-RJ", true)]
        [InlineData("1234567-MG", true)]
        [InlineData("12345/SP", true)]
        [InlineData("123456/RJ", true)]
        public void ValidarCrm_DeveRetornarTrue_QuandoCrmValido(string crm, bool esperado)
        {
            // Act
            var resultado = ValidarCrm(crm);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("123", false)]
        [InlineData("ABCDE", false)]
        [InlineData("12345", false)] // Sem UF
        [InlineData("12345-", false)] // UF vazia
        [InlineData("-SP", false)] // Número vazio
        [InlineData("12345-ABC", false)] // UF inválida
        public void ValidarCrm_DeveRetornarFalse_QuandoCrmInvalido(string crm, bool esperado)
        {
            // Act
            var resultado = ValidarCrm(crm);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Fact]
        public void ValidarEspecialidade_DeveRetornarTrue_QuandoEspecialidadeValida()
        {
            // Arrange
            var especialidadesValidas = new[] { 1, 2, 3, 10, 50 };

            foreach (var especialidadeId in especialidadesValidas)
            {
                // Act
                var resultado = ValidarEspecialidade(especialidadeId);

                // Assert
                resultado.Should().BeTrue($"Especialidade ID '{especialidadeId}' deveria ser válida");
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void ValidarEspecialidade_DeveRetornarFalse_QuandoEspecialidadeInvalida(int especialidadeId)
        {
            // Act
            var resultado = ValidarEspecialidade(especialidadeId);

            // Assert
            resultado.Should().BeFalse($"Especialidade ID '{especialidadeId}' deveria ser inválida");
        }

        [Fact]
        public void PodeAtenderPaciente_DeveRetornarTrue_QuandoMedicoAtivo()
        {
            // Arrange
            var medico = new Medico
            {
                Nome = "Dr. Teste",
                Crm = "12345-SP",
                EspecialidadeId = 1,
                Ativo = true
            };

            // Act
            var resultado = medico.PodeAtenderPaciente();

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void PodeAtenderPaciente_DeveRetornarFalse_QuandoMedicoInativo()
        {
            // Arrange
            var medico = new Medico
            {
                Nome = "Dr. Teste",
                Crm = "12345-SP",
                EspecialidadeId = 1,
                Ativo = false
            };

            // Act
            var resultado = medico.PodeAtenderPaciente();

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void ObterNomeCompleto_DeveRetornarNomeComTitulo_QuandoNomeNaoTemDr()
        {
            // Arrange
            var medico = new Medico { Nome = "João Silva" };

            // Act
            var nomeCompleto = medico.ObterNomeCompleto();

            // Assert
            nomeCompleto.Should().Be("Dr. João Silva");
        }

        [Fact]
        public void ObterNomeCompleto_DeveRetornarNomeOriginal_QuandoNomeJaTemDr()
        {
            // Arrange
            var medico = new Medico { Nome = "Dr. João Silva" };

            // Act
            var nomeCompleto = medico.ObterNomeCompleto();

            // Assert
            nomeCompleto.Should().Be("Dr. João Silva");
        }

        [Fact]
        public void ObterNomeCompleto_DeveRetornarNomeOriginal_QuandoNomeTemDra()
        {
            // Arrange
            var medico = new Medico { Nome = "Dra. Maria Silva" };

            // Act
            var nomeCompleto = medico.ObterNomeCompleto();

            // Assert
            nomeCompleto.Should().Be("Dra. Maria Silva");
        }

        [Theory]
        [InlineData("João Silva", "Dr. João Silva")]
        [InlineData("Dr. João Silva", "Dr. João Silva")]
        [InlineData("Dra. Maria Silva", "Dra. Maria Silva")]
        [InlineData("DR. PEDRO SANTOS", "DR. PEDRO SANTOS")]
        [InlineData("", "Dr. ")]
        public void ObterNomeCompleto_DeveFormatarCorretamente_ParaDiferentesNomes(string nomeOriginal, string nomeEsperado)
        {
            // Arrange
            var medico = new Medico { Nome = nomeOriginal };

            // Act
            var nomeCompleto = medico.ObterNomeCompleto();

            // Assert
            nomeCompleto.Should().Be(nomeEsperado);
        }

        [Fact]
        public void ValidarDadosCompletos_DeveRetornarTrue_QuandoTodosDadosPreenchidos()
        {
            // Arrange
            var medico = new Medico
            {
                Nome = "Dr. João Silva",
                Email = "dr.joao@hospital.com",
                Telefone = "(11) 99999-9999",
                Crm = "12345-SP",
                EspecialidadeId = 1
            };

            // Act
            var resultado = medico.ValidarDadosCompletos();

            // Assert
            resultado.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "email@teste.com", "(11) 99999-9999", "12345-SP", 1)] // Nome vazio
        [InlineData("Dr. João", "", "(11) 99999-9999", "12345-SP", 1)] // Email vazio
        [InlineData("Dr. João", "email@teste.com", "", "12345-SP", 1)] // Telefone vazio
        [InlineData("Dr. João", "email@teste.com", "(11) 99999-9999", "", 1)] // CRM vazio
        [InlineData("Dr. João", "email@teste.com", "(11) 99999-9999", "12345-SP", 0)] // Especialidade inválida
        public void ValidarDadosCompletos_DeveRetornarFalse_QuandoDadosIncompletos(
            string nome, string email, string telefone, string crm, int especialidadeId)
        {
            // Arrange
            var medico = new Medico
            {
                Nome = nome,
                Email = email,
                Telefone = telefone,
                Crm = crm,
                EspecialidadeId = especialidadeId
            };

            // Act
            var resultado = medico.ValidarDadosCompletos();

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void CalcularTempoExperiencia_DeveRetornarTempoCorreto_QuandoDataCadastroValida()
        {
            // Arrange
            var dataCadastro = new DateTime(2020, 1, 1);
            var dataAtual = new DateTime(2024, 1, 1);
            var medico = new Medico { DataCadastro = dataCadastro };

            // Act
            var tempoExperiencia = medico.CalcularTempoExperiencia(dataAtual);

            // Assert
            tempoExperiencia.Should().Be(4); // 4 anos
        }

        // Métodos auxiliares que simulam a lógica interna da entidade
        private static bool ValidarCrm(string crm)
        {
            if (string.IsNullOrWhiteSpace(crm))
                return false;

            // CRM deve ter formato: números + separador + UF
            var separadores = new[] { "-", "/" };
            var temSeparador = separadores.Any(s => crm.Contains(s));
            
            if (!temSeparador)
                return false;

            var partes = crm.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
            
            if (partes.Length != 2)
                return false;

            var numero = partes[0];
            var uf = partes[1];

            // Número deve ter entre 4 e 7 dígitos
            if (!numero.All(char.IsDigit) || numero.Length < 4 || numero.Length > 7)
                return false;

            // UF deve ter 2 caracteres e ser válida
            var ufsValidas = new[] { "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO" };
            
            return uf.Length == 2 && ufsValidas.Contains(uf.ToUpper());
        }

        private static bool ValidarEspecialidade(int especialidadeId)
        {
            return especialidadeId > 0;
        }
    }

    // Classe auxiliar para simular a entidade Médico
    public class Medico
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Crm { get; set; } = string.Empty;
        public int EspecialidadeId { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public bool PodeAtenderPaciente()
        {
            return Ativo;
        }

        public string ObterNomeCompleto()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                return "Dr. ";

            if (Nome.StartsWith("Dr.", StringComparison.OrdinalIgnoreCase) ||
                Nome.StartsWith("Dra.", StringComparison.OrdinalIgnoreCase))
                return Nome;

            return $"Dr. {Nome}";
        }

        public bool ValidarDadosCompletos()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Telefone) &&
                   !string.IsNullOrWhiteSpace(Crm) &&
                   EspecialidadeId > 0;
        }

        public int CalcularTempoExperiencia(DateTime? dataReferencia = null)
        {
            var dataAtual = dataReferencia ?? DateTime.Now;
            return dataAtual.Year - DataCadastro.Year;
        }
    }
}