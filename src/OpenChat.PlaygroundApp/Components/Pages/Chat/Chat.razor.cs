using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Services;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class Chat : ComponentBase, IDisposable
{
    private const string SystemPrompt = @"
        You are an assistant who answers questions about anything.
        Do not answer questions about anything else.
        Use only simple markdown to format your responses.
        ";

    private readonly ChatOptions chatOptions = new();
    private readonly List<ChatMessage> messages = new();
    private CancellationTokenSource? currentResponseCancellation;
    private ChatMessage? currentResponseMessage;
    private ChatInput? chatInput;

    [Inject]
    public required IChatService ChatService { get; set; }
    
    [Inject]
    public required NavigationManager Nav { get; set; }

    protected override void OnInitialized()
    {
        messages.Add(new(ChatRole.System, SystemPrompt));
    }

    private async Task AddUserMessageAsync(ChatMessage userMessage)
    {
        CancelAnyCurrentResponse();

        // Add the user message to the conversation
        messages.Add(userMessage);
        await chatInput!.FocusAsync();

        // Stream and display a new response from the IChatClient
        var responseText = new TextContent("");
        currentResponseMessage = new ChatMessage(ChatRole.Assistant, [responseText]);
        currentResponseCancellation = new();

        await InvokeAsync(StateHasChanged);

        await foreach (var update in ChatService.GetStreamingResponseAsync([.. messages], chatOptions, currentResponseCancellation.Token))
        {
            messages.AddMessages(update, filter: c => c is not TextContent);
            responseText.Text += update.Text;
            ChatMessageItem.NotifyChanged(currentResponseMessage);
        }

        // Store the final response in the conversation, and begin getting suggestions
        messages.Add(currentResponseMessage!);
        currentResponseMessage = null;
    }

    private void CancelAnyCurrentResponse()
    {
        // If a response was cancelled while streaming, include it in the conversation so it's not lost
        if (currentResponseMessage is not null)
        {
            messages.Add(currentResponseMessage);
        }

        currentResponseCancellation?.Cancel();
        currentResponseMessage = null;
    }

    private async Task ResetConversationAsync()
    {
        CancelAnyCurrentResponse();
        messages.Clear();
        messages.Add(new(ChatRole.System, SystemPrompt));
        await chatInput!.FocusAsync();
    }

    public void Dispose()
        => currentResponseCancellation?.Cancel();
}
