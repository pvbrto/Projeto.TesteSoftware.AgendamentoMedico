using Bogus;

namespace Testes.Funcionais.Fixtures
{
    public static class TestDataGenerator
    {
        private static readonly Faker _faker = new("pt_BR");

        public static class Paciente
        {
            public static object CreateValidPaciente()
            {
                return new
                {
                    Nome = _faker.Person.FullName,
                    Email = _faker.Internet.Email(),
                    Telefone = _faker.Phone.PhoneNumber("(##) #####-####"),
                    DataNascimento = _faker.Date.Past(80, DateTime.Now.AddYears(-18)),
                    Cpf = GenerateCpf(),
                    Endereco = new
                    {
                        Logradouro = _faker.Address.StreetAddress(),
                        Numero = _faker.Random.Number(1, 9999).ToString(),
                        Complemento = _faker.Address.SecondaryAddress(),
                        Bairro = _faker.Address.City(),
                        Cidade = _faker.Address.City(),
                        Estado = _faker.Address.StateAbbr(),
                        Cep = _faker.Address.ZipCode("########")
                    }
                };
            }

            public static object CreateInvalidPaciente()
            {
                return new
                {
                    Nome = "",
                    Email = "email-invalido",
                    Telefone = "123",
                    DataNascimento = DateTime.Now.AddYears(10),
                    Cpf = "123.456.789-00"
                };
            }
        }

        public static class Medico
        {
            public static object CreateValidMedico()
            {
                return new
                {
                    Nome = _faker.Person.FullName,
                    Email = _faker.Internet.Email(),
                    Telefone = _faker.Phone.PhoneNumber("(##) #####-####"),
                    Crm = _faker.Random.Number(10000, 99999).ToString(),
                    EspecialidadeId = 1
                };
            }

            public static object CreateInvalidMedico()
            {
                return new
                {
                    Nome = "",
                    Email = "email-invalido",
                    Telefone = "123",
                    Crm = "",
                    EspecialidadeId = 999999
                };
            }
        }

        public static class Consulta
        {
            public static object CreateValidConsulta()
            {
                return new
                {
                    PacienteId = 1,
                    MedicoId = 1,
                    DataHora = DateTime.Now.AddDays(_faker.Random.Number(1, 30)),
                    Observacoes = _faker.Lorem.Sentence()
                };
            }

            public static object CreateInvalidConsulta()
            {
                return new
                {
                    PacienteId = 999999,
                    MedicoId = 999999,
                    DataHora = DateTime.Now.AddDays(-10),
                    Observacoes = ""
                };
            }

            public static object CreateConsultaFiltro()
            {
                return new
                {
                    DataInicio = DateTime.Now.Date,
                    DataFim = DateTime.Now.Date.AddDays(30),
                    MedicoId = (int?)null,
                    PacienteId = (int?)null,
                    Status = (int?)null
                };
            }
        }

        private static string GenerateCpf()
        {
            var cpf = new int[11];
            var random = new Random();

            for (int i = 0; i < 9; i++)
                cpf[i] = random.Next(0, 10);

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += cpf[i] * (10 - i);
            cpf[9] = (sum * 10) % 11;
            if (cpf[9] >= 10) cpf[9] = 0;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += cpf[i] * (11 - i);
            cpf[10] = (sum * 10) % 11;
            if (cpf[10] >= 10) cpf[10] = 0;

            return $"{cpf[0]}{cpf[1]}{cpf[2]}.{cpf[3]}{cpf[4]}{cpf[5]}.{cpf[6]}{cpf[7]}{cpf[8]}-{cpf[9]}{cpf[10]}";
        }
    }
}