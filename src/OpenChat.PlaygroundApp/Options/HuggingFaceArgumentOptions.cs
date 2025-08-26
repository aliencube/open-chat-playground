using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Hugging Face.
/// </summary>
public class HuggingFaceArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for the Hugging Face API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for Hugging Face.
    /// </summary>
    public string? Model { get; set; }

}
