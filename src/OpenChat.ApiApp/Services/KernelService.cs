using System.Reflection;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
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
        await foreach (var message in this.InvokeStoryTellerAgentAsync(messages))
        {
            yield return message;
        }
    }

    private async IAsyncEnumerable<ChatMessage> InvokeStoryTellerAgentAsync(IEnumerable<ChatMessageContent> messages)
    {
        var filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                                    "Agents",
                                    "StoryTellerAgent",
                                    "StoryTeller.yaml");
        var definition = File.ReadAllText(filepath);
        var template = KernelFunctionYaml.ToPromptTemplateConfig(definition);
        var agent = new ChatCompletionAgent(template, new KernelPromptTemplateFactory())
        {
            Kernel = kernel
        };
        var settings = new PromptExecutionSettings()
        {
            ServiceId = config["SemanticKernel:ServiceId"]!,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        var arguments = new KernelArguments(settings)
        {
            { "topic", messages.Last().Content },
            { "length", 3 }
        };
        var history = new ChatHistory();
        history.AddSystemMessage("You're a very good storyteller agent. Always answer in Korean.");

        history.AddRange(messages);

        var result = agent.InvokeStreamingAsync(history, arguments);
        await foreach (var text in result)
        {
            yield return new ChatMessage(RoleType.Assistant, text.ToString());
        }
    }
}
