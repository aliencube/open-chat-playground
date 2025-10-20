using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Upstage.
/// </summary>
public class UpstageConnector(AppSettings settings) : LanguageModelConnector(settings.Upstage)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as UpstageSettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: Upstage.");
        }

        if (string.IsNullOrWhiteSpace(settings.BaseUrl?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Upstage:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Upstage:ApiKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Upstage:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as UpstageSettings;

        var baseUrl = settings!.BaseUrl!.Trim() ?? throw new InvalidOperationException("Missing configuration: Upstage:BaseUrl.");
        var model = settings!.Model!.Trim() ?? throw new InvalidOperationException("Missing configuration: Upstage:Model.");
        var apiKey = settings!.ApiKey!.Trim() ?? throw new InvalidOperationException("Missing configuration: Upstage:ApiKey.");

        var credential = new ApiKeyCredential(apiKey);

        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(baseUrl),
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
