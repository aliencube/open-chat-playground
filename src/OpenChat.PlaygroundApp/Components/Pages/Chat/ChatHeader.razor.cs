using Microsoft.AspNetCore.Components;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatHeader : ComponentBase
{
    [Parameter]
    public EventCallback OnNewChat { get; set; }
}
