using App.HttpClients;
using App.Services;
using App.Services.Interfaces;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    // TODO: Change when multiple sessions are implemented
    .AddLogging(configure =>
    {
        configure.ClearProviders();
        configure.AddConsole();
        configure.SetMinimumLevel(LogLevel.Information);
    })
    .AddSingleton<IKernelBuilder>(sp =>
    {
        var kb = Kernel.CreateBuilder();
        kb.Services.AddLogging(c => c.SetMinimumLevel(LogLevel.Trace));
        kb.AddOpenAIChatCompletion(modelId: "local-llm", apiKey: "no-api-key-needed-for-local-llm",
            httpClient: new HttpClient(new LlmHttpRequestHandler()));

        return kb;
    })
    .AddSingleton<ISemanticKernelService, SemanticKernelService>()
    .AddSingleton<IChatService, ChatService>();
    
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();