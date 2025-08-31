using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="GitHubModelsSettings"/> instance.
    /// </summary>
    public GitHubModelsSettings? GitHubModels { get; set; }
}

/// <summary>
/// This represents the app settings entity for GitHub Models.
/// </summary>
public class GitHubModelsSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the endpoint URL of GitHub Models API.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the GitHub Personal Access Token (PAT).
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the model name of GitHub Models.
    /// </summary>
    public string? Model { get; set; }
}