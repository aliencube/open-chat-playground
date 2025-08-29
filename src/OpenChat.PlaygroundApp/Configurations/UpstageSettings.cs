using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="UpstageSettings"/> instance.
    /// </summary>
    public UpstageSettings? Upstage { get; set; }
}

/// <summary>
/// This represents the app settings entity for Upstage.
/// </summary>
public class UpstageSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the base URL of Upstage API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the Upstage API key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Upstage.
    /// </summary>
    public string? Model { get; set; }
}
