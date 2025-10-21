using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatMessageItem
{
    private static readonly ConditionalWeakTable<ChatMessage, ChatMessageItem> SubscribersLookup = new();

    [Parameter, EditorRequired]
    public required ChatMessage Message { get; set; }

    [Parameter]
    public bool InProgress { get; set; }

    protected override void OnInitialized()
    {
        SubscribersLookup.AddOrUpdate(Message, this);
    }

    public static void NotifyChanged(ChatMessage source)
    {
        if (SubscribersLookup.TryGetValue(source, out var subscriber))
        {
            subscriber.StateHasChanged();
        }
    }
}

