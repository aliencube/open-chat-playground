using Microsoft.Extensions.AI;

using OllamaSharp;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Docker Model Runner.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class DockerModelRunnerConnector(AppSettings settings) : LanguageModelConnector(settings.DockerModelRunner)
{
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
        var baseUrl = settings!.BaseUrl!;
        var model = settings!.Model!;

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(baseUrl),
            Model = model,
        };

        var chatClient = new OllamaApiClient(config);

        var pulls = chatClient.PullModelAsync(model);
        await foreach (var pull in pulls)
        {
            Console.WriteLine($"Pull status: {pull!.Status}");
        }

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
