using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.AI;
using Microsoft.JSInterop;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatMessageList : ComponentBase
{
    [Inject]
    public IJSRuntime? JS { get; set; }

    [Parameter]
    public required IEnumerable<ChatMessage> Messages { get; set; }

    [Parameter]
    public ChatMessage? InProgressMessage { get; set; }

    [Parameter]
    public RenderFragment? NoMessagesContent { get; set; }

    private bool IsEmpty => !Messages.Any(m => (m.Role == ChatRole.User || m.Role == ChatRole.Assistant) && !string.IsNullOrEmpty(m.Text));

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS!.InvokeVoidAsync("import", "./Components/Pages/Chat/ChatMessageList.razor.js");
        }
    }
}