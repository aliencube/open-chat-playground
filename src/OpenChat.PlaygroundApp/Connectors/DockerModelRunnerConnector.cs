using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Docker Model Runner.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class DockerModelRunnerConnector(AppSettings settings) : LanguageModelConnector(settings.DockerModelRunner)
{
    private const string DockerModelRunnerPrefix = "ai";
    private const string DockerModelRunnerApiKey = "docker-model-runner-api-key";

    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not DockerModelRunnerSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner.");
        }

        if (string.IsNullOrWhiteSpace(settings.BaseUrl!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:Model.");
        }

        // Accepts formats like:
        // - ai/{model} e.g ai/gemma3
        if (IsValidModel(settings.Model!.Trim()) == false)
        {
            throw new InvalidOperationException("Invalid configuration: DockerModelRunner:Model format. Expected 'ai/{model}' format.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as DockerModelRunnerSettings;
        var baseUrl = settings!.BaseUrl!;
        var model = settings!.Model!;


        var credential = new ApiKeyCredential(DockerModelRunnerApiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(baseUrl ?? throw new InvalidOperationException("Missing configuration: DockerModelRunner:BaseUrl."))
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }

    private static bool IsValidModel(string model)
    {
        var segments = model.Split([ '/' ], StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length != 2)
        {
            return false;
        }

        if (segments.First().Equals(DockerModelRunnerPrefix, StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        return true;
    }
}