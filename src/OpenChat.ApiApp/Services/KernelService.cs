using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

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
        await foreach (var message in this.InvokeChatMessageContentAsync(messages))
        {
            yield return message;
        }
    }

    private async IAsyncEnumerable<ChatMessage> InvokeChatMessageContentAsync(IEnumerable<ChatMessageContent> messages)
    {
        var history = new ChatHistory();
        history.AddRange(messages);

        var service = kernel.GetRequiredService<IChatCompletionService>();
        var settings = new PromptExecutionSettings()
        {
            ServiceId = config["SemanticKernel:ServiceId"]!
        };

        var result = service.GetStreamingChatMessageContentsAsync(chatHistory: history, executionSettings: settings, kernel: kernel);
        await foreach (var text in result)
        {
            yield return new ChatMessage(RoleType.Assistant, text.ToString());
        }
    }
}
