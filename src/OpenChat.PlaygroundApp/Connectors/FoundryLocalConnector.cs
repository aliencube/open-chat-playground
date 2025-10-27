using System.ClientModel;

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Foundry Local.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class FoundryLocalConnector(AppSettings settings) : LanguageModelConnector(settings.FoundryLocal)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    private const string ApiKey = "OPENAI_API_KEY";

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not FoundryLocalSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal.");
        }

        if (string.IsNullOrWhiteSpace(settings.Alias!.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal:Alias.");
        }

        if (settings.DisableFoundryLocalManager == true && string.IsNullOrWhiteSpace(settings.Endpoint!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal:Endpoint is required when DisableFoundryLocalManager is enabled.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as FoundryLocalSettings;

        Uri endpoint;
        string modelId;

        if (settings!.DisableFoundryLocalManager == true)
        {
            modelId = settings.Alias!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:Alias.");
            var settingsEndpoint = settings.Endpoint!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:Endpoint.");
            if (Uri.IsWellFormedUriString(settingsEndpoint, UriKind.Absolute) == false)
            {
                throw new UriFormatException($"Invalid URI: The Foundry Local endpoint '{settingsEndpoint}' is not a valid URI.");
            }
            endpoint = new Uri(settingsEndpoint);
        }
        else
        {
            var alias = settings!.Alias!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:Alias.");
            var manager = await FoundryLocalManager.StartModelAsync(aliasOrModelId: alias).ConfigureAwait(false);
            var model = await manager.GetModelInfoAsync(aliasOrModelId: alias).ConfigureAwait(false);

            endpoint = manager.Endpoint;
            modelId = model?.ModelId ?? alias;
        }

        var credential = new ApiKeyCredential(ApiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = endpoint,
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(modelId)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {modelId}");

        return chatClient;
    }
}