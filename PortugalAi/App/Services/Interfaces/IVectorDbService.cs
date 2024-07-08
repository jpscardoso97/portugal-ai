namespace App.Services.Interfaces;

using Dto;

public interface IVectorDbService
{
    Task<IEnumerable<VectorDbSearchResult>> SearchRecommendationsAsync(string query);
    Task<IEnumerable<VectorDbSearchResult>> SearchRecommendationsAsync(string query, string location);
}