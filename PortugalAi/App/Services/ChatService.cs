using System.Text;

namespace App.Services;

using Plugins;
using Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class ChatService : IChatService
{
    private string[] validLocations = new[]
    {
        "aveiro", "porto"
    };
    
    private const string SystemMessage = """
                                         You are Portugal-AI, a friendly AI tourist guide assistant who likes to follow the rules. You are polite
                                         and you answer questions only with true information.
                                         """;

    private static readonly OpenAIPromptExecutionSettings Settings = new()
        { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

    private readonly Kernel _kernel;
    private readonly KernelPlugin _prompts;
    private readonly IChatCompletionService _chatCompletionService;


    private ChatHistory _chatHistory = new(SystemMessage);
    private string? _location;

    public ChatService(HttpClient llmHttpClient)
    {
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Services.AddLogging(c => c.SetMinimumLevel(LogLevel.Trace));
        kernelBuilder.AddOpenAIChatCompletion(modelId: "local-llm", apiKey: "no-api-key-needed-for-local-llm",
            httpClient: llmHttpClient);
        _kernel = kernelBuilder.Build();
        _kernel.ImportPluginFromType<RecommenderPlugin>();
        _prompts = _kernel.ImportPluginFromPromptDirectory("Prompts");
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GetInitialResponse(string initialInput)
    {
        var l = await _kernel.InvokeAsync<string>(_prompts["GetLocation"],
            new()
            {
                { "input", initialInput }
            }
        );

        _location = l?.Split('<')[0].ToLowerInvariant();

        // TODO: Validate location
        if (string.IsNullOrWhiteSpace(_location) || !validLocations.Contains(_location))
        {
            return "Invalid location. Please try again.";
        }

        Console.WriteLine($"Location: {_location}");

        var recommendations = await _kernel.InvokeAsync<string>("RecommenderPlugin",
            "GetRecommendations",
            new()
            {
                { "location", _location }
            }
        );

        Console.WriteLine("Information: \n" + recommendations);

        var prompt = $"""
                        Information: \n {recommendations}
                        Question: {initialInput}
                        
                        Answer the question according to the information above ONLY!
                      """;

        _chatHistory.AddUserMessage(prompt);

        //builder.Clear();

        var completions = await _chatCompletionService.GetChatMessageContentsAsync(_chatHistory, Settings, _kernel);

        var contents = completions.FirstOrDefault()?.Content;

        if (contents == null)
        {
            //TODO: Log error
        }

        contents = contents.Split("<|eot_id|>")[0];
        
        _chatHistory.AddAssistantMessage(contents);

        return contents;
    }

    public async Task<string> GetCompletion(string input)
    {
        throw new NotImplementedException();
    }

    private async IAsyncEnumerable<string> GetAsyncStreamResponse()
    {
        var responseBuilder = new StringBuilder();
        
        try
        {
            await foreach (var message in _chatCompletionService.GetStreamingChatMessageContentsAsync(_chatHistory,
                               executionSettings: Settings, kernel: _kernel))
            {
                if (message.Content?.StartsWith("<") ?? true)
                    break;

                responseBuilder.Append(message.Content);
                
                yield return message.Content;
            }
        }
        finally
        {
            _chatHistory.AddAssistantMessage(responseBuilder.ToString());
        }
    }
}