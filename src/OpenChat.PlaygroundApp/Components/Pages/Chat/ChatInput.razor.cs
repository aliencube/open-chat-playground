using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatInput
{
    protected ElementReference textArea;
    protected string? messageText;

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public EventCallback<ChatMessage> OnSend { get; set; }

    public ValueTask FocusAsync()
        => textArea.FocusAsync();

    private async Task SendMessageAsync()
    {
        if (messageText is { Length: > 0 } text)
        {
            messageText = null;
            await OnSend.InvokeAsync(new ChatMessage(ChatRole.User, text));
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
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
}

