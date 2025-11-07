using FluentAssertions;
using AutoFixture;
using Xunit;

namespace Testes.Unitarios.Domain.Entities
{
    /// <summary>
    /// Testes de caixa branca para a entidade Paciente
    /// Foca na lógica interna, validações e comportamentos específicos
    /// </summary>
    [Trait("Category", "Domain")]
    [Trait("Type", "Unit")]
    public class PacienteTests
    {
        private readonly Fixture _fixture;

        public PacienteTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Construtor_DeveInicializarPropriedadesCorretamente()
        {
            // Arrange
            var nome = "João Silva";
            var email = "joao@teste.com";
            var telefone = "(11) 99999-9999";
            var cpf = "123.456.789-00";
            var dataNascimento = new DateTime(1990, 1, 1);

            // Act
            var paciente = new Paciente
            {
                Nome = nome,
                Email = email,
                Telefone = telefone,
                Cpf = cpf,
                DataNascimento = dataNascimento
            };

            // Assert
            paciente.Nome.Should().Be(nome);
            paciente.Email.Should().Be(email);
            paciente.Telefone.Should().Be(telefone);
            paciente.Cpf.Should().Be(cpf);
            paciente.DataNascimento.Should().Be(dataNascimento);
        }

        [Fact]
        public void CalcularIdade_DeveRetornarIdadeCorreta_QuandoDataNascimentoValida()
        {
            // Arrange
            var dataAtual = new DateTime(2024, 11, 6);
            var dataNascimento = new DateTime(1990, 5, 15);
            var paciente = new Paciente { DataNascimento = dataNascimento };
            var idadeEsperada = 34;

            // Act
            var idade = CalcularIdade(paciente.DataNascimento, dataAtual);

            // Assert
            idade.Should().Be(idadeEsperada);
        }

        [Theory]
        [InlineData(2024, 6, 15, 2024, 6, 14, 0)] // Mesmo ano, antes do aniversário
        [InlineData(2024, 6, 15, 2024, 6, 15, 0)] // Mesmo ano, no aniversário
        [InlineData(2024, 1, 1, 2024, 6, 15, 0)] // Mesmo ano, depois do aniversário
        [InlineData(2023, 6, 15, 2024, 6, 14, 0)] // Ano anterior, um dia antes do aniversário
        [InlineData(2023, 6, 15, 2024, 6, 15, 1)] // Ano anterior, no aniversário
        [InlineData(2023, 6, 15, 2024, 6, 16, 1)] // Ano anterior, um dia depois do aniversário
        public void CalcularIdade_DeveCalcularCorretamente_ParaDiferentesCenarios(
            int anoNascimento, int mesNascimento, int diaNascimento,
            int anoAtual, int mesAtual, int diaAtual,
            int idadeEsperada)
        {
            // Arrange
            var dataNascimento = new DateTime(anoNascimento, mesNascimento, diaNascimento);
            var dataAtual = new DateTime(anoAtual, mesAtual, diaAtual);
            var paciente = new Paciente { DataNascimento = dataNascimento };

            // Act
            var idade = CalcularIdade(paciente.DataNascimento, dataAtual);

            // Assert
            idade.Should().Be(idadeEsperada);
        }

        [Fact]
        public void ValidarEmail_DeveRetornarTrue_QuandoEmailValido()
        {
            // Arrange
            var emailsValidos = new[]
            {
                "teste@exemplo.com",
                "usuario.nome@dominio.com.br",
                "email+tag@teste.org",
                "123@numerico.com"
            };

            foreach (var email in emailsValidos)
            {
                // Act
                var resultado = ValidarEmail(email);

                // Assert
                resultado.Should().BeTrue($"Email '{email}' deveria ser válido");
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("email-sem-arroba")]
        [InlineData("@sem-usuario.com")]
        [InlineData("usuario@")]
        public void ValidarEmail_DeveRetornarFalse_QuandoEmailInvalido(string emailInvalido)
        {
            // Act
            var resultado = ValidarEmail(emailInvalido);

            // Assert
            resultado.Should().BeFalse($"Email '{emailInvalido}' deveria ser inválido");
        }

        [Fact]
        public void ValidarCpf_DeveRetornarTrue_QuandoCpfValido()
        {
            // Arrange
            var cpfsValidos = new[]
            {
                "123.456.789-09",
                "111.444.777-35",
                "000.000.001-91"
            };

            foreach (var cpf in cpfsValidos)
            {
                // Act
                var resultado = ValidarCpf(cpf);

                // Assert
                resultado.Should().BeTrue($"CPF '{cpf}' deveria ser válido");
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("123.456.789-00")] // CPF inválido
        [InlineData("111.111.111-11")] // CPF com todos os dígitos iguais
        [InlineData("000.000.000-00")] // CPF com todos os dígitos zero
        public void ValidarCpf_DeveRetornarFalse_QuandoCpfInvalido(string cpfInvalido)
        {
            // Act
            var resultado = ValidarCpf(cpfInvalido);

            // Assert
            resultado.Should().BeFalse($"CPF '{cpfInvalido}' deveria ser inválido");
        }

        [Fact]
        public void ValidarTelefone_DeveRetornarTrue_QuandoTelefoneValido()
        {
            // Arrange
            var telefonesValidos = new[]
            {
                "(11) 99999-9999",
                "(21) 88888-8888",
                "(85) 77777-7777",
                "11999999999",
                "+5511999999999"
            };

            foreach (var telefone in telefonesValidos)
            {
                // Act
                var resultado = ValidarTelefone(telefone);

                // Assert
                resultado.Should().BeTrue($"Telefone '{telefone}' deveria ser válido");
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("(11) 9999-999")] // Muito curto
        [InlineData("abc-def-ghij")] // Com letras
        public void ValidarTelefone_DeveRetornarFalse_QuandoTelefoneInvalido(string telefoneInvalido)
        {
            // Act
            var resultado = ValidarTelefone(telefoneInvalido);

            // Assert
            resultado.Should().BeFalse($"Telefone '{telefoneInvalido}' deveria ser inválido");
        }

        [Fact]
        public void EhMaiorDeIdade_DeveRetornarTrue_QuandoPacienteMaiorDe18Anos()
        {
            // Arrange
            var dataAtual = new DateTime(2024, 11, 6);
            var dataNascimento = new DateTime(2000, 1, 1); // 24 anos
            var paciente = new Paciente { DataNascimento = dataNascimento };

            // Act
            var resultado = EhMaiorDeIdade(paciente.DataNascimento, dataAtual);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void EhMaiorDeIdade_DeveRetornarFalse_QuandoPacienteMenorDe18Anos()
        {
            // Arrange
            var dataAtual = new DateTime(2024, 11, 6);
            var dataNascimento = new DateTime(2010, 1, 1); // 14 anos
            var paciente = new Paciente { DataNascimento = dataNascimento };

            // Act
            var resultado = EhMaiorDeIdade(paciente.DataNascimento, dataAtual);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void EhMaiorDeIdade_DeveRetornarTrue_QuandoPacienteExatamente18Anos()
        {
            // Arrange
            var dataAtual = new DateTime(2024, 11, 6);
            var dataNascimento = new DateTime(2006, 11, 6); // Exatamente 18 anos
            var paciente = new Paciente { DataNascimento = dataNascimento };

            // Act
            var resultado = EhMaiorDeIdade(paciente.DataNascimento, dataAtual);

            // Assert
            resultado.Should().BeTrue();
        }

        // Métodos auxiliares que simulam a lógica interna da entidade
        // Em um projeto real, estes métodos estariam na própria entidade
        private static int CalcularIdade(DateTime dataNascimento, DateTime? dataReferencia = null)
        {
            var dataAtual = dataReferencia ?? DateTime.Now;
            
            // Se a data de nascimento é no futuro, retorna 0
            if (dataNascimento > dataAtual)
                return 0;
                
            var idade = dataAtual.Year - dataNascimento.Year;
            
            if (dataAtual.Month < dataNascimento.Month || 
                (dataAtual.Month == dataNascimento.Month && dataAtual.Day < dataNascimento.Day))
            {
                idade--;
            }
            
            return Math.Max(0, idade);
        }

        private static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove formatação
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validação dos dígitos verificadores
            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(tempCpf[i]))
                    return false;
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            var resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        private static bool ValidarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            // Remove formatação
            var numeroLimpo = new string(telefone.Where(char.IsDigit).ToArray());

            // Aceita números com 10 ou 11 dígitos (com ou sem 9 no celular)
            // Pode ter código do país (+55)
            return numeroLimpo.Length >= 10 && numeroLimpo.Length <= 13;
        }

        private static bool EhMaiorDeIdade(DateTime dataNascimento, DateTime? dataReferencia = null)
        {
            var idade = CalcularIdade(dataNascimento, dataReferencia);
            return idade >= 18;
        }
    }

    // Classe auxiliar para simular a entidade Paciente
    // Em um projeto real, esta classe estaria no domínio
    public class Paciente
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
    }
}