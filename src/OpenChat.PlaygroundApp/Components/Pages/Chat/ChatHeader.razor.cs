using Microsoft.AspNetCore.Components;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatHeader
{
    [Parameter]
    public EventCallback OnNewChat { get; set; }
}

