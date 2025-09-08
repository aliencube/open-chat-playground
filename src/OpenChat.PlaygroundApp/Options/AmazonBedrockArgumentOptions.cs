using OpenChat.PlaygroundApp.Abstractions;

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
}
