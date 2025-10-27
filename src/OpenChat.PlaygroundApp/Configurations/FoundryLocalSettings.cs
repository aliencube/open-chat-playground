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
    /// Gets or sets the alias of FoundryLocal.
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    /// Gets or sets the Endpoint of FoundryLocal.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the automatic FoundryLocal manager and use a manually configured endpoint.
    /// </summary>
    public bool DisableFoundryLocalManager { get; set; }
}