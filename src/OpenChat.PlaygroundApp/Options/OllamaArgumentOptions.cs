namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// Represents the command-line argument options for Ollama.
/// </summary>
public class OllamaArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for Ollama API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for Ollama.
    /// </summary>
    public string? Model { get; set; }
}
