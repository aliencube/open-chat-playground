using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Amazon Bedrock.
/// </summary>
public class AmazonBedrockArgumentOptions : ArgumentOptions
{
    /// <summary>
    ///  Gets or sets the AWS region for the Amazon Bedrock service.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    ///  Gets or sets the model for the Amazon Bedrock service.
    /// </summary>
    public string? Model { get; set; }

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var amazonBedrock = settings.AmazonBedrock;

        this.Region ??= amazonBedrock?.Region;
        this.Model ??= amazonBedrock?.Model;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--region":
                    if (i + 1 < args.Length)
                    {
                        this.Region = args[++i];
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
