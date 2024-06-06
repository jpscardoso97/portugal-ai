using App.HttpClients;
using App.Services;
using App.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// TODO: Change when multiple sessions are implemented
builder.Services.AddSingleton<IChatService>(sp => new ChatService(new HttpClient(new LlmHttpRequestHandler())));
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