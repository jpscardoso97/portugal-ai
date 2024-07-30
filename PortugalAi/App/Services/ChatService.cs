namespace App.Services;

using System.Text;
using Interfaces;

public class ChatService : IChatService
{
    private static readonly IEnumerable<string> ValidLocations =
    [
        "aveiro", "alentejo", "algarve", "azores", "braga", "braganca", "covilha", "coimbra", "funchal", "guimaraes", "leiria", "lisbon", "porto", "portugal", "santarem", "vila real", "viseu", 
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
        
        _logger.LogDebug("Extracted location: "+location);
        
        var l = ValidLocations.FirstOrDefault(l => location == l);

        Console.WriteLine(!string.IsNullOrWhiteSpace(l)
            ? $"Location: {l}"
            : $"Location couldn't be found, using from context: {_location}");

        if (l != null)
            _location = l;

        var searchResults = _location != null 
            ? await _dbService.SearchRecommendationsAsync(initialInput, _location) 
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