using System.Reflection;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
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
        await foreach (var message in this.InvokeChatMessageContentAsync(messages))
        // await foreach (var message in this.InvokeStoryTellerAgentAsync(messages))
        // await foreach (var message in this.InvokeRestaurantServerAgentAsync(messages))
        // await foreach (var message in this.InvokeAgentCollaborationAsync(messages))
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

    private async IAsyncEnumerable<ChatMessage> InvokeRestaurantServerAgentAsync(IEnumerable<ChatMessageContent> messages)
    {
        var plugin = kernel.Plugins.SingleOrDefault(p => p.Name == "MenuPlugin");
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

    private async IAsyncEnumerable<ChatMessage> InvokeAgentCollaborationAsync(IEnumerable<ChatMessageContent> messages)
    {
        var reviewerName = "ProjectManager";
        var reviewerInstructions =
            """
            You are a project manager who has opinions about copywriting born of a love for David Ogilvy.
            The goal is to determine if the given copy is acceptable to print.
            If so, state that it is approved.
            If not, provide insight on how to refine suggested copy without examples.
            """;

        var copywriterName = "Copywriter";
        var copywriterInstructions =
            """
            You are a copywriter with ten years of experience and are known for brevity and a dry humor.
            The goal is to refine and decide on the single best copy as an expert in the field.
            Only provide a single proposal per response.
            Never delimit the response with quotation marks.
            You're laser focused on the goal at hand.
            Don't waste time with chit chat.
            Consider suggestions when refining an idea.
            """;

        var agentReviewer = new ChatCompletionAgent()
        {
            Name = reviewerName,
            Instructions = reviewerInstructions,
            Kernel = kernel
        };

        var agentWriter = new ChatCompletionAgent()
        {
            Name = copywriterName,
            Instructions = copywriterInstructions,
            Kernel = kernel
        };

        var terminationFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                """
                Determine if the copy has been approved. If so, respond with a single word: yes

                History:
                {{$history}}
                """,
                safeParameterNames: "history");

        var selectionFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Determine which participant takes the next turn in a conversation based on the the most recent participant.
                State only the name of the participant to take the next turn.
                No participant should take more than one turn in a row.
                
                Choose only from these participants:
                - {{{reviewerName}}}
                - {{{copywriterName}}}
                
                Always follow these rules when selecting the next participant:
                - After {{{copywriterName}}}, it is {{{reviewerName}}}'s turn.
                - After {{{reviewerName}}}, it is {{{copywriterName}}}'s turn.

                History:
                {{$history}}
                """,
                safeParameterNames: "history");

        // Limit history used for selection and termination to the most recent message.
        var strategyReducer = new ChatHistoryTruncationReducer(1);

        var chat = new AgentGroupChat(agentWriter, agentReviewer)
        {
            ExecutionSettings = new AgentGroupChatSettings()
            {
                // Here a KernelFunctionSelectionStrategy selects agents based on a prompt function.
                SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, kernel)
                {
                    // Always start with the writer agent.
                    InitialAgent = agentWriter,
                    // Returns the entire result value as a string.
                    ResultParser = (result) => result.GetValue<string>() ?? copywriterName,
                    // The prompt variable name for the history argument.
                    HistoryVariableName = "history",
                    // Save tokens by not including the entire history in the prompt
                    HistoryReducer = strategyReducer,
                    // Only include the agent names and not the message content
                    EvaluateNameOnly = true,
                },
                // Here KernelFunctionTerminationStrategy will terminate when the art-director has given their approval.
                TerminationStrategy = new KernelFunctionTerminationStrategy(terminationFunction, kernel)
                {
                    // Only the art-director may approve.
                    Agents = [ agentReviewer ],
                    // Customer result parser to determine if the response is "yes"
                    ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.InvariantCultureIgnoreCase) ?? false,
                    // The prompt variable name for the history argument.
                    HistoryVariableName = "history",
                    // Limit total number of turns
                    MaximumIterations = 10,
                    // Save tokens by not including the entire history in the prompt
                    HistoryReducer = strategyReducer,
                    // Reset for restart
                    AutomaticReset = true,
                },
            }
        };

        chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, messages.Last().Content));

        var response = chat.InvokeStreamingAsync();
        await foreach (var content in response)
        {
            await Task.Delay(20);

            yield return new ChatMessage(RoleType.Assistant, content.Content) { AgentName = content.AuthorName };
        }
    }
}
