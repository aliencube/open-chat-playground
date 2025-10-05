using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Amazon Bedrock.
/// </summary>
public class AmazonBedrockArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the AWSCredentials Access Key ID for the Amazon Bedrock service.
    /// </summary>
    public string? AccessKeyId { get; set; }

    /// <summary>
    ///  Gets or sets the AWSCredentials Secret Access Key for the Amazon Bedrock service.
    /// </summary>
    public string? SecretAccessKey { get; set; }

    /// <summary>
    ///  Gets or sets the AWS region for the Amazon Bedrock service.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    ///  Gets or sets the model for the Amazon Bedrock service.
    /// </summary>
    public string? ModelId { get; set; }

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var amazonBedrock = settings.AmazonBedrock;

        this.AccessKeyId ??= amazonBedrock?.AccessKeyId;
        this.SecretAccessKey ??= amazonBedrock?.SecretAccessKey;
        this.Region ??= amazonBedrock?.Region;
        this.ModelId ??= amazonBedrock?.ModelId;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case var _ when args[i] == ArgumentOptionConstants.AmazonBedrock.AccessKeyId:
                    if (i + 1 < args.Length)
                    {
                        this.AccessKeyId = args[++i];
                    }
                    break;

                case var _ when args[i] == ArgumentOptionConstants.AmazonBedrock.SecretAccessKey:
                    if (i + 1 < args.Length)
                    {
                        this.SecretAccessKey = args[++i];
                    }
                    break;

                case var _ when args[i] == ArgumentOptionConstants.AmazonBedrock.Region:
                    if (i + 1 < args.Length)
                    {
                        this.Region = args[++i];
                    }
                    break;

                case var _ when args[i] == ArgumentOptionConstants.AmazonBedrock.ModelId:
                    if (i + 1 < args.Length)
                    {
                        this.ModelId = args[++i];
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
