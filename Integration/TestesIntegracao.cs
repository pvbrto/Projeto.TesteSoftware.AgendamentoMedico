using AgendamentoService.Application.Services;
using AgendamentoService.Application.Services.Emails;
using AgendamentoService.Domain.Constants.RestServices;
using AgendamentoService.Infraestructure.Repositories;
using AgendamentoService.Infraestructure.RestServices;
using CadastroService.Application.Services;
using CadastroService.Domain.Entities.Especialidades;
using CadastroService.Domain.Entities.Medicos;
using CadastroService.Domain.Entities.Pacientes;
using CadastroService.Infraestructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Integration;

public class TestesIntegracao
{

    private static Mock<ILogger<T>> CreateLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    private readonly CadastroService.Infraestructure.Data.SQliteContext _contextCadastro;
    private readonly AgendamentoService.Infraestructure.Data.SQliteContext _contextAgendamento;

    private readonly MedicoRepository _medicoRepository;
    private readonly PacienteRepository _pacienteRepository;
    private readonly ClinicaRepository _clinicaRepository;
    private readonly EspecialidadeRepository _especialidadeRepository;
    private readonly ConsultaRepository _consultaRepository;

    private readonly MedicoService _medicoService;
    private readonly PacienteService _pacienteService;
    private readonly EspecialidadeService _especialidadeService;
    private readonly ClinicaService _clinicaService;
    private readonly ConsultaService _consultaService;

    private readonly CadastroRestService _cadastroRestService;
    private readonly EmailService _emailService;

    public TestesIntegracao()
    {
        var settings = new Dictionary<string, string?>
        {
            { "ConnectionStrings:CadastroServiceConnection", "Data Source={0};" },
            { "ConnectionStrings:AgendamentoServiceConnection", "Data Source={0};" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        _contextCadastro = new(config);
        _contextAgendamento = new(config);

        _especialidadeRepository = new EspecialidadeRepository(_contextCadastro);
        _medicoRepository = new MedicoRepository(_contextCadastro);
        _pacienteRepository = new PacienteRepository(_contextCadastro);
        _clinicaRepository = new ClinicaRepository(_contextCadastro);
        _consultaRepository = new ConsultaRepository(_contextAgendamento);

        _cadastroRestService = new(RestServiceConstants.CADASTRO_SERVICE_URL);
        _emailService = new();

        _especialidadeService = new(CreateLogger<EspecialidadeService>().Object, _especialidadeRepository);
        _medicoService = new(CreateLogger<MedicoService>().Object, _medicoRepository, _especialidadeRepository);
        _pacienteService = new(CreateLogger<PacienteService>().Object, _pacienteRepository);
        _clinicaService = new(CreateLogger<ClinicaService>().Object, _clinicaRepository);
        _consultaService = new(_cadastroRestService, _consultaRepository, _emailService, CreateLogger<ConsultaService>().Object);

    }

    private async Task<int> CriarEspecialidade(string nome = "Cardiologia")
        => (await _especialidadeRepository.Create(new Especialidade { Nome = nome, Ativo = true })).Id;

    private async Task<int> CriarMedico(int espId, string nome = "Dr. Teste")
        => (await _medicoRepository.Create(new Medico { Nome = nome, CRM = "CRMXYZ", Ativo = true, EspecialidadeId = espId })).Id;

    private async Task<int> CriarPaciente(string nome = "Paciente Teste")
        => (await _pacienteRepository.Create(new Paciente { Nome = nome, Ativo = true })).Id;

    private async Task<int> CriarClinica(string nome = "Clínica Teste")
        => (await _clinicaRepository.Create(new CadastroService.Domain.Entities.Clinicas.Clinica { Nome = nome, Ativo = true })).Id;

    [Fact]
    public async Task DeveCriarMedico()
    {
        var esp = await CriarEspecialidade();
        var id = await CriarMedico(esp);

        var medico = await _medicoRepository.GetById(id);
        Assert.NotNull(medico);
    }

    [Fact]
    public async Task DeveBuscarTodosMedicos()
    {
        var esp = await CriarEspecialidade();
        await CriarMedico(esp);
        await CriarMedico(esp);

        var medicos = await _medicoRepository.GetAll();
        Assert.True(medicos.Count >= 2);
    }

    [Fact]
    public async Task DeveAtualizarMedico()
    {
        var esp = await CriarEspecialidade();
        var id = await CriarMedico(esp);

        var medico = await _medicoRepository.GetById(id);
        medico!.Nome = "Dr. Alterado";

        await _medicoRepository.Update(medico);
        var atualizado = await _medicoRepository.GetById(id);

        Assert.Equal("Dr. Alterado", atualizado!.Nome);
    }

    [Fact]
    public async Task DeveExcluirMedicoSoftDelete()
    {
        var esp = await CriarEspecialidade();
        var id = await CriarMedico(esp);

        var result = await _medicoRepository.Delete(id);
        Assert.True(result);

        var medico = await _medicoRepository.GetById(id);
        Assert.Null(medico);
    }

    [Fact]
    public async Task DeveFiltrarPorEspecialidade()
    {
        var espCardio = await CriarEspecialidade("Cardio");
        var espOrto = await CriarEspecialidade("Orto");

        await CriarMedico(espCardio, "CardioTeste");
        await CriarMedico(espOrto, "OrtoTeste");

        var cardios = await _medicoRepository.GetByEspecialidade(espCardio);
        Assert.Single(cardios);
        Assert.Contains(cardios, x => x.Nome.Contains("Cardio"));
    }

    [Fact]
    public async Task Service_DeveCriarPaciente()
    {
        var id = await CriarPaciente();
        Assert.True(id > 0);
    }

    [Fact]
    public async Task Service_DeveCriarMedicoComEspecialidade()
    {
        var esp = await CriarEspecialidade();
        var id = await CriarMedico(esp);

        Assert.True(id > 0);
    }

    [Fact]
    public async Task Service_DeveListarEspecialidades()
    {
        await CriarEspecialidade();
        var esp = await _especialidadeService.GetAll();
        Assert.NotEmpty(esp);
    }
}
