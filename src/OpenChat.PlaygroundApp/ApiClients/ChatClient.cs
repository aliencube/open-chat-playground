using OpenChat.PlaygroundApp.Models;

namespace OpenChat.PlaygroundApp.ApiClients;

public interface IChatClient
{
    IAsyncEnumerable<string> CompleteChatStreamingAsync(string prompt);
    IAsyncEnumerable<string> CompleteChatStreamingWithHistoryAsync(IEnumerable<ChatMessage> messages);
}

public class ChatClient(HttpClient http) : IChatClient
{
    private const string REQUEST_URI = "api/chat/complete";

    public async IAsyncEnumerable<string> CompleteChatStreamingAsync(string prompt)
    {
        var content = new PromptRequest(prompt);
        var response = await http.PostAsJsonAsync<PromptRequest>(REQUEST_URI, content);

        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadFromJsonAsAsyncEnumerable<PromptResponse>();
        await foreach (var message in result)
        {
            yield return message!.Content;
        }
    }

    public async IAsyncEnumerable<string> CompleteChatStreamingWithHistoryAsync(IEnumerable<ChatMessage> messages)
    {
        var content = messages.Select(p => new PromptWithRoleRequest(p.Role, p.Content));
        var response = await http.PostAsJsonAsync<IEnumerable<PromptWithRoleRequest>>($"{REQUEST_URI}-with-role", content);

        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadFromJsonAsAsyncEnumerable<PromptResponse>();
        await foreach (var message in result)
        {
            yield return message!.Content;
        }
    }
}
