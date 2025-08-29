using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="OpenAISettings"/> instance.
    /// </summary>
    public OpenAISettings? OpenAI { get; set; }
}

/// <summary>
/// This represents the app settings entity for OpenAI.
/// </summary>
public class OpenAISettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the OpenAI API key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name for OpenAI.
    /// </summary>
    public string? Model { get; set; }
}
