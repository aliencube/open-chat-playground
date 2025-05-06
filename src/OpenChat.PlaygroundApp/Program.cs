using OpenChat.Common.Configurations;
using OpenChat.PlaygroundApp.Components;

var builder = WebApplication.CreateBuilder(args);

var config = AppSettings.Parse(builder.Configuration, args);
if (config.Help == true)
{
    Console.WriteLine("Usage: dotnet run -- [options]");
    return;
}
if (config.LLM.ProviderType == LLMProviderType.Undefined)
{
    Console.WriteLine("Usage: dotnet run -- [options]");
    return;
}

builder.Services.AddSingleton(config);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// Add OpenAI
if (config.LLM.ProviderType == LLMProviderType.OpenAI)
{
    builder.AddAzureOpenAIClient(config.LLM.ProviderType.ToString().ToLowerInvariant()).AddChatClient(config.OpenAI.DeploymentName);
}

// Add Ollama or Hugging Face
if (config.LLM.ProviderType == LLMProviderType.Ollama || config.LLM.ProviderType == LLMProviderType.HuggingFace)
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
