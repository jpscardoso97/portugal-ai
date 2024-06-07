namespace App.Services.Interfaces;

public interface ISemanticKernelService
{
    Task<string?> GetCompletionAsync(string prompt);
    
    Task<string?> ExtractLocationFromInputAsync(string input);

    Task<string?> GetRecommendationsForLocationAsync(string location);
}