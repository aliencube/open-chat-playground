using Microsoft.AspNetCore.Components;

using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatHeader : ComponentBase
{
    [Parameter]
    public EventCallback OnNewChat { get; set; }

    [Inject]
    public required AppSettings Settings { get; set; }
}
