using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;


/// <summary>
/// Represents the command-line argument options for Ollama.
/// </summary>
public class OllamaArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for Ollama API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for Ollama.
    /// </summary>
    public string? Model { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var ollama = settings.Ollama;

        this.BaseUrl ??= ollama?.BaseUrl;
        this.Model ??= ollama?.Model;

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
