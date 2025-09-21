using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="AmazonBedrockSettings"/> instance.
    /// </summary>
    public AmazonBedrockSettings? AmazonBedrock { get; set; }
}

/// <summary>
/// This represents the app settings entity for Amazon Bedrock.
/// </summary>
public class AmazonBedrockSettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the AWSCredentials Access Key ID for the Amazon Bedrock service.
    /// </summary>
    public string? AccessKeyId { get; set; }

    /// <summary>
    /// Gets or sets the AWSCredentials Secret Access Key for the Amazon Bedrock service.
    /// </summary>
    public string? SecretAccessKey { get; set; }

    /// <summary>
    /// Gets or sets the AWS region for the Amazon Bedrock service.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the model ID for the Amazon Bedrock service.
    /// </summary>
    public string? ModelId { get; set; }
}