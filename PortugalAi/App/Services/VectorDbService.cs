namespace App.Services;

using HttpClients.Interfaces;
using Dto;
using Interfaces;

public class VectorDbService : IVectorDbService
{
    private readonly IVectorDbHttpClient _client;
    
    public VectorDbService(IVectorDbHttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<VectorDbSearchResult>> SearchRecommendationsAsync(string query)
    {
        try
        {
            return await _client.SearchAsync(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            return Array.Empty<VectorDbSearchResult>();
        }
    }

    public async Task<IEnumerable<VectorDbSearchResult>> SearchRecommendationsAsync(string query, string location)
    {
        try
        {
            return await _client.SearchAsync(query, location);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            return Array.Empty<VectorDbSearchResult>();
        }
    }
}