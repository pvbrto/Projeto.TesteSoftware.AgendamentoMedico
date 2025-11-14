using System.Net.Mail;

namespace Testes.Mutacao.CodeUnderTest
{
    public static class PacienteLogic
    {
        public static int CalcularIdade(DateTime dataNascimento, DateTime? dataReferencia = null)
        {
            var dataAtual = dataReferencia ?? DateTime.Now;
            
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

        public static bool EhMaiorDeIdade(DateTime dataNascimento, DateTime? dataReferencia = null)
        {
            var idade = CalcularIdade(dataNascimento, dataReferencia);
            return idade >= 18;
        }

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

        public static bool ValidarCpf(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            if (!cpf.All(char.IsDigit))
                return false;

            return ValidarDigitosVerificadores(cpf);
        }

        public static bool ValidarTelefone(string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            var numeroLimpo = new string(telefone.Where(char.IsDigit).ToArray());

            return numeroLimpo.Length >= 10 && numeroLimpo.Length <= 13;
        }

        public static double CalcularIMC(double peso, double altura)
        {
            if (peso <= 0 || altura <= 0)
                throw new ArgumentException("Peso e altura devem ser maiores que zero");

            return peso / (altura * altura);
        }

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

        public static bool ValidarDataNascimento(DateTime dataNascimento)
        {
            var hoje = DateTime.Now;
            var idadeMaxima = 150;
            
            if (dataNascimento > hoje)
                return false;
                
            if (hoje.Year - dataNascimento.Year > idadeMaxima)
                return false;
                
            return true;
        }

        public static DateTime ProximoAniversario(DateTime dataNascimento)
        {
            var hoje = DateTime.Now;
            var proximoAniversario = new DateTime(hoje.Year, dataNascimento.Month, dataNascimento.Day);
            
            if (proximoAniversario < hoje)
            {
                proximoAniversario = proximoAniversario.AddYears(1);
            }
            
            return proximoAniversario;
        }

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