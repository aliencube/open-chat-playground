using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Docker Model Runner.
/// </summary>
public class DockerModelRunnerConnector(AppSettings settings) : LanguageModelConnector(settings.DockerModelRunner)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as DockerModelRunnerSettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner.");
        }

        if (string.IsNullOrWhiteSpace(settings.BaseUrl?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as DockerModelRunnerSettings;

        var credential = new ApiKeyCredential("not-used");
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(settings?.BaseUrl ?? throw new InvalidOperationException("Missing configuration: DockerModelRunner:BaseUrl."))
        };
        
        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(settings.Model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
