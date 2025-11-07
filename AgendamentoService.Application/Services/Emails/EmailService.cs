using AgendamentoService.Domain.Interfaces.Services.Emails;

namespace AgendamentoService.Application.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly string _emailFolderPath;

        public EmailService()
        {
            _emailFolderPath = Path.Combine("D:\\Erebor\\FEI\\Teste de Software\\Projeto.TesteSoftware.AgendamentoMedico\\AgendamentoService.Infraestructure\\Data\\Emails");

            if (!Directory.Exists(_emailFolderPath))
                Directory.CreateDirectory(_emailFolderPath);
        }

        public async Task EnviarEmail(string destinatario, string assunto, string mensagem)
        {
            try
            {
                string fileName = $"{DateTime.Now:yyyyMMdd_HHmmssfff}_{destinatario}.txt";
                string filePath = Path.Combine(_emailFolderPath, fileName);

                string conteudo = $@"
                Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                Destinatário: {destinatario}
                Assunto: {assunto}

                --- MENSAGEM ---
                {mensagem}
                ";

                await File.WriteAllTextAsync(filePath, conteudo);

                Console.WriteLine($"[EmailService] E-mail simulado criado em: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Erro ao criar e-mail simulado: {ex.Message}");
                throw;
            }
        }
    }
}
