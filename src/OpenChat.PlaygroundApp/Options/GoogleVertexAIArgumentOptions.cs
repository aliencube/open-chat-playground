using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Google Vertex AI.
/// </summary>
public class GoogleVertexAIArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the Google Vertex AI API Key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Google Vertex AI.
    /// </summary>
    public string? Model { get; set; }


    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var googleVertexAI = settings.GoogleVertexAI;

        this.ApiKey ??= googleVertexAI?.ApiKey;
        this.Model ??= googleVertexAI?.Model;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--api-key":
                    if (i + 1 < args.Length)
                    {
                        this.ApiKey = args[++i];
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