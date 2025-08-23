using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="AnthropicSettings"/> instance.
    /// </summary>
    public AnthropicSettings? Anthropic { get; set; }
}

/// <summary>
/// This represents the app settings entity for Anthropic Claude.
/// </summary>
public class AnthropicSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the Anthropic API key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name. Defaults to 'claude-3-5-sonnet-latest' if not specified.
    /// </summary>
    public string? Model { get; set; } = "claude-3-5-sonnet-latest";
}
