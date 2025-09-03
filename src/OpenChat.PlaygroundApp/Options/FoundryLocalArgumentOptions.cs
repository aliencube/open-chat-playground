using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Foundry Local.
/// </summary>
public class FoundryLocalArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the alias of Foundry Local.
    /// </summary>
    public string? Alias { get; set; }
}