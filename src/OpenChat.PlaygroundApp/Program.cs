using OpenChat.Common.Configurations;
using OpenChat.PlaygroundApp.Components;

var builder = WebApplication.CreateBuilder(args);

var config = AppSettings.Parse(builder.Configuration, args);
builder.Services.AddSingleton(config);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

if (config.LLM.Provider == LLMProviderType.OpenAI)
{
    builder.AddAzureOpenAIClient(config.LLM.Provider.ToString()).AddChatClient(config.OpenAI.DeploymentName);
}

if (config.LLM.Provider == LLMProviderType.Ollama || config.LLM.Provider == LLMProviderType.HuggingFace)
{
    builder.AddOllamaSharpChatClient(config.Ollama.DeploymentName);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
