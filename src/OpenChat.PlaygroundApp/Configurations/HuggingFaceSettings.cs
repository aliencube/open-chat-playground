using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="HuggingFaceSettings"/> instance.
    /// </summary>
    public HuggingFaceSettings? HuggingFace { get; set; }
}

/// <summary>
/// This represents the app settings entity for Hugging Face.
/// </summary>
public class HuggingFaceSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the endpoint URL of Hugging Face API.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the Hugging Face API token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the model name of Hugging Face.
    /// </summary>
    public string? Model { get; set; }
}