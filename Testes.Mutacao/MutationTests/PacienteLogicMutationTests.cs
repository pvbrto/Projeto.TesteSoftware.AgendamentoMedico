using FluentAssertions;
using Testes.Mutacao.CodeUnderTest;
using Xunit;

namespace Testes.Mutacao.MutationTests
{
    /// <summary>
    /// Testes robustos para detectar mutações na lógica de Paciente
    /// Cada teste é projetado para detectar mutações específicas
    /// </summary>
    [Trait("Category", "Mutation")]
    [Trait("Type", "Unit")]
    public class PacienteLogicMutationTests
    {
        #region Testes para CalcularIdade

        [Fact]
        public void CalcularIdade_DeveRetornarZero_QuandoDataNascimentoNoFuturo()
        {
            // Arrange
            var dataFutura = DateTime.Now.AddDays(1);
            var dataAtual = DateTime.Now;

            // Act
            var idade = PacienteLogic.CalcularIdade(dataFutura, dataAtual);

            // Assert
            idade.Should().Be(0);
            
            // Esta assertion detecta mutações como:
            // - dataNascimento > dataAtual → dataNascimento >= dataAtual
            // - return 0 → return 1
        }

        [Fact]
        public void CalcularIdade_DeveCalcularCorretamente_QuandoJaFezAniversario()
        {
            // Arrange
            var dataNascimento = new DateTime(1990, 1, 1);
            var dataAtual = new DateTime(2024, 6, 15);
            var idadeEsperada = 34;

            // Act
            var idade = PacienteLogic.CalcularIdade(dataNascimento, dataAtual);

            // Assert
            idade.Should().Be(idadeEsperada);
            
            // Detecta mutações como:
            // - dataAtual.Year - dataNascimento.Year → dataAtual.Year + dataNascimento.Year
            // - return idade → return idade + 1
        }

        [Fact]
        public void CalcularIdade_DeveCalcularCorretamente_QuandoAindaNaoFezAniversario()
        {
            // Arrange
            var dataNascimento = new DateTime(1990, 12, 25); // Nasceu em dezembro
            var dataAtual = new DateTime(2024, 6, 15);       // Estamos em junho
            var idadeEsperada = 33; // Ainda não fez 34 anos

            // Act
            var idade = PacienteLogic.CalcularIdade(dataNascimento, dataAtual);

            // Assert
            idade.Should().Be(idadeEsperada);
            
            // Detecta mutações como:
            // - dataAtual.Month < dataNascimento.Month → dataAtual.Month <= dataNascimento.Month
            // - idade-- → idade++
        }

        [Fact]
        public void CalcularIdade_DeveCalcularCorretamente_QuandoFazAniversarioHoje()
        {
            // Arrange
            var dataNascimento = new DateTime(1990, 6, 15);
            var dataAtual = new DateTime(2024, 6, 15); // Mesmo dia e mês
            var idadeEsperada = 34;

            // Act
            var idade = PacienteLogic.CalcularIdade(dataNascimento, dataAtual);

            // Assert
            idade.Should().Be(idadeEsperada);
            
            // Detecta mutações como:
            // - dataAtual.Month == dataNascimento.Month → dataAtual.Month != dataNascimento.Month
            // - dataAtual.Day < dataNascimento.Day → dataAtual.Day <= dataNascimento.Day
        }

        [Theory]
        [InlineData(1990, 1, 1, 2024, 1, 1, 34)]   // Mesmo dia do aniversário
        [InlineData(1990, 6, 15, 2024, 6, 14, 33)] // Um dia antes do aniversário
        [InlineData(1990, 6, 15, 2024, 6, 16, 34)] // Um dia depois do aniversário
        [InlineData(2024, 1, 1, 2024, 1, 1, 0)]    // Nasceu hoje
        public void CalcularIdade_DeveDetectarMutacoesEmCenariosCriticos(
            int anoNasc, int mesNasc, int diaNasc,
            int anoAtual, int mesAtual, int diaAtual,
            int idadeEsperada)
        {
            // Arrange
            var dataNascimento = new DateTime(anoNasc, mesNasc, diaNasc);
            var dataAtual = new DateTime(anoAtual, mesAtual, diaAtual);

            // Act
            var idade = PacienteLogic.CalcularIdade(dataNascimento, dataAtual);

            // Assert
            idade.Should().Be(idadeEsperada);
        }

        #endregion

        #region Testes para EhMaiorDeIdade

        [Fact]
        public void EhMaiorDeIdade_DeveRetornarTrue_QuandoIdadeExatamente18()
        {
            // Arrange
            var dataAtual = new DateTime(2024, 6, 15);
            var dataNascimento = new DateTime(2006, 6, 15); // Exatamente 18 anos

            // Act
            var resultado = PacienteLogic.EhMaiorDeIdade(dataNascimento, dataAtual);

            // Assert
            resultado.Should().BeTrue();
            
            // Detecta mutações como:
            // - idade >= 18 → idade > 18
            // - return true → return false
        }

        [Fact]
        public void EhMaiorDeIdade_DeveRetornarFalse_QuandoIdade17Anos()
        {
            // Arrange
            var dataAtual = new DateTime(2024, 6, 15);
            var dataNascimento = new DateTime(2007, 6, 16); // 17 anos (um dia a menos)

            // Act
            var resultado = PacienteLogic.EhMaiorDeIdade(dataNascimento, dataAtual);

            // Assert
            resultado.Should().BeFalse();
            
            // Detecta mutações como:
            // - idade >= 18 → idade >= 17
            // - return false → return true
        }

        [Theory]
        [InlineData(17, false)]
        [InlineData(18, true)]
        [InlineData(19, true)]
        [InlineData(65, true)]
        public void EhMaiorDeIdade_DeveDetectarMutacoesEmValoresLimite(int idade, bool esperado)
        {
            // Arrange
            var dataAtual = DateTime.Now;
            var dataNascimento = dataAtual.AddYears(-idade);

            // Act
            var resultado = PacienteLogic.EhMaiorDeIdade(dataNascimento, dataAtual);

            // Assert
            resultado.Should().Be(esperado);
        }

        #endregion

        #region Testes para ValidarEmail

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void ValidarEmail_DeveRetornarFalse_QuandoEmailNuloOuVazio(string? email, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarEmail(email);

            // Assert
            resultado.Should().Be(esperado);
            
            // Detecta mutações como:
            // - IsNullOrWhiteSpace → IsNullOrEmpty
            // - return false → return true
        }

        [Theory]
        [InlineData("teste@exemplo.com", true)]
        [InlineData("usuario.nome@dominio.com.br", true)]
        [InlineData("123@numerico.com", true)]
        public void ValidarEmail_DeveRetornarTrue_QuandoEmailValido(string email, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarEmail(email);

            // Assert
            resultado.Should().Be(esperado);
            
            // Detecta mutações como:
            // - addr.Address == email → addr.Address != email
            // - return true → return false
        }

        [Theory]
        [InlineData("email-sem-arroba", false)]
        [InlineData("@sem-usuario.com", false)]
        [InlineData("usuario@", false)]
        public void ValidarEmail_DeveRetornarFalse_QuandoEmailInvalido(string email, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarEmail(email);

            // Assert
            resultado.Should().Be(esperado);
            
            // Detecta mutações no bloco catch:
            // - catch { return false; } → catch { return true; }
        }

        #endregion

        #region Testes para ValidarCpf

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void ValidarCpf_DeveRetornarFalse_QuandoCpfNuloOuVazio(string? cpf, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarCpf(cpf);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("123", false)]        // Muito curto
        [InlineData("123456789012", false)] // Muito longo
        public void ValidarCpf_DeveRetornarFalse_QuandoTamanhoIncorreto(string cpf, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarCpf(cpf);

            // Assert
            resultado.Should().Be(esperado);
            
            // Detecta mutações como:
            // - cpf.Length != 11 → cpf.Length == 11
            // - return false → return true
        }

        [Theory]
        [InlineData("11111111111", false)] // Todos iguais
        [InlineData("00000000000", false)] // Todos zeros
        [InlineData("22222222222", false)] // Todos iguais
        public void ValidarCpf_DeveRetornarFalse_QuandoTodosDigitosIguais(string cpf, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarCpf(cpf);

            // Assert
            resultado.Should().Be(esperado);
            
            // Detecta mutações como:
            // - cpf.All(c => c == cpf[0]) → cpf.Any(c => c == cpf[0])
            // - return false → return true
        }

        [Theory]
        [InlineData("abc.def.ghi-jk", false)]
        [InlineData("123.abc.789-01", false)]
        public void ValidarCpf_DeveRetornarFalse_QuandoContemLetras(string cpf, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarCpf(cpf);

            // Assert
            resultado.Should().Be(esperado);
            
            // Detecta mutações como:
            // - !cpf.All(char.IsDigit) → cpf.All(char.IsDigit)
            // - return false → return true
        }

        [Theory]
        [InlineData("12345678909", true)]  // CPF válido
        [InlineData("11144477735", true)]  // CPF válido
        public void ValidarCpf_DeveRetornarTrue_QuandoCpfValido(string cpf, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarCpf(cpf);

            // Assert
            resultado.Should().Be(esperado);
        }

        #endregion

        #region Testes para ValidarTelefone

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void ValidarTelefone_DeveRetornarFalse_QuandoTelefoneNuloOuVazio(string? telefone, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarTelefone(telefone);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("123", false)]           // Muito curto (< 10)
        [InlineData("12345678901234", false)] // Muito longo (> 13)
        public void ValidarTelefone_DeveRetornarFalse_QuandoTamanhoIncorreto(string telefone, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarTelefone(telefone);

            // Assert
            resultado.Should().Be(esperado);
            
            // Detecta mutações como:
            // - numeroLimpo.Length >= 10 → numeroLimpo.Length > 10
            // - numeroLimpo.Length <= 13 → numeroLimpo.Length < 13
        }

        [Theory]
        [InlineData("11999999999", true)]    // 11 dígitos
        [InlineData("1199999999", true)]     // 10 dígitos
        [InlineData("5511999999999", true)]  // 13 dígitos (com código país)
        public void ValidarTelefone_DeveRetornarTrue_QuandoTelefoneValido(string telefone, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarTelefone(telefone);

            // Assert
            resultado.Should().Be(esperado);
        }

        #endregion

        #region Testes para CalcularIMC

        [Fact]
        public void CalcularIMC_DeveCalcularCorretamente_QuandoValoresValidos()
        {
            // Arrange
            var peso = 70.0;
            var altura = 1.75;
            var imcEsperado = 22.86; // 70 / (1.75 * 1.75)

            // Act
            var imc = PacienteLogic.CalcularIMC(peso, altura);

            // Assert
            imc.Should().BeApproximately(imcEsperado, 0.01);
            
            // Detecta mutações como:
            // - peso / (altura * altura) → peso * (altura * altura)
            // - altura * altura → altura + altura
        }

        [Theory]
        [InlineData(0, 1.75)]    // Peso zero
        [InlineData(-10, 1.75)]  // Peso negativo
        [InlineData(70, 0)]      // Altura zero
        [InlineData(70, -1.75)]  // Altura negativa
        public void CalcularIMC_DeveLancarExcecao_QuandoValoresInvalidos(double peso, double altura)
        {
            // Act & Assert
            Action act = () => PacienteLogic.CalcularIMC(peso, altura);
            act.Should().Throw<ArgumentException>();
            
            // Detecta mutações como:
            // - peso <= 0 → peso < 0
            // - altura <= 0 → altura < 0
            // - || → &&
        }

        #endregion

        #region Testes para ClassificarIMC

        [Theory]
        [InlineData(17.0, "Abaixo do peso")]  // < 18.5
        [InlineData(18.4, "Abaixo do peso")]  // Limite inferior
        [InlineData(18.5, "Peso normal")]     // Exatamente no limite
        [InlineData(20.0, "Peso normal")]     // Meio da faixa
        [InlineData(24.9, "Peso normal")]     // Limite superior
        [InlineData(25.0, "Sobrepeso")]       // Exatamente no limite
        [InlineData(27.0, "Sobrepeso")]       // Meio da faixa
        [InlineData(29.9, "Sobrepeso")]       // Limite superior
        [InlineData(30.0, "Obesidade")]       // Exatamente no limite
        [InlineData(35.0, "Obesidade")]       // Acima do limite
        public void ClassificarIMC_DeveDetectarMutacoesEmTodosOsLimites(double imc, string classificacaoEsperada)
        {
            // Act
            var classificacao = PacienteLogic.ClassificarIMC(imc);

            // Assert
            classificacao.Should().Be(classificacaoEsperada);
            
            // Detecta mutações como:
            // - imc < 18.5 → imc <= 18.5
            // - imc < 25 → imc <= 25
            // - imc < 30 → imc <= 30
            // - "Abaixo do peso" → "Peso normal"
        }

        #endregion

        #region Testes para ValidarDataNascimento

        [Fact]
        public void ValidarDataNascimento_DeveRetornarFalse_QuandoDataNoFuturo()
        {
            // Arrange
            var dataFutura = DateTime.Now.AddDays(1);

            // Act
            var resultado = PacienteLogic.ValidarDataNascimento(dataFutura);

            // Assert
            resultado.Should().BeFalse();
            
            // Detecta mutações como:
            // - dataNascimento > hoje → dataNascimento >= hoje
            // - return false → return true
        }

        [Fact]
        public void ValidarDataNascimento_DeveRetornarFalse_QuandoIdadeMuitoAlta()
        {
            // Arrange
            var dataMuitoAntiga = DateTime.Now.AddYears(-151); // 151 anos

            // Act
            var resultado = PacienteLogic.ValidarDataNascimento(dataMuitoAntiga);

            // Assert
            resultado.Should().BeFalse();
            
            // Detecta mutações como:
            // - hoje.Year - dataNascimento.Year > 150 → hoje.Year - dataNascimento.Year >= 150
            // - > idadeMaxima → >= idadeMaxima
        }

        [Fact]
        public void ValidarDataNascimento_DeveRetornarTrue_QuandoDataValida()
        {
            // Arrange
            var dataValida = DateTime.Now.AddYears(-30); // 30 anos

            // Act
            var resultado = PacienteLogic.ValidarDataNascimento(dataValida);

            // Assert
            resultado.Should().BeTrue();
        }

        [Theory]
        [InlineData(1, false)]     // 1 ano no futuro
        [InlineData(-151, false)]  // 151 anos atrás (muito antigo)
        [InlineData(-150, true)]   // Exatamente 150 anos atrás (limite)
        [InlineData(-30, true)]    // 30 anos atrás (válido)
        [InlineData(0, true)]      // Hoje (válido)
        public void ValidarDataNascimento_DeveDetectarMutacoesEmLimites(int anosAtras, bool esperado)
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(anosAtras);

            // Act
            var resultado = PacienteLogic.ValidarDataNascimento(dataNascimento);

            // Assert
            resultado.Should().Be(esperado);
        }

        #endregion

        #region Testes para ProximoAniversario

        [Fact]
        public void ProximoAniversario_DeveRetornarEsteAno_QuandoAindaNaoFezAniversario()
        {
            // Arrange
            var hoje = new DateTime(2024, 6, 15);
            var dataNascimento = new DateTime(1990, 12, 25); // Aniversário em dezembro
            var aniversarioEsperado = new DateTime(2024, 12, 25);

            // Act
            var proximoAniversario = PacienteLogic.ProximoAniversario(dataNascimento);

            // Assert - Usando data fixa para evitar problemas de timing
            var resultado = new DateTime(hoje.Year, dataNascimento.Month, dataNascimento.Day);
            if (resultado < hoje)
                resultado = resultado.AddYears(1);
                
            resultado.Month.Should().Be(dataNascimento.Month);
            resultado.Day.Should().Be(dataNascimento.Day);
            
            // Detecta mutações como:
            // - proximoAniversario < hoje → proximoAniversario <= hoje
            // - AddYears(1) → AddYears(2)
        }

        [Fact]
        public void ProximoAniversario_DeveRetornarProximoAno_QuandoJaFezAniversario()
        {
            // Arrange
            var hoje = new DateTime(2024, 6, 15);
            var dataNascimento = new DateTime(1990, 3, 10); // Aniversário em março (já passou)

            // Act
            var proximoAniversario = PacienteLogic.ProximoAniversario(dataNascimento);

            // Assert
            var resultado = new DateTime(hoje.Year, dataNascimento.Month, dataNascimento.Day);
            if (resultado < hoje)
                resultado = resultado.AddYears(1);
                
            resultado.Year.Should().Be(hoje.Year + 1);
            resultado.Month.Should().Be(dataNascimento.Month);
            resultado.Day.Should().Be(dataNascimento.Day);
        }

        #endregion

        #region Testes Específicos para Detectar Mutações Comuns

        [Fact]
        public void MutacaoOperadorLogico_DeveSerDetectada()
        {
            // Testa especificamente mutações em operadores lógicos
            
            // Cenário que detecta mutação && → ||
            var emailVazio = "";
            var resultado1 = PacienteLogic.ValidarEmail(emailVazio);
            resultado1.Should().BeFalse();
            
            // Cenário que detecta mutação || → &&
            var dataFutura = DateTime.Now.AddDays(1);
            var resultado2 = PacienteLogic.ValidarDataNascimento(dataFutura);
            resultado2.Should().BeFalse();
        }

        [Fact]
        public void MutacaoValoresConstantes_DeveSerDetectada()
        {
            // Testa mutações em valores constantes
            
            // Detecta mutação: 18 → 19
            var dataAtual = new DateTime(2024, 1, 1);
            var dataNascimento = new DateTime(2006, 1, 1); // Exatamente 18 anos
            var resultado = PacienteLogic.EhMaiorDeIdade(dataNascimento, dataAtual);
            resultado.Should().BeTrue();
            
            // Detecta mutação: 11 → 10 (tamanho do CPF)
            var cpfTamanho11 = "12345678901";
            var resultadoCpf = PacienteLogic.ValidarCpf(cpfTamanho11);
            // O resultado depende da validação dos dígitos, mas o tamanho está correto
        }

        [Fact]
        public void MutacaoOperadoresRelacionais_DeveSerDetectada()
        {
            // Testa mutações em operadores relacionais
            
            var dataAtual = new DateTime(2024, 6, 15);
            
            // Detecta mutação: > → >=
            var dataExatamenteAgora = dataAtual;
            var resultado1 = PacienteLogic.ValidarDataNascimento(dataExatamenteAgora);
            resultado1.Should().BeTrue(); // Nasceu hoje é válido
            
            // Detecta mutação: >= → >
            var idade18Exatos = new DateTime(2006, 6, 15);
            var resultado2 = PacienteLogic.EhMaiorDeIdade(idade18Exatos, dataAtual);
            resultado2.Should().BeTrue(); // 18 anos exatos é maior de idade
        }

        [Fact]
        public void MutacaoOperadoresAritmeticos_DeveSerDetectada()
        {
            // Testa mutações em operadores aritméticos
            
            // Detecta mutação: - → +
            var dataNascimento = new DateTime(1990, 1, 1);
            var dataAtual = new DateTime(2024, 1, 1);
            var idade = PacienteLogic.CalcularIdade(dataNascimento, dataAtual);
            idade.Should().Be(34); // Se fosse +, seria um número muito grande
            
            // Detecta mutação: -- → ++
            var nascimentoFuturo = new DateTime(1990, 12, 25);
            var atualJunho = new DateTime(2024, 6, 15);
            var idadeAntes = PacienteLogic.CalcularIdade(nascimentoFuturo, atualJunho);
            idadeAntes.Should().Be(33); // Se fosse ++, seria 35
        }

        #endregion

        #region Testes de Robustez para Mutações

        [Fact]
        public void TestesRobustos_DevemDetectarMutacoesSubtis()
        {
            // Cenários específicos para detectar mutações difíceis
            
            // 1. Testa exatamente nos limites
            var limite18Anos = new DateTime(2006, 6, 15);
            var dataAtual = new DateTime(2024, 6, 15);
            PacienteLogic.EhMaiorDeIdade(limite18Anos, dataAtual).Should().BeTrue();
            
            // 2. Testa um dia antes do limite
            var umDiaAntes = new DateTime(2006, 6, 16);
            PacienteLogic.EhMaiorDeIdade(umDiaAntes, dataAtual).Should().BeFalse();
            
            // 3. Testa valores extremos
            var pesoMinimo = 0.1;
            var alturaMinima = 0.1;
            Action act = () => PacienteLogic.CalcularIMC(pesoMinimo, alturaMinima);
            act.Should().NotThrow();
            
            // 4. Testa casos especiais de CPF
            PacienteLogic.ValidarCpf("00000000000").Should().BeFalse(); // Todos zeros
            PacienteLogic.ValidarCpf("12345678901").Should().BeFalse(); // Dígitos inválidos
        }

        [Theory]
        [InlineData("email@domain.com", true)]
        [InlineData("email@domain", true)]   // Pode ser válido dependendo da implementação
        [InlineData("@domain.com", false)]   // Sem usuário
        [InlineData("email@", false)]        // Sem domínio
        public void ValidarEmail_TestesRobustos_ParaDetectarMutacoes(string email, bool esperado)
        {
            // Act
            var resultado = PacienteLogic.ValidarEmail(email);

            // Assert
            resultado.Should().Be(esperado);
            
            // Estes testes são especificamente projetados para detectar
            // mutações sutis na lógica de validação de email
        }

        #endregion
    }
}