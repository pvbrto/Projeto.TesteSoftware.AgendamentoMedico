using CadastroService.Application.Services;
using CadastroService.Domain.Interfaces.Data;
using CadastroService.Domain.Interfaces.Repositories;
using CadastroService.Domain.Interfaces.Services;
using CadastroService.Infraestructure.Data;
using CadastroService.Infraestructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CadastroService.Infraestructure.CrossCutting
{
    public static class DependencyInjection
    {
        public static void Inject(IServiceCollection services)
        {
            // Data
            services.AddSingleton<ISQliteContext, SQliteContext>();

            // Services
            services.AddSingleton<IClinicaService, ClinicaService>();
            services.AddSingleton<IEspecialidadeService, EspecialidadeService>();
            services.AddSingleton<IPacienteService, PacienteService>();
            services.AddSingleton<IMedicoService, MedicoService>();

            // Repository
            services.AddSingleton<IClinicaRepository, ClinicaRepository>();
            services.AddSingleton<IEspecialidadeRepository, EspecialidadeRepository>();
            services.AddSingleton<IPacienteRepository, PacienteRepository>();
            services.AddSingleton<IMedicoRepository, MedicoRepository>();
        }
    }
}
