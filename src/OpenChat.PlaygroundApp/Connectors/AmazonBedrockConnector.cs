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

        if (string.IsNullOrWhiteSpace(settings.AccessKeyId?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:AccessKeyId.");
        }

        if (string.IsNullOrWhiteSpace(settings.SecretAccessKey?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:SecretAccessKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.Region?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:Region.");
        }

        if (string.IsNullOrWhiteSpace(settings.ModelId?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:ModelId.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as AmazonBedrockSettings;

        var accessKeyId = settings!.AccessKeyId!;
        var secretAccessKey = settings!.SecretAccessKey!;
        var region = settings!.Region!;
        var modelId = settings!.ModelId!;

        var regionEndpoint = RegionEndpoint.GetBySystemName(region);
        var bedrockClient = new AmazonBedrockRuntimeClient(accessKeyId, secretAccessKey, regionEndpoint);

        var chatClient = bedrockClient.AsIChatClient(modelId);

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {modelId}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}