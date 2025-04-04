using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

using OpenChat.ApiApp.Plugins.RestaurantAgent;
using OpenChat.Common.Models;

using ChatMessage = OpenChat.Common.Models.ChatMessage;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace OpenChat.ApiApp.Services;

public interface IKernelService
{
    IAsyncEnumerable<ChatMessage> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages);
}

public class KernelService(Kernel kernel, IConfiguration config) : IKernelService
{
    public async IAsyncEnumerable<ChatMessage> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages)
    {
        await foreach (var message in this.InvokeRestaurantServerAgentAsync(messages))
        {
            yield return message;
        }
    }

    private async IAsyncEnumerable<ChatMessage> InvokeRestaurantServerAgentAsync(IEnumerable<ChatMessageContent> messages)
    {
        var plugin = kernel.Plugins.SingleOrDefault(p => p.Name == nameof(MenuPlugin));
        if (plugin is null)
        {
            kernel.Plugins.AddFromType<MenuPlugin>();
        }

        var settings = new PromptExecutionSettings()
        {
            ServiceId = config["SemanticKernel:ServiceId"]!,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var agent = new ChatCompletionAgent()
        {
            Name = "Host",
            Instructions = "Answer questions about the menu.",
            Kernel = kernel,
            Arguments = new KernelArguments(settings)
        };

        var history = new ChatHistory();
        history.AddSystemMessage("You're a friendly host at a restaurant. Always answer in Korean.");

        history.AddRange(messages);

        var result = agent.InvokeStreamingAsync(history);
        await foreach (var text in result)
        {
            yield return new ChatMessage(RoleType.Assistant, text.ToString());
        }
    }
}
