using Microsoft.SemanticKernel;

using OllamaSharp;

using OpenAI;

using OpenChat.ApiApp.Endpoints;
using OpenChat.ApiApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddScoped<IKernelService, KernelService>();

builder.AddAzureOpenAIClient("openai");
builder.AddKeyedOllamaApiClient("ollama-phi4-mini");
builder.AddKeyedOllamaApiClient("exaone");

builder.Services.AddSingleton<Kernel>(sp =>
{
    var config = builder.Configuration;

    var openAIClient = sp.GetRequiredService<OpenAIClient>();
    var ollamaClient = sp.GetRequiredKeyedService<IOllamaApiClient>("ollama-phi4-mini");
    var hfaceClient = sp.GetRequiredKeyedService<IOllamaApiClient>("exaone");

    var kernel = Kernel.CreateBuilder()
                       .AddOpenAIChatCompletion(
                           modelId: config["GitHub:Models:ModelId"]!,
                           openAIClient: openAIClient,
                           serviceId: "github")
                       .AddOllamaChatCompletion(
                           ollamaClient: (OllamaApiClient)ollamaClient,
                           serviceId: "ollama")
                       .AddOllamaChatCompletion(
                           ollamaClient: (OllamaApiClient)hfaceClient,
                           serviceId: "hface")
                       .Build();

    return kernel;
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapChatCompletionEndpoint();

app.Run();
