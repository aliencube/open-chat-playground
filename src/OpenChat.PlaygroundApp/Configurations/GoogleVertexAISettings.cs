using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="GoogleVertexAISettings"/> instance.
    /// </summary>
    public GoogleVertexAISettings? GoogleVertexAI { get; set; }
}

/// <summary>
/// This represents the app settings entity for Google Vertex AI.
/// </summary>
public class GoogleVertexAISettings : LanguageModelSettings
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