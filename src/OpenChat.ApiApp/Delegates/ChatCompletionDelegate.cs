using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using OpenChat.ApiApp.Services;
using OpenChat.Common.Models;

using ChatRequest = OpenChat.Common.Models.ChatRequest;
using ChatResponse = OpenChat.Common.Models.ChatResponse;

namespace OpenChat.ApiApp.Delegates;

public static class ChatCompletionDelegate
{
    public static async IAsyncEnumerable<ChatResponse> PostChatCompletionStreamingAsync([FromBody] ChatRequest req, IKernelService service)
    {
        var messages = new List<ChatMessageContent>();
        foreach (var msg in req.Messages)
        {
            ChatMessageContent message = msg.Role switch
            {
                RoleType.System => new ChatMessageContent(AuthorRole.System, msg.Content),
                RoleType.User => new ChatMessageContent(AuthorRole.User, msg.Content),
                RoleType.Assistant => new ChatMessageContent(AuthorRole.Assistant, msg.Content),
                RoleType.Tool => new ChatMessageContent(AuthorRole.Tool, msg.Content),
                _ => throw new ArgumentException($"Invalid role: {msg.Role}")
            };
            messages.Add(message);
        }

        var result = service.CompleteChatStreamingAsync(messages);
        await foreach (var text in result)
        {
            yield return new ChatResponse(text);
        }
    }
}