namespace App.HttpClients.Interfaces;

using Dto;

public interface IVectorDbHttpClient
{
    Task<IEnumerable<VectorDbSearchResult>> SearchAsync(string query, string location);
    
    Task<IEnumerable<VectorDbSearchResult>> SearchAsync(string query);
}