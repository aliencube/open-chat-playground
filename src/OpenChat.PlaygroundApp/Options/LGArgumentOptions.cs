using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// Represents the command-line argument options for LG AI EXAONE.
/// </summary>
public class LGArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for LG AI EXAONE API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for LG AI EXAONE.
    /// </summary>
    public string? Model { get; set; }
}