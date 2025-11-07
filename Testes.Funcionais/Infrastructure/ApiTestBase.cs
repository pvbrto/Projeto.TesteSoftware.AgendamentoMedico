using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;

namespace Testes.Funcionais.Infrastructure
{
    public abstract class ApiTestBase<TStartup> : IClassFixture<TestWebApplicationFactory<TStartup>> where TStartup : class
    {
        protected readonly HttpClient _client;
        protected readonly TestWebApplicationFactory<TStartup> _factory;

        protected ApiTestBase(TestWebApplicationFactory<TStartup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync(endpoint, content);
        }

        protected async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PutAsync(endpoint, content);
        }

        protected async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            return await _client.GetAsync(endpoint);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return await _client.DeleteAsync(endpoint);
        }

        protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        protected async Task<string> GetResponseContent(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }
    }
}