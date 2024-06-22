namespace App.Services;

using System.Text;
using Interfaces;

public class ChatService : IChatService
{
    private static readonly string[] ValidLocations =
    [
        "aveiro", "porto", "portugal"
    ];

    private readonly ISemanticKernelService _semanticKernelService;
    private readonly IVectorDbService _dbService;
    private readonly ILogger _logger;

    private string? _location;

    public ChatService(
        ILogger<ChatService> logger,
        ISemanticKernelService semanticKernelService,
        IVectorDbService dbService)
    {
        _logger = logger;
        _semanticKernelService = semanticKernelService;
        _dbService = dbService;
    }

    public async Task<string> GetInitialResponse(string initialInput)
    {
        var location = await _semanticKernelService.ExtractLocationFromInputAsync(initialInput);

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

        var searchResults = location != null 
            ? await _dbService.SearchRecommendationsAsync(initialInput, location) 
            : await _dbService.SearchRecommendationsAsync(initialInput);
        
        var stringBuilder = new StringBuilder();
        
        foreach (var res in searchResults)
        {
            stringBuilder.AppendLine(res.Text);
        }

        var context = stringBuilder.Length == 0 ? null : stringBuilder.ToString();
        
        Console.WriteLine("Information: \n" + context);

        var prompt = $"""
                        Information: \n {context}
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