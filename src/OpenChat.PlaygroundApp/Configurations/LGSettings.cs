using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="LGSettings"/> instance.
    /// </summary>
    public LGSettings? LG { get; set; }
}

/// <summary>
/// This represents the app settings entity for LG AI EXAONE.
/// </summary>
public class LGSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the base URL of the LG AI EXAONE API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for LG AI EXAONE.
    /// </summary>
    public string? Model { get; set; }
}
