using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for GitHub Models.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class GitHubModelsConnector(AppSettings settings) : LanguageModelConnector(settings.GitHubModels)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not GitHubModelsSettings settings)
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

        var credential = new ApiKeyCredential(settings?.Token ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(settings.Endpoint ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Endpoint."))
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(settings.Model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}