using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Upstage.
/// </summary>
public class UpstageArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for Upstage API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the API key for Upstage.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Upstage.
    /// </summary>
    public string? Model { get; set; }

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var upstage = settings.Upstage;

        // Priority 1: appsettings.json (lowest priority)
        this.BaseUrl = upstage?.BaseUrl;
        this.ApiKey = upstage?.ApiKey;
        this.Model = upstage?.Model;

        // Priority 2: Environment variables (middle priority)
        this.BaseUrl = Environment.GetEnvironmentVariable("UPSTAGE_BASE_URL") ?? this.BaseUrl;
        this.ApiKey = Environment.GetEnvironmentVariable("UPSTAGE_API_KEY") ?? this.ApiKey;
        this.Model = Environment.GetEnvironmentVariable("UPSTAGE_MODEL") ?? this.Model;

        // Priority 3: Command-line arguments (highest priority)
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
