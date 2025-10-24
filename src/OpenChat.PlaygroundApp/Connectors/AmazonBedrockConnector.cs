using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Amazon;
using Amazon.BedrockRuntime;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Amazon Bedrock.
/// </summary>
public class AmazonBedrockConnector(AppSettings settings) : LanguageModelConnector(settings.AmazonBedrock)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not AmazonBedrockSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock.");
        }

        if (string.IsNullOrWhiteSpace(settings.AccessKeyId?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:AccessKeyId.");
        }

        if (string.IsNullOrWhiteSpace(settings.SecretAccessKey?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:SecretAccessKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.Region?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:Region.");
        }

        if (string.IsNullOrWhiteSpace(settings.ModelId?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:ModelId.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        this.EnsureLanguageModelSettingsValid();
        
        var settings = this.Settings as AmazonBedrockSettings;

        var accessKeyId = settings!.AccessKeyId!.Trim() ?? throw new InvalidOperationException("Missing configuration: AmazonBedrock:AccessKeyId.");
        var secretAccessKey = settings!.SecretAccessKey!.Trim() ?? throw new InvalidOperationException("Missing configuration: AmazonBedrock:SecretAccessKey.");
        var region = settings!.Region!.Trim() ?? throw new InvalidOperationException("Missing configuration: AmazonBedrock:Region.");
        var modelId = settings!.ModelId!.Trim() ?? throw new InvalidOperationException("Missing configuration: AmazonBedrock:ModelId.");

        var endpoint = RegionEndpoint.GetBySystemName(region);
        var client = new AmazonBedrockRuntimeClient(accessKeyId, secretAccessKey, endpoint);

        var chatClient = client.AsIChatClient(modelId);

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {modelId}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}