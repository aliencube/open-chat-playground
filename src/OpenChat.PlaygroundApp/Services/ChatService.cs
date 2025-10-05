using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Services;

/// <summary>
/// This provides interfaces to the chat service.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Sends chat messages and streams the response.
    /// </summary>
    /// <param name="messages">The sequence of <see cref="ChatMessage"/> to send.</param>
    /// <param name="options">The <see cref="ChatOptions"/> with which to configure the request.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The <see cref="ChatResponseUpdate"/> generated.</returns>
    IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// This represents the service entity for chat operations.
/// </summary>
/// <param name="chatClient">The <see cref="IChatClient"/>.</param>
/// <param name="logger">The <see cref="ILogger{ChatService}"/>.</param>
public class ChatService(IChatClient chatClient, ILogger<ChatService> logger) : IChatService
{
    private readonly IChatClient _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    private readonly ILogger<ChatService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var chats = messages.ToList();
        if (chats.Count < 2)
        {
            throw new ArgumentException("At least two messages are required", nameof(messages));
        }

        if (chats.First().Role != ChatRole.System)
        {
            throw new ArgumentException("The first message must be a system message", nameof(messages));
        }

        if (chats.ElementAt(1).Role != ChatRole.User)
        {
            throw new ArgumentException("The second message must be a user message", nameof(messages));
        }

        this._logger.LogInformation("Requesting chat response with {MessageCount} messages", chats.Count);

        return this._chatClient.GetStreamingResponseAsync(chats, options, cancellationToken);
    }
}