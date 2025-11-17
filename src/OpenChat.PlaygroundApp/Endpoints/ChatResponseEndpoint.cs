using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Models;
using OpenChat.PlaygroundApp.Services;

using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using ChatResponse = OpenChat.PlaygroundApp.Models.ChatResponse;

namespace OpenChat.PlaygroundApp.Endpoints;

/// <summary>
/// This represents the endpoint entity for chat operations.
/// </summary>
/// <param name="chatService">The <see cref="IChatService"/>.</param>
/// <param name="logger">The <see cref="ILogger{ChatResponseEndpoint}"/>.</param>
public class ChatResponseEndpoint(IChatService chatService, ILogger<ChatResponseEndpoint> logger) : IEndpoint
{
    private readonly IChatService _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
    private readonly ILogger<ChatResponseEndpoint> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/chat/responses", PostChatResponseAsync)
           .WithTags("Chat")
           .Accepts<IEnumerable<ChatRequest>>(contentType: "application/json")
           .Produces<List<ChatResponse>>(statusCode: StatusCodes.Status200OK, contentType: "application/json")
           .WithName("PostChatResponses")
           .AddOpenApiOperationTransformer((operation, context, token) =>
           {
               operation.Summary = "Get streaming chat responses based on the provided chat requests.";
               operation.Description = "This endpoint accepts a list of chat requests and returns streaming chat responses.";
               return Task.CompletedTask;
           });
    }

    private async IAsyncEnumerable<ChatResponse> PostChatResponseAsync(
        [FromBody] IEnumerable<ChatRequest> request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chats = request.ToList();

        this._logger.LogInformation("Received {RequestCount} chat requests", chats.Count);

        var messages = chats.Select(chat => new ChatMessage(new(chat.Role), chat.Message));
        var options = new ChatOptions();

        var result = this._chatService.GetStreamingResponseAsync(messages, options, cancellationToken: cancellationToken);
        await foreach (var update in result)
        {
            yield return new ChatResponse { Role = update.Role?.Value ?? string.Empty, Message = update.Text };
        }
    }
}
