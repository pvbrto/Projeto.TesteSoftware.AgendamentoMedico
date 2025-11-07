using AgendamentoService.Application.Services;
using AgendamentoService.Application.Services.Emails;
using AgendamentoService.Domain.Constants.RestServices;
using AgendamentoService.Domain.Interfaces.Data;
using AgendamentoService.Domain.Interfaces.Repositories;
using AgendamentoService.Domain.Interfaces.RestServices;
using AgendamentoService.Domain.Interfaces.Services;
using AgendamentoService.Domain.Interfaces.Services.Emails;
using AgendamentoService.Infraestructure.Data;
using AgendamentoService.Infraestructure.Repositories;
using AgendamentoService.Infraestructure.RestServices;
using Microsoft.Extensions.DependencyInjection;

namespace AgendamentoService.Infraestructure.CrossCutting
{
    public static class DependencyInjection
    {
        public static void Inject(IServiceCollection services)
        {
            // Data
            services.AddSingleton<ISQliteContext, SQliteContext>();

            services.AddSingleton<IConsultaService, ConsultaService>();
            services.AddSingleton<IEmailService, EmailService>();

            services.AddSingleton<IConsultaRepository, ConsultaRepository>();

            services.AddSingleton<ICadastroRestService>(_ => new CadastroRestService(RestServiceConstants.CADASTRO_SERVICE_URL));
        }
    }
}
