using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="OllamaSettings"/> instance.
    /// </summary>
    public OllamaSettings? Ollama { get; set; }
}

/// <summary>
/// This represents the app settings entity for Ollama.
/// </summary>
public class OllamaSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the base URL of the Ollama API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for Ollama.
    /// </summary>
    public string? Model { get; set; }
}
