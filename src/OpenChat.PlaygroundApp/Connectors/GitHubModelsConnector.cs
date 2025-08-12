using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

public class GitHubModelsConnector(AppSettings settings) : LanguageModelConnector(settings.GitHubModels)
{
    /// <inheritdoc/>
    public override IChatClient GetChatClient()
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

        return chatClient;
    }
}