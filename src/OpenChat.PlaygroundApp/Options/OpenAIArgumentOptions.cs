using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for OpenAI.
/// </summary>
public class OpenAIArgumentOptions : ArgumentOptions
{
	/// <summary>
	/// Gets or sets the OpenAI API key.
	/// </summary>
	public string? ApiKey { get; set; }

	/// <summary>
	/// Gets or sets the OpenAI model name.
	/// </summary>
	public string? Model { get; set; }
}
