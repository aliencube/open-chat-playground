using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatMessageList
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    [Parameter]
    public required IEnumerable<ChatMessage> Messages { get; set; }

    [Parameter]
    public ChatMessage? InProgressMessage { get; set; }

    [Parameter]
    public RenderFragment? NoMessagesContent { get; set; }

    protected bool IsEmpty => !Messages.Any(m => (m.Role == ChatRole.User || m.Role == ChatRole.Assistant) && !string.IsNullOrEmpty(m.Text));

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Activates the auto-scrolling behavior
            await JS.InvokeVoidAsync("import", "./Components/Pages/Chat/ChatMessageList.razor.js");
        }
    }
}

