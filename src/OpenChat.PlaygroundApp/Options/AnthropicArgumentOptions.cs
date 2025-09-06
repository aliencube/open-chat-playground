using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Anthropic Claude.
/// </summary>
public class AnthropicArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the API key for Anthropic Claude.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Anthropic Claude.
    /// </summary>
    public string? Model { get; set; }

    /// <inheritdoc/>
}