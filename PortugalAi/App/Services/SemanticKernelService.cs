namespace App.Services;

using Interfaces;
using Plugins;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class SemanticKernelService : ISemanticKernelService
{
    private readonly ILogger _logger;
    private readonly Kernel _kernel;
    private readonly KernelPlugin _prompts;
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
        _prompts = _kernel.ImportPluginFromPromptDirectory("Prompts");   
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string?> GetCompletionAsync(string prompt)
    {
        var completions = await _chatCompletionService.GetChatMessageContentsAsync(_chatHistory, Settings, _kernel);

        var contents = completions.FirstOrDefault()?.Content;

        if (contents == null)
        {
            _logger.LogError("Got no response from LLM");
        }
        else
        {
            _chatHistory.AddUserMessage(prompt);
            _chatHistory.AddAssistantMessage(contents);
        }

        return contents;
    }

    public async Task<string?> ExtractLocationFromInputAsync(string input)
    {
        var l = await _kernel.InvokeAsync<string>(_prompts["GetLocation"],
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