using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatInput : ComponentBase
{
    private ElementReference textArea;
    private string? messageText;
    private bool hasJustSent;

    [Parameter]
    public EventCallback<ChatMessage> OnSend { get; set; }

    [Inject]
    public required IJSRuntime JS { get; set; }

    public ValueTask FocusAsync()
        => textArea.FocusAsync();

    private async Task SendMessageAsync()
    {
        if (messageText is { Length: > 0 } rawText)
        {
            var trimmedText = rawText.Trim();
            if (trimmedText.Length == 0)
            {
                messageText = null;
                return;
            }
            hasJustSent = true;
            messageText = null;
            await OnSend.InvokeAsync(new ChatMessage(ChatRole.User, trimmedText));
        }
    }

    private Task HandleKeyDown(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs e)
    {
        // Ignore while composing (IME) to avoid duplicate Enter keydown on macOS
        if (e.IsComposing == true)
        {
            return Task.CompletedTask;
        }

        // Ignore auto-repeat from holding the Enter key
        if (e.Repeat == true)
        {
            return Task.CompletedTask;
        }

        var isEnter = e.Key == "Enter" && !e.ShiftKey;
        if (isEnter == false)
        {
            // Any non-Enter key press clears the justSent guard
            hasJustSent = false;
            return Task.CompletedTask;
        }

        // Ignore immediate Enter after a send until user types again
        if (hasJustSent == true)
        {
            return Task.CompletedTask;
        }

        // Don't submit on empty Enter
        var isEmpty = string.IsNullOrWhiteSpace(messageText);
        if (isEmpty == true)
        {
            return Task.CompletedTask;
        }
        return SendMessageAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false)
        {
            return;
        }

        try
        {
            var module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Pages/Chat/ChatInput.razor.js");
            await module.InvokeVoidAsync("init", textArea);
            await module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }
}