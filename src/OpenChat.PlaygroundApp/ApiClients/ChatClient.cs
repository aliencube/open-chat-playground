// using ChatMessage = OpenChat.Common.Models.ChatMessage;
// using ChatRequest = OpenChat.Common.Models.ChatRequest;
// using ChatResponse = OpenChat.Common.Models.ChatResponse;

// namespace OpenChat.PlaygroundApp.ApiClients;

// public interface IChatClient
// {
//     IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages);
// }

// public class ChatClient(HttpClient http) : IChatClient
// {
//     private const string REQUEST_URI = "api/chat/complete";

//     public async IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessage> messages)
//     {
//         var content = new ChatRequest() { Messages = [.. messages] };
//         var response = await http.PostAsJsonAsync<ChatRequest>($"{REQUEST_URI}", content);

//         response.EnsureSuccessStatusCode();

//         var responses = response.Content.ReadFromJsonAsAsyncEnumerable<ChatResponse>();
//         await foreach (var message in responses)
//         {
//             yield return message!.Content!;
//         }
//     }
// }
