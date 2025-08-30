using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Upstage.
/// </summary>
public class UpstageArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for Upstage API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the API key for Upstage.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Upstage.
    /// </summary>
    public string? Model { get; set; }
}
