using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace OpenChat.PlaygroundApp.Components;

public partial class App
{
    protected readonly IComponentRenderMode renderMode = new InteractiveServerRenderMode(prerender: false);
}

