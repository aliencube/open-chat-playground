using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Abstractions;

using Azure;
using Azure.AI.OpenAI;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Azure AI Foundry.
/// </summary>
public class AzureAIFoundryConnector(AppSettings settings) : LanguageModelConnector(settings.AzureAIFoundry)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not AzureAIFoundrySettings settings)
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry.");
        }

        if (string.IsNullOrWhiteSpace(settings.Endpoint?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:Endpoint.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:ApiKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.DeploymentName?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AzureAIFoundry:DeploymentName.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as AzureAIFoundrySettings;

        var endpoint = settings!.Endpoint!.Trim() ?? throw new InvalidOperationException("Missing configuration: AzureAIFoundry:Endpoint.");
        var deploymentName = settings.DeploymentName!.Trim() ?? throw new InvalidOperationException("Missing configuration: AzureAIFoundry:DeploymentName.");
        var apiKey = settings.ApiKey!.Trim() ?? throw new InvalidOperationException("Missing configuration: AzureAIFoundry:ApiKey.");

        var credential = new AzureKeyCredential(apiKey); 
        var azureClient = new AzureOpenAIClient(new Uri(endpoint), credential);
        
        var chatClient = azureClient.GetChatClient(deploymentName) 
                                    .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {deploymentName}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}