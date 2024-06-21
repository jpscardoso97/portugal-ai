namespace App.Services;

using Extensions;
using Interfaces;
using Plugins;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class SemanticKernelService : ISemanticKernelService
{
    private readonly ILogger _logger;
    private readonly Kernel _kernel;
    private readonly KernelFunction _locationFunction;
    private readonly IChatCompletionService _chatCompletionService;

    private const string SystemMessage = """
                                         You are Portugal-AI, a friendly AI tourist guide assistant who likes to follow the rules. You are polite
                                         and you answer questions only with true information.
                                         """;
    
    private static readonly OpenAIPromptExecutionSettings Settings = new()
        { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

    private readonly ChatHistory _chatHistory = new(SystemMessage);

    public SemanticKernelService(ILogger<SemanticKernelService> logger, IKernelBuilder kernelBuilder)
    {
        _logger = logger;
        _kernel = kernelBuilder.Build();
        _kernel.ImportPluginFromType<RecommenderPlugin>();
        _locationFunction = _kernel.GetLocationFunction(); 
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string?> GetCompletionAsync(string prompt)
    {
        _chatHistory.AddUserMessage(prompt);
        var completions = await _chatCompletionService.GetChatMessageContentsAsync(_chatHistory, Settings, _kernel);

        var contents = completions.FirstOrDefault()?.Content;

        if (contents == null)
        {
            _logger.LogError("Got no response from LLM");
        }
        else
        {
            _chatHistory.AddAssistantMessage(contents);
        }

        return contents;
    }

    public async Task<string?> ExtractLocationFromInputAsync(string input)
    {
        var l = await _kernel.InvokeAsync<string>(_locationFunction,
            new()
            {
                { "input", input }
            }
        );

        return l?.Split('<')[0].ToLowerInvariant();
    }

    public async Task<string?> GetRecommendationsForLocationAsync(string location)
    {
        return await _kernel.InvokeAsync<string>("RecommenderPlugin",
            "GetRecommendations",
            new()
            {
                { "location", location }
            }
        );
    }
}