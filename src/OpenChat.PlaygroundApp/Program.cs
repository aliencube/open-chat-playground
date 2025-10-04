using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.AI;
using Microsoft.OpenApi.Models;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Components;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var settings = ArgumentOptions.Parse(config, args);
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var chatClient = await LanguageModelConnector.CreateChatClientAsync(settings);

builder.Services.AddChatClient(chatClient)
                .UseFunctionInvocation()
                .UseLogging();

// 👇👇👇 OpenAPI - TO BE REFACTORED
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi("openapi", options =>
{
    options.AddDocumentTransformer<OpenApiDocumentTransformer>();
});
// 👆👆👆 OpenAPI - TO BE REFACTORED

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
}

app.UseAntiforgery();
app.UseStaticFiles();

// 👇👇👇 OpenAPI - TO BE REFACTORED
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/{documentName}.json");
}

var group = app.MapGroup("/api");
group.MapPost("/chat/responses", (IEnumerable<ChatRequest> request) =>
{
    return new List<ChatResponse>
    {
        new() { Message = "This " },
        new() { Message = "is " },
        new() { Message = "a " },
        new() { Message = "placeholder " },
        new() { Message = "response " },
        new() { Message = "from " },
        new() { Message = "the " },
        new() { Message = "chat " },
        new() { Message = "API." },
    };
})
.WithTags("Chat")
.Accepts<IEnumerable<ChatRequest>>(contentType: "application/json")
.Produces<List<ChatResponse>>(statusCode: StatusCodes.Status200OK, contentType: "application/json")
.WithName("PostChatResponses")
.WithOpenApi();
// 👆👆👆 OpenAPI - TO BE REFACTORED

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();

// 👇👇👇 OpenAPI - TO BE REFACTORED
public class ChatRequest
{
    public string? Role { get; set; }
    public string? Message { get; set; }
}

public class ChatResponse
{
    public string? Message { get; set; }
}

public class OpenApiDocumentTransformer(IHttpContextAccessor accessor) : IOpenApiDocumentTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "OpenChat Playground API",
            Version = "1.0.0",
            Description = "An API for the OpenChat Playground."
        };
        document.Servers =
        [
            new OpenApiServer
            {
                Url = accessor.HttpContext != null
                    ? $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host}/"
                    : "http://localhost:5280/"
            }
        ];

        return Task.CompletedTask;
    }
}
// 👆👆👆 OpenAPI - TO BE REFACTORED