namespace App.Services;

using Interfaces;

public class ChatService : IChatService
{
    private static readonly string[] ValidLocations =
    [
        "aveiro", "porto", "portugal"
    ];
    
    private readonly ISemanticKernelService _semanticKernelService;
    
    private string? _location;
    private readonly ILogger _logger;

    public ChatService(ILogger<ChatService> logger, ISemanticKernelService semanticKernelService)
    {
        _logger = logger;
        _semanticKernelService = semanticKernelService;
    }

    public async Task<string> GetInitialResponse(string initialInput)
    {
        var location = await _semanticKernelService.ExtractLocationFromInputAsync(_location);

        // TODO: Validate location
        if (string.IsNullOrWhiteSpace(location))
        {
            _logger.LogError(
                message: "Error extracting location from user query: {initialInput}. Extracted: {location}",
                initialInput, location);
            location = null;
        }
        else
        {
            location = ValidLocations.FirstOrDefault(l => location.Contains(l));
        }

        _location = location;

        Console.WriteLine(!string.IsNullOrWhiteSpace(_location)
            ? $"Location: {_location}"
            : "Location couldn't be found");

        var recommendations = await _semanticKernelService.GetRecommendationsForLocationAsync(_location);

        Console.WriteLine("Information: \n" + recommendations);

        var prompt = $"""
                        Information: \n {recommendations}
                        Question: {initialInput}
                        
                        Answer the question according to the information above ONLY!
                      """;

        var response = await _semanticKernelService.GetCompletionAsync(prompt);

        response = CleanEof(response);

        return response;
    }

    public Task<string> GetCompletionAsync(string input)
    {
        throw new NotImplementedException();
    }

    private static string CleanEof(string res) =>
        res.Split("<|eot_id|>")[0];
}