using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="DockerModelRunnerSettings"/> instance.
    /// </summary>
    public DockerModelRunnerSettings? DockerModelRunner { get; set; }
}

/// <summary>
/// This represents the app settings entity for Docker Model Runner.
/// </summary>
public class DockerModelRunnerSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the base URL of the Docker Model Runner API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name of Docker Model Runner.
    /// </summary>
    public string? Model { get; set; }
}