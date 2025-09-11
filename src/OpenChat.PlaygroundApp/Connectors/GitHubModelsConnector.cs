using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for GitHub Models.
/// </summary>
public class GitHubModelsConnector(AppSettings settings) : LanguageModelConnector(settings.GitHubModels)
{
    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as GitHubModelsSettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: GitHubModels.");
        }

        if (string.IsNullOrWhiteSpace(settings.Endpoint!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: GitHubModels:Endpoint.");
        }

        if (string.IsNullOrWhiteSpace(settings.Token!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: GitHubModels:Token.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: GitHubModels:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as GitHubModelsSettings;

        var credential = new ApiKeyCredential(settings?.Token!);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(settings.Endpoint!)
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(settings.Model)
                               .AsIChatClient();

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}