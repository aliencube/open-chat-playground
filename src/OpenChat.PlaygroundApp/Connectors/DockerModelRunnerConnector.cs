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
    private const string DockerModelRunnerApiKey = "docker-model-runner-api-key";

    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not DockerModelRunnerSettings settings)
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

        var baseUrl = settings!.BaseUrl!.Trim() ?? throw new InvalidOperationException("Missing configuration: DockerModelRunner:BaseUrl.");
        if (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute) == false)
        {
            throw new UriFormatException($"Invalid URI: The Docker Model Runner base URL '{baseUrl}' is not a valid URI.");
        }
        var model = settings!.Model!.Trim() ?? throw new InvalidOperationException("Missing configuration: DockerModelRunner:Model.");

        var credential = new ApiKeyCredential(DockerModelRunnerApiKey);
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri($"{baseUrl.TrimEnd('/')}/engines/v1")
        };
        
        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
