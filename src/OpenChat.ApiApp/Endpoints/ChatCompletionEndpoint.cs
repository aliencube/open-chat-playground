using OpenChat.ApiApp.Delegates;

using ChatRequest = OpenChat.Common.Models.ChatRequest;
using ChatResponse = OpenChat.Common.Models.ChatResponse;

namespace OpenChat.ApiApp.Endpoints;

public static class ChatCompletionEndpoint
{
    public static IEndpointRouteBuilder MapChatCompletionEndpoint(this IEndpointRouteBuilder routeBuilder)
    {
        var api = routeBuilder.MapGroup("api/chat")
                              .WithTags("chat");

        api.MapPost("complete", ChatCompletionDelegate.PostChatCompletionStreamingAsync)
           .Accepts<ChatRequest>(contentType: "application/json")
           .Produces<IEnumerable<ChatResponse>>(statusCode: StatusCodes.Status200OK, contentType: "application/json")
           .WithName("ChatCompletion")
           .WithOpenApi();

        return routeBuilder;
    }
}
