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
    /// Gets or sets the base URL of the Hugging Face API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for Hugging Face.
    /// </summary>
    public string? Model { get; set; }
}