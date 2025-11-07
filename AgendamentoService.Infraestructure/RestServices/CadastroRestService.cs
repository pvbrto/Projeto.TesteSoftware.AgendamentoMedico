using AgendamentoService.Domain.Entities;
using AgendamentoService.Domain.Interfaces.RestServices;
using System.Net.Http.Json;
using System.Text.Json;

namespace AgendamentoService.Infraestructure.RestServices
{
    public class CadastroRestService : ICadastroRestService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public CadastroRestService(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<MedicoDTO>> GetMedicosByEspecialidade(int especialidadeId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<MedicoDTO>>($"Medico/ByEspecialidade/{especialidadeId}", _jsonOptions);
            return response ?? new List<MedicoDTO>();
        }

        public async Task<ClinicaDTO> GetClinicaById(int clinicaId)
        {
            var response = await _httpClient.GetFromJsonAsync<ClinicaDTO>($"Clinica/{clinicaId}", _jsonOptions);
            return response;
        }

        public async Task<MedicoDTO> GetMedicoById(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<MedicoDTO>($"Medico/{id}", _jsonOptions);
            return response;
        }

        public async Task<PacienteDTO> GetPacienteById(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<PacienteDTO>($"Paciente/{id}", _jsonOptions);
            return response;
        }
    }
}
