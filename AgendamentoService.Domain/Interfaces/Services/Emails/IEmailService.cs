namespace AgendamentoService.Domain.Interfaces.Services.Emails
{
    public interface IEmailService
    {
        Task EnviarEmail(string destinatario, string assunto, string mensagem);
    }
}
