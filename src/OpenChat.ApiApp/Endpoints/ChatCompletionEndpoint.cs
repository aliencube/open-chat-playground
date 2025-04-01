using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;

using OpenChat.ApiApp.Models;
using OpenChat.ApiApp.Services;

using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace OpenChat.ApiApp.Endpoints;

public static class ChatCompletionEndpoint
{
    public static IEndpointRouteBuilder MapChatCompletionEndpoint(this IEndpointRouteBuilder routeBuilder)
    {
        var api = routeBuilder.MapGroup("api/chat");

        api.MapPost("complete", PostChatCompletionAsync)
           .Accepts<PromptRequest>(contentType: "application/json")
           .Produces<IEnumerable<PromptResponse>>(statusCode: StatusCodes.Status200OK, contentType: "application/json")
           .WithTags("chat")
           .WithName("ChatCompletion")
           .WithOpenApi();

        api.MapPost("complete-with-role", PostChatCompletionWithRoleAsync)
           .Accepts<PromptWithRoleRequest>(contentType: "application/json")
           .Produces<IEnumerable<PromptResponse>>(statusCode: StatusCodes.Status200OK, contentType: "application/json")
           .WithTags("chat")
           .WithName("ChatCompletionWithRole")
           .WithOpenApi();

        return routeBuilder;
    }

    public static async IAsyncEnumerable<PromptResponse> PostChatCompletionAsync([FromBody] PromptRequest req, IKernelService service)
    {
        var result = service.CompleteChatStreamingAsync(req.Prompt);

        await foreach (var text in result)
        {
            yield return new PromptResponse(text);
        }
    }

    public static async IAsyncEnumerable<PromptResponse> PostChatCompletionWithRoleAsync([FromBody] IEnumerable<PromptWithRoleRequest> req, IKernelService service)
    {
        var messages = new List<ChatMessageContent>();
        foreach (var msg in req)
        {
            ChatMessageContent message = msg.Role switch
            {
                "User" => new ChatMessageContent(AuthorRole.User, msg.Content),
                "Assistant" => new ChatMessageContent(AuthorRole.Assistant, msg.Content),
                "System" => new ChatMessageContent(AuthorRole.System, msg.Content),
                _ => throw new ArgumentException($"Invalid role: {msg.Role}")
            };
            messages.Add(message);
        }

        var result = service.CompleteChatStreamingAsync(messages);
        await foreach (var text in result)
        {
            yield return new PromptResponse(text);
        }
    }
}
