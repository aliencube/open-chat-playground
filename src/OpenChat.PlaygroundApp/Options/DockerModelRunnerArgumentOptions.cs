using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Docker Model Runner.
/// </summary>
public class DockerModelRunnerArgumentOptions : ArgumentOptions
{
	/// <summary>
	/// Gets or sets the Docker Model Runner Base URL.
	/// </summary>
	public string? BaseUrl { get; set; }

	/// <summary>
	/// Gets or sets the Docker Model Runner model/deployment name.
	/// </summary>
	public string? Model { get; set; }
}