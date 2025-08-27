using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="UpstageSolarSettings"/> instance.
    /// </summary>
    public UpstageSolarSettings? Upstage { get; set; }
}

/// <summary>
/// This represents the app settings entity for Upstage Solar.
/// </summary>
public class UpstageSolarSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the base URL of Upstage Solar API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the Upstage API key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Upstage Solar.
    /// </summary>
    public string? Model { get; set; }
}
