using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

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

	/// <inheritdoc/>
	protected override void ParseOptions(IConfiguration config, string[] args)
	{
		var settings = new AppSettings();
		config.Bind(settings);

		var dockerModelRunner = settings.DockerModelRunner;

		this.BaseUrl ??= dockerModelRunner?.BaseUrl;
		this.Model ??= dockerModelRunner?.Model;

		for (var i = 0; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "--base-url":
					if (i + 1 < args.Length)
					{
						this.BaseUrl = args[++i];
					}
					break;

				case "--model":
					if (i + 1 < args.Length)
					{
						this.Model = args[++i];
					}
					break;

				default:
					break;
			}
		}
	}
}