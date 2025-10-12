using System.ClientModel;

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Foundry Local.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class FoundryLocalConnector(AppSettings settings) : LanguageModelConnector(settings.FoundryLocal)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not FoundryLocalSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal.");
        }

        if (string.IsNullOrWhiteSpace(settings.Alias?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal:Alias.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as FoundryLocalSettings;
        var alias = settings!.Alias!;

        var manager = await FoundryLocalManager.StartModelAsync(aliasOrModelId: alias);
        var model = await manager.GetModelInfoAsync(aliasOrModelId: alias);

        var credential = new ApiKeyCredential(manager.ApiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = manager.Endpoint,
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(model?.ModelId)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Alias}");

        return chatClient;
    }
}