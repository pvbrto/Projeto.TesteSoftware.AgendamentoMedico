using AgendamentoService.Domain.Entities;
using AgendamentoService.Domain.Entities.Consultas;
using AgendamentoService.Domain.Entities.Exceptions;
using AgendamentoService.Domain.Enums;
using AgendamentoService.Domain.Interfaces.Repositories;
using AgendamentoService.Domain.Interfaces.RestServices;
using AgendamentoService.Domain.Interfaces.Services;
using AgendamentoService.Domain.Interfaces.Services.Emails;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AgendamentoService.Application.Services
{
    public class ConsultaService(
        ICadastroRestService _cadastroRestService,
        IConsultaRepository _consultaRepository,
        IEmailService _emailService,
        ILogger<ConsultaService> _logger) : IConsultaService
    {
        public async Task<List<Consulta>> GetAll()
        {
            _logger.LogInformation("Buscando todas as consultas sem filtro.");
            return await HandleEntities(await _consultaRepository.GetAll());
        }

        public async Task<Consulta> GetById(int id)
        {
            _logger.LogInformation($"Buscando consulta com ID {id}.");
            return await HandleEntities(await _consultaRepository.GetById(id));
        }

        public async Task<bool> Delete(int id)
        {
            _logger.LogInformation($"Deletando consulta com ID {id}.");
            return await _consultaRepository.Delete(id);
        }

        public async Task<Consulta> Create(CriarConsultaDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            try
            {
                ClinicaDTO clinica = await _cadastroRestService.GetClinicaById(dto.ClinicaId)
                    ?? throw new BusinessException(HttpStatusCode.NotFound, $"Clinica com ID {dto.ClinicaId} não encontrada.");

                MedicoDTO medico = await _cadastroRestService.GetMedicoById(dto.MedicoId)
                    ?? throw new BusinessException(HttpStatusCode.NotFound, $"Médico com ID {dto.MedicoId} não encontrado.");

                PacienteDTO paciente = await _cadastroRestService.GetPacienteById(dto.PacienteId)
                    ?? throw new BusinessException(HttpStatusCode.NotFound, $"Paciente com ID {dto.PacienteId} não encontrado.");

                var conflito = await _consultaRepository.GetConsultaNoHorario(dto.MedicoId, dto.ClinicaId, dto.DataHora);

                if (conflito != null)
                {
                    _logger.LogInformation("Existe um conflito de horários.");
                    Consulta consulta = await _consultaRepository.Create(new()
                    {
                        ClinicaId = dto.ClinicaId,
                        MedicoId = dto.MedicoId,
                        PacienteId = dto.PacienteId,
                        DataHora = dto.DataHora,
                        Status = StatusAgendamento.AguardandoHorario
                    });

                    _logger.LogInformation("Enviando email (arquivo) para email.");
                    await _emailService.EnviarEmail(
                        paciente.Email.Replace(".", "_"),
                        "Conflito na consulta",
                        "Sua consulta possui um conflito de horários. Ela será salva e estará no status de Aguardando Horário. Altere o horário ou aguarde para continuar."
                    );

                    return consulta;
                }

                _logger.LogInformation("Salvando consulta.");
                var createdConsulta = await _consultaRepository.Create(new()
                {
                    ClinicaId = dto.ClinicaId,
                    MedicoId = dto.MedicoId,
                    PacienteId = dto.PacienteId,
                    DataHora = dto.DataHora,
                    Status = StatusAgendamento.Agendado
                });

                if (createdConsulta != null)
                {
                    createdConsulta.Medico = medico;
                    createdConsulta.Paciente = paciente;
                    createdConsulta.Clinica = clinica;
                }

                return createdConsulta;
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("BusinessException durante o processamento.");
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Consulta> RealizarConsulta(int id, string observacoes)
        {
            try
            {
                var consulta = await _consultaRepository.GetById(id)
                    ?? throw new BusinessException(HttpStatusCode.NotFound, "Consulta não encontrada");

                if (consulta.Status == StatusAgendamento.Encerrado) throw new BusinessException(HttpStatusCode.UnprocessableEntity, "Consulta já encerrada");

                consulta.Status = StatusAgendamento.Encerrado;
                consulta.Observacoes = observacoes;

                return await _consultaRepository.Update(consulta);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("BusinessException durante o processamento.");
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Consulta>> GetFiltro(ConsultaFiltroDTO filtro)
        {
            try
            {
                var consultas = await _consultaRepository.GetAll();
                if (consultas.Count == 0) return consultas;

                consultas = consultas.FindAll(c => c.DataHora >= filtro.DataInicio && c.DataHora <= filtro.DataFim);

                if (!filtro.Encerradas) consultas = consultas.FindAll(c => c.Status != StatusAgendamento.Encerrado);
                if (filtro.StatusConsulta != StatusAgendamento.None) consultas = consultas.FindAll(c => c.Status == filtro.StatusConsulta);
                if (filtro.MedicoId.HasValue) consultas = consultas.FindAll(c => c.MedicoId == filtro.MedicoId.Value);
                if (filtro.PacienteId.HasValue) consultas = consultas.FindAll(c => c.PacienteId == filtro.PacienteId.Value);
                if (filtro.ClinicaId.HasValue) consultas = consultas.FindAll(c => c.ClinicaId == filtro.ClinicaId.Value);

                return consultas;
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("BusinessException durante o processamento.");
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<List<Consulta>> HandleEntities(List<Consulta> consultas)
        {
            if (consultas == null) return consultas;

            foreach (var consulta in consultas)
            {
                ClinicaDTO clinica = await _cadastroRestService.GetClinicaById(consulta.ClinicaId);
                consulta.Clinica = clinica;

                MedicoDTO medico = await _cadastroRestService.GetMedicoById(consulta.MedicoId);
                consulta.Medico = medico;

                PacienteDTO paciente = await _cadastroRestService.GetPacienteById(consulta.PacienteId);
                consulta.Paciente = paciente;
            }

            return consultas;
        }

        private async Task<Consulta> HandleEntities(Consulta consulta)
        {
            if (consulta == null) return consulta;

            ClinicaDTO clinica = await _cadastroRestService.GetClinicaById(consulta.ClinicaId);
            consulta.Clinica = clinica;

            MedicoDTO medico = await _cadastroRestService.GetMedicoById(consulta.MedicoId);
            consulta.Medico = medico;

            PacienteDTO paciente = await _cadastroRestService.GetPacienteById(consulta.PacienteId);
            consulta.Paciente = paciente;

            return consulta;
        }
    }
}
