using AgendamentoService.Application.Services;
using AgendamentoService.Domain.Entities;
using AgendamentoService.Domain.Entities.Consultas;
using AgendamentoService.Domain.Entities.Exceptions;
using AgendamentoService.Domain.Enums;
using AgendamentoService.Domain.Interfaces.Repositories;
using AgendamentoService.Domain.Interfaces.RestServices;
using AgendamentoService.Domain.Interfaces.Services.Emails;
using CadastroService.Application.Services;
using CadastroService.Domain.Entities.Clinicas;
using CadastroService.Domain.Entities.Especialidades;
using CadastroService.Domain.Entities.Medicos;
using CadastroService.Domain.Entities.Pacientes;
using CadastroService.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Unitarios
{
    public class TestesUnitarios
    {
        private readonly Mock<IClinicaRepository> _clinicaRepositoryMock = new();
        private readonly Mock<IPacienteRepository> _pacienteRepositoryMock = new();
        private readonly Mock<IEspecialidadeRepository> _especialidadeRepositoryMock = new();
        private readonly Mock<IMedicoRepository> _medicoRepositoryMock = new();
        private readonly Mock<IConsultaRepository> _consultaRepositoryMock = new();

        private readonly Mock<ICadastroRestService> _cadastroRestServiceMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();

        private readonly PacienteService _pacienteService;
        private readonly ClinicaService _clinicaService;
        private readonly EspecialidadeService _especialidadeService;
        private readonly MedicoService _medicoService;
        private readonly ConsultaService _consultaService;

        public TestesUnitarios()
        {
            _clinicaService = new ClinicaService(CreateLogger<ClinicaService>().Object, _clinicaRepositoryMock.Object);
            _pacienteService = new PacienteService(CreateLogger<PacienteService>().Object, _pacienteRepositoryMock.Object);
            _especialidadeService = new EspecialidadeService(CreateLogger<EspecialidadeService>().Object, _especialidadeRepositoryMock.Object);
            _medicoService = new MedicoService(CreateLogger<MedicoService>().Object, _medicoRepositoryMock.Object, _especialidadeRepositoryMock.Object);
            _consultaService = new ConsultaService(_cadastroRestServiceMock.Object, _consultaRepositoryMock.Object, _emailServiceMock.Object, CreateLogger<ConsultaService>().Object
            );
        }

        #region Clinica

        [Fact]
        public async Task Clinica_GetAll()
        {
            _clinicaRepositoryMock.Setup(s => s.GetAll()).ReturnsAsync([new()]);
            var result = await _clinicaService.GetAll();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task Clinica_GetAll_DeveRetornarListaDeClinicas()
        {
            // Arrange
            var clinicas = new List<Clinica> { new Clinica { Id = 1, Nome = "A" } };
            _clinicaRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(clinicas);

            // Act
            var result = await _clinicaService.GetAll();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public async Task Clinica_GetById_DeveRetornarClinicaQuandoExistir()
        {
            var clinica = new Clinica { Id = 1, Nome = "A" };
            _clinicaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(clinica);

            var result = await _clinicaService.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("A", result.Nome);
        }

        [Fact]
        public async Task Clinica_GetById_DeveRetornarNullQuandoNaoExistir()
        {
            _clinicaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((Clinica)null);

            var result = await _clinicaService.GetById(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task Clinica_Create_DeveChamarRepositorioERetornarClinicaCriada()
        {
            var clinica = new Clinica { Nome = "Nova" };
            _clinicaRepositoryMock.Setup(r => r.Create(clinica))
                           .ReturnsAsync(() => { clinica.Id = 123; return clinica; });

            var result = await _clinicaService.Create(clinica);

            Assert.Equal(123, result.Id);
            _clinicaRepositoryMock.Verify(r => r.Create(clinica), Times.Once);
        }

        [Fact]
        public async Task Clinica_Update_DeveRetornarClinicaAtualizada()
        {
            var clinica = new Clinica { Id = 1, Nome = "Atualizada" };
            _clinicaRepositoryMock.Setup(r => r.Update(clinica)).ReturnsAsync(clinica);

            var result = await _clinicaService.Update(clinica);

            Assert.Equal("Atualizada", result.Nome);
        }

        [Fact]
        public async Task Clinica_Update_DeveRetornarNullQuandoClinicaNaoExistir()
        {
            var clinica = new Clinica { Id = 1, Nome = "A" };
            _clinicaRepositoryMock.Setup(r => r.Update(clinica)).ReturnsAsync((Clinica)null);

            var result = await _clinicaService.Update(clinica);

            Assert.Null(result);
        }

        [Fact]
        public async Task Clinica_Delete_DeveRetornarTrueQuandoExcluido()
        {
            _clinicaRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(true);

            var result = await _clinicaService.Delete(1);

            Assert.True(result);
        }

        [Fact]
        public async Task Clinica_Delete_DeveRetornarFalseQuandoNaoExistir()
        {
            _clinicaRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(false);

            var result = await _clinicaService.Delete(1);

            Assert.False(result);
        }

        #endregion

        #region Paciente

        [Fact]
        public async Task Paciente_GetAll_DeveRetornarListaPacientes()
        {
            // Arrange
            var pacientes = new List<Paciente>
            {
                new Paciente { Id = 1, Nome = "João" }
            };
            _pacienteRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(pacientes);

            // Act
            var result = await _pacienteService.GetAll();

            // Assert
            Assert.Single(result);
            Assert.Equal("João", result[0].Nome);
        }

        [Fact]
        public async Task Paciente_GetById_DeveRetornarPaciente_QuandoExistir()
        {
            var paciente = new Paciente { Id = 1, Nome = "Maria" };
            _pacienteRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(paciente);

            var result = await _pacienteService.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Maria", result.Nome);
        }

        [Fact]
        public async Task Paciente_GetById_DeveRetornarNull_QuandoNaoExistir()
        {
            _pacienteRepositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((Paciente?)null);

            var result = await _pacienteService.GetById(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task Paciente_Create_DeveCriarPaciente()
        {
            var paciente = new Paciente { Nome = "Carlos" };

            _pacienteRepositoryMock.Setup(r => r.Create(It.IsAny<Paciente>()))
                .ReturnsAsync((Paciente p) =>
                {
                    p.Id = 1;
                    return p;
                });

            var result = await _pacienteService.Create(paciente);

            Assert.Equal(1, result.Id);
            Assert.Equal("Carlos", result.Nome);
        }

        [Fact]
        public async Task Paciente_Update_DeveAtualizarPaciente_QuandoExistir()
        {
            var paciente = new Paciente { Id = 1, Nome = "Ana" };

            _pacienteRepositoryMock.Setup(r => r.Update(It.IsAny<Paciente>()))
                .ReturnsAsync(paciente);

            var result = await _pacienteService.Update(paciente);

            Assert.NotNull(result);
            Assert.Equal("Ana", result.Nome);
        }

        [Fact]
        public async Task Paciente_Delete_DeveRetornarTrue_QuandoPacienteExistir()
        {
            _pacienteRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(true);

            var result = await _pacienteService.Delete(1);

            Assert.True(result);
        }

        #endregion

        #region Especialidade

        [Fact]
        public async Task Especialidade_GetAll_DeveRetornarListaEspecialidades()
        {
            var especialidades = new List<Especialidade>
            {
                new Especialidade { Id = 1, Nome = "Ortopedia" }
            };

            _especialidadeRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(especialidades);

            var result = await _especialidadeService.GetAll();

            Assert.Single(result);
            Assert.Equal("Ortopedia", result[0].Nome);
        }

        [Fact]
        public async Task Especialidade_GetById_DeveRetornarEspecialidade_QuandoExistir()
        {
            var esp = new Especialidade { Id = 1, Nome = "Dermatologia" };
            _especialidadeRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(esp);

            var result = await _especialidadeService.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Dermatologia", result?.Nome);
        }

        [Fact]
        public async Task Especialidade_GetById_DeveRetornarNull_QuandoNaoExistir()
        {
            _especialidadeRepositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((Especialidade?)null);

            var result = await _especialidadeService.GetById(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task Especialidade_Create_DeveCriarEspecialidade()
        {
            var nova = new Especialidade { Nome = "Pediatria" };

            _especialidadeRepositoryMock.Setup(r => r.Create(It.IsAny<Especialidade>()))
                .ReturnsAsync((Especialidade e) =>
                {
                    e.Id = 1;
                    return e;
                });

            var result = await _especialidadeService.Create(nova);

            Assert.Equal(1, result.Id);
            Assert.Equal("Pediatria", result.Nome);
        }

        [Fact]
        public async Task Especialidade_Update_DeveAtualizarEspecialidade_QuandoExistir()
        {
            var especialidade = new Especialidade { Id = 1, Nome = "Cardiologia" };

            _especialidadeRepositoryMock.Setup(r => r.Update(It.IsAny<Especialidade>()))
                .ReturnsAsync(especialidade);

            var result = await _especialidadeService.Update(especialidade);

            Assert.NotNull(result);
            Assert.Equal("Cardiologia", result?.Nome);
        }

        [Fact]
        public async Task Especialidade_Delete_DeveRetornarTrue_QuandoEspecialidadeExistir()
        {
            _especialidadeRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(true);

            var result = await _especialidadeService.Delete(1);

            Assert.True(result);
        }

        #endregion

        #region Medico

        [Fact]
        public async Task GetAll_DeveRetornarListaDeMedicos()
        {
            _medicoRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(
                new List<Medico> { new Medico { Id = 1, Nome = "Dr. João" } }
            );

            var result = await _medicoService.GetAll();

            Assert.Single(result);
            Assert.Equal("Dr. João", result[0].Nome);
        }

        [Fact]
        public async Task GetById_DeveRetornarMedico()
        {
            var medico = new Medico { Id = 1, Nome = "Dr. Pedro" };
            _medicoRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(medico);

            var result = await _medicoService.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Dr. Pedro", result?.Nome);
        }

        [Fact]
        public async Task Create_DeveCriarMedico_ComEspecialidadeValida()
        {
            var especialidade = new Especialidade { Id = 10, Nome = "Ortopedia", Ativo = true };
            _especialidadeRepositoryMock.Setup(r => r.GetById(10)).ReturnsAsync(especialidade);

            _medicoRepositoryMock.Setup(r => r.Create(It.IsAny<Medico>()))
                .ReturnsAsync((Medico m) =>
                {
                    m.Id = 1;
                    return m;
                });

            var medico = new Medico { Nome = "Dr. Mario", EspecialidadeId = 10 };

            var result = await _medicoService.Create(medico);

            Assert.Equal(1, result.Id);
            Assert.Equal("Ortopedia", result.Especialidade.Nome);
        }

        [Fact]
        public async Task Create_DeveLancarErro_SeEspecialidadeInvalida()
        {
            _especialidadeRepositoryMock.Setup(r => r.GetById(99))
                .ReturnsAsync((Especialidade?)null);

            var medico = new Medico { Nome = "Dr. Carlos", EspecialidadeId = 99 };

            await Assert.ThrowsAsync<Exception>(() => _medicoService.Create(medico));
        }

        [Fact]
        public async Task GetByEspecialidade_DeveRetornarMedicos()
        {
            _medicoRepositoryMock.Setup(r => r.GetByEspecialidade(3))
                .ReturnsAsync(new List<Medico> { new Medico { Nome = "Dr. Ana" } });

            var result = await _medicoService.GetByEspecialidade(3);

            Assert.Single(result);
            Assert.Equal("Dr. Ana", result[0].Nome);
        }

        #endregion

        #region Consulta

        [Fact]
        public async Task Create_DeveLancarException_QuandoMedicoOuClinicaOuPacienteNaoExistir()
        {
            var dto = new CriarConsultaDTO { ClinicaId = 1, MedicoId = 2, PacienteId = 3, DataHora = DateTime.Now };

            _cadastroRestServiceMock.Setup(x => x.GetClinicaById(1)).ReturnsAsync((ClinicaDTO)null);

            await Assert.ThrowsAsync<BusinessException>(() => _consultaService.Create(dto));
        }

        [Fact]
        public async Task Create_DeveCriarConsultaComStatusAgendado_QuandoNaoHouverConflito()
        {
            var dto = new CriarConsultaDTO { ClinicaId = 1, MedicoId = 2, PacienteId = 3, DataHora = DateTime.Now };

            _cadastroRestServiceMock.Setup(x => x.GetClinicaById(1)).ReturnsAsync(new ClinicaDTO { Id = 1 });
            _cadastroRestServiceMock.Setup(x => x.GetMedicoById(2)).ReturnsAsync(new MedicoDTO { Id = 2 });
            _cadastroRestServiceMock.Setup(x => x.GetPacienteById(3)).ReturnsAsync(new PacienteDTO { Id = 3 });

            _consultaRepositoryMock.Setup(r => r.GetConsultaNoHorario(2, 1, dto.DataHora))
                .ReturnsAsync((Consulta)null);

            _consultaRepositoryMock.Setup(r => r.Create(It.IsAny<Consulta>()))
                .ReturnsAsync((Consulta c) => { c.Id = 10; c.Status = StatusAgendamento.Agendado; return c; });

            var result = await _consultaService.Create(dto);

            Assert.NotNull(result);
            Assert.Equal(StatusAgendamento.Agendado, result.Status);
            Assert.Equal(10, result.Id);
        }

        [Fact]
        public async Task RealizarConsulta_DeveEncerrarConsulta()
        {
            var consulta = new Consulta { Id = 1, Status = StatusAgendamento.Agendado };

            _consultaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(consulta);
            _consultaRepositoryMock.Setup(r => r.Update(It.IsAny<Consulta>()))
                .ReturnsAsync((Consulta c) => c);

            var result = await _consultaService.RealizarConsulta(1, "ok");

            Assert.Equal(StatusAgendamento.Encerrado, result.Status);
            Assert.Equal("ok", result.Observacoes);
        }

        [Fact]
        public async Task RealizarConsulta_DeveLancarException_SeConsultaJaEncerrada()
        {
            var consulta = new Consulta { Id = 1, Status = StatusAgendamento.Encerrado };
            _consultaRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(consulta);

            await Assert.ThrowsAsync<BusinessException>(() => _consultaService.RealizarConsulta(1, "teste"));
        }

        #endregion

        private static Mock<ILogger<T>> CreateLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }
    }
}