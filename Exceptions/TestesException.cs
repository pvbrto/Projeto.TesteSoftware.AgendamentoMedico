using CadastroService.Application.Services;
using CadastroService.Domain.Entities.Especialidades;
using CadastroService.Domain.Entities.Medicos;
using CadastroService.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Exceptions
{
    public class TestesException
    {
        private readonly Mock<IMedicoRepository> _medicoRepository = new();
        private readonly Mock<IEspecialidadeRepository> _especialidadeRepository = new();
        private MedicoService _medicoService;

        public TestesException()
        {
            _medicoService = new MedicoService(CreateLogger<MedicoService>().Object, _medicoRepository.Object, _especialidadeRepository.Object);

        }

        [Fact]
        public async Task Create_DeveLancarException_QuandoEspecialidadeNaoExistir()
        {
            var medico = new Medico { Nome = "João", EspecialidadeId = 999 };

            _especialidadeRepository.Setup(x => x.GetById(999))
                .ReturnsAsync((Especialidade)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _medicoService.Create(medico));

            Assert.Contains($"Especialidade ID {medico.EspecialidadeId} não encontrada ou inativa.", ex.Message);
        }

        [Fact]
        public async Task Update_DeveLancarException_QuandoMedicoNaoExistir()
        {
            var medico = new Medico { Nome = "João", EspecialidadeId = 999 };

            _especialidadeRepository.Setup(x => x.GetById(999))
                .ReturnsAsync((Especialidade)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _medicoService.Update(medico));

            Assert.Contains($"Especialidade ID {medico.EspecialidadeId} não encontrada ou inativa.", ex.Message);
        }

        private static Mock<ILogger<T>> CreateLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }
    }
}