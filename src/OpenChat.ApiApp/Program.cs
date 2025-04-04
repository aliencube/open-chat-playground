using Microsoft.SemanticKernel;

using OllamaSharp;

using OpenChat.ApiApp.Endpoints;
using OpenChat.ApiApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddScoped<IKernelService, KernelService>();

builder.AddKeyedOllamaApiClient("exaone");

builder.Services.AddSingleton<Kernel>(sp =>
{
    var config = builder.Configuration;

    var hfaceClient = sp.GetRequiredKeyedService<IOllamaApiClient>("exaone");

    var kernel = Kernel.CreateBuilder()
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
