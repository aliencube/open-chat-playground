using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class Chat : ComponentBase, IDisposable
{
    private const string SystemPrompt = @"
        You are an assistant who answers questions about anything.
        Do not answer questions about anything else.
        Use only simple markdown to format your responses.
        ";

    protected readonly ChatOptions chatOptions = new();
    protected readonly List<ChatMessage> messages = new();
    protected CancellationTokenSource? currentResponseCancellation;
    protected ChatMessage? currentResponseMessage;
    protected ChatInput? chatInput;

    [Inject]
    public required IChatClient ChatClient { get; set; }
    
    [Inject]
    public required NavigationManager Nav { get; set; }

    protected override void OnInitialized()
    {
        messages.Add(new(ChatRole.System, SystemPrompt));
    }

    protected async Task AddUserMessageAsync(ChatMessage userMessage)
    {
        CancelAnyCurrentResponse();

        // Add the user message to the conversation
        messages.Add(userMessage);
        await chatInput!.FocusAsync();

        // Stream and display a new response from the IChatClient
        var responseText = new TextContent("");
        currentResponseMessage = new ChatMessage(ChatRole.Assistant, [responseText]);
        currentResponseCancellation = new();
        await foreach (var update in ChatClient.GetStreamingResponseAsync([.. messages], chatOptions, currentResponseCancellation.Token))
        {
            messages.AddMessages(update, filter: c => c is not TextContent);
            responseText.Text += update.Text;
            ChatMessageItem.NotifyChanged(currentResponseMessage);
            StateHasChanged();
        }

        // Store the final response in the conversation, and begin getting suggestions
        messages.Add(currentResponseMessage!);
        currentResponseMessage = null;
        StateHasChanged();
    }

    protected void CancelAnyCurrentResponse()
    {
        // If a response was cancelled while streaming, include it in the conversation so it's not lost
        if (currentResponseMessage is not null)
        {
            messages.Add(currentResponseMessage);
        }

        currentResponseCancellation?.Cancel();
        currentResponseMessage = null;
    }

    protected async Task ResetConversationAsync()
    {
        CancelAnyCurrentResponse();
        messages.Clear();
        messages.Add(new(ChatRole.System, SystemPrompt));
        await chatInput!.FocusAsync();
        StateHasChanged();
    }

    public void Dispose()
        => currentResponseCancellation?.Cancel();
}
