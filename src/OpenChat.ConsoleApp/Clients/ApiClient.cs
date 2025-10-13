using System.Net.Http.Json;

using OpenChat.ConsoleApp.Models;

namespace OpenChat.ConsoleApp.Clients;

/// <summary>
/// This provides interfaces to the <see cref="ApiClient"/>.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Invokes the chat API and streams the responses.
    /// </summary>
    /// <param name="messages">List of <see cref="ChatMessage"/> objects.</param>
    /// <returns>Returns the <see cref="IAsyncEnumerable{ChatMessage}"/> object containing the response details.</returns>
    Task<IAsyncEnumerable<ChatMessage>> InvokeStreamAsync(IEnumerable<ChatMessage> messages);
}

/// <summary>
/// This represents the API client entity for the Playground app.
/// </summary>
/// <param name="http"><see cref="HttpClient"/> instance.</param>
public class ApiClient(HttpClient http) : IApiClient
{
    private const string REQUEST_URI = "api/chat/responses";

    private readonly HttpClient _http = http ?? throw new ArgumentNullException(nameof(http));

    /// <inheritdoc/>
    public async Task<IAsyncEnumerable<ChatMessage>> InvokeStreamAsync(IEnumerable<ChatMessage> messages)
    {
        var response = await this._http.PostAsJsonAsync(REQUEST_URI, messages);
        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadFromJsonAsAsyncEnumerable<ChatMessage>();

        return result!;
    }
}
