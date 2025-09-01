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
}