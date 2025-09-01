using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Google Vertex AI.
/// </summary>
public class GoogleVertexAIArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the Google Vertex AI API Key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Google Vertex AI.
    /// </summary>
    public string? Model { get; set; }
}