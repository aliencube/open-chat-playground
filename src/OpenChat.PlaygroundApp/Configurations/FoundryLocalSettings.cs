using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="FoundryLocalSettings"/> instance.
    /// </summary>
    public FoundryLocalSettings? FoundryLocal { get; set; }
}

/// <summary>
/// This represents the app settings entity for FoundryLocal.
/// </summary>
public class FoundryLocalSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the Base URL of Foundry Local. If `DisableFoundryLocalManager` is set, this value must be provided.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets either alias or model ID of Foundry Local.
    /// </summary>
    public string? AliasOrModel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the automatic Foundry Local manager and use a manually configured endpoint.
    /// </summary>
    public bool DisableFoundryLocalManager { get; set; }
}