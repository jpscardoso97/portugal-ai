namespace App.HttpClients;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Dto;
using Interfaces;

public class VectorDbHttpClient : IVectorDbHttpClient
{
    private const string dbHost = "http://api:5002/";
    //private const string dbHost = "http://host.docker.internal:5002/";
    //private const string dbHost = "http://localhost:8000/";

    private readonly HttpClient _httpClient;

    public VectorDbHttpClient()
    {
        _httpClient = new()
        {
            BaseAddress = new(dbHost),
        };
    }

    public async Task<IEnumerable<VectorDbSearchResult>> SearchAsync(string query, string location)
    {
        var response = await _httpClient.GetAsync($"search?query={query}&location={location}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<IEnumerable<VectorDbSearchResult>>(content) ?? Array.Empty<VectorDbSearchResult>();
    }

    public async Task<IEnumerable<VectorDbSearchResult>> SearchAsync(string query)
    {
        var response = await _httpClient.GetAsync($"search?query={query}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<IEnumerable<VectorDbSearchResult>>(content) ?? Array.Empty<VectorDbSearchResult>();
    }
}