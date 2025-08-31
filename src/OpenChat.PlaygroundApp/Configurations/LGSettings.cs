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
/// This represents the app settings entity for LG models.
/// </summary>
public class LGSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the base URL of the Ollama API for LG models.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the LG model name.
    /// </summary>
    public string? Model { get; set; }
}
