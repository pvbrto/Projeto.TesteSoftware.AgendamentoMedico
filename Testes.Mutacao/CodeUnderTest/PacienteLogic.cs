using System.Net.Mail;

namespace Testes.Mutacao.CodeUnderTest
{
    /// <summary>
    /// Classe que contém a lógica de negócio para Paciente
    /// Esta é a implementação real que será testada com mutações
    /// </summary>
    public static class PacienteLogic
    {
        /// <summary>
        /// Calcula a idade baseada na data de nascimento
        /// </summary>
        public static int CalcularIdade(DateTime dataNascimento, DateTime? dataReferencia = null)
        {
            var dataAtual = dataReferencia ?? DateTime.Now;
            
            // Se a data de nascimento é no futuro, retorna 0
            if (dataNascimento > dataAtual)
                return 0;
                
            var idade = dataAtual.Year - dataNascimento.Year;
            
            // Ajusta se ainda não fez aniversário este ano
            if (dataAtual.Month < dataNascimento.Month || 
                (dataAtual.Month == dataNascimento.Month && dataAtual.Day < dataNascimento.Day))
            {
                idade--;
            }
            
            return Math.Max(0, idade);
        }

        /// <summary>
        /// Valida se o paciente é maior de idade
        /// </summary>
        public static bool EhMaiorDeIdade(DateTime dataNascimento, DateTime? dataReferencia = null)
        {
            var idade = CalcularIdade(dataNascimento, dataReferencia);
            return idade >= 18;
        }

        /// <summary>
        /// Valida formato de email
        /// </summary>
        public static bool ValidarEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida formato de CPF
        /// </summary>
        public static bool ValidarCpf(string? cpf)
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

            // Verifica se todos são dígitos
            if (!cpf.All(char.IsDigit))
                return false;

            // Validação dos dígitos verificadores
            return ValidarDigitosVerificadores(cpf);
        }

        /// <summary>
        /// Valida formato de telefone
        /// </summary>
        public static bool ValidarTelefone(string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            // Remove formatação
            var numeroLimpo = new string(telefone.Where(char.IsDigit).ToArray());

            // Aceita números com 10 ou 11 dígitos (com ou sem 9 no celular)
            // Pode ter código do país (+55)
            return numeroLimpo.Length >= 10 && numeroLimpo.Length <= 13;
        }

        /// <summary>
        /// Calcula o IMC (Índice de Massa Corporal)
        /// </summary>
        public static double CalcularIMC(double peso, double altura)
        {
            if (peso <= 0 || altura <= 0)
                throw new ArgumentException("Peso e altura devem ser maiores que zero");

            return peso / (altura * altura);
        }

        /// <summary>
        /// Classifica o IMC
        /// </summary>
        public static string ClassificarIMC(double imc)
        {
            if (imc < 18.5)
                return "Abaixo do peso";
            else if (imc < 25)
                return "Peso normal";
            else if (imc < 30)
                return "Sobrepeso";
            else
                return "Obesidade";
        }

        /// <summary>
        /// Valida se a data de nascimento é válida
        /// </summary>
        public static bool ValidarDataNascimento(DateTime dataNascimento)
        {
            var hoje = DateTime.Now;
            var idadeMaxima = 150;
            
            // Não pode ser no futuro
            if (dataNascimento > hoje)
                return false;
                
            // Não pode ser muito antiga (mais de 150 anos)
            if (hoje.Year - dataNascimento.Year > idadeMaxima)
                return false;
                
            return true;
        }

        /// <summary>
        /// Calcula o próximo aniversário
        /// </summary>
        public static DateTime ProximoAniversario(DateTime dataNascimento)
        {
            var hoje = DateTime.Now;
            var proximoAniversario = new DateTime(hoje.Year, dataNascimento.Month, dataNascimento.Day);
            
            // Se já passou este ano, é no próximo ano
            if (proximoAniversario < hoje)
            {
                proximoAniversario = proximoAniversario.AddYears(1);
            }
            
            return proximoAniversario;
        }

        /// <summary>
        /// Valida dígitos verificadores do CPF
        /// </summary>
        private static bool ValidarDigitosVerificadores(string cpf)
        {
            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (int i = 0; i < 9; i++)
            {
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
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}