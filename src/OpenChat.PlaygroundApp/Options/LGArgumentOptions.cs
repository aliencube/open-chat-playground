using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// Represents the command-line argument options for LG AI EXAONE.
/// </summary>
public class LGArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the base URL for LG AI EXAONE API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the model name for LG AI EXAONE.
    /// </summary>
    public string? Model { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var lg = settings.LG;

        this.BaseUrl ??= lg?.BaseUrl;
        this.Model ??= lg?.Model;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case var _ when args[i] == CommandLineArguments.LG.BaseUrl:
                    if (i + 1 < args.Length)
                    {
                        this.BaseUrl = args[++i];
                    }
                    break;

                case var _ when args[i] == CommandLineArguments.LG.Model:
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