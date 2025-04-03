using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace OpenChat.ApiApp.Services;

public interface IKernelService
{
    IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages);
}

public class KernelService(Kernel kernel, IConfiguration config) : IKernelService
{
    public async IAsyncEnumerable<string> CompleteChatStreamingAsync(IEnumerable<ChatMessageContent> messages)
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
            yield return text.ToString();
        }
    }
}
