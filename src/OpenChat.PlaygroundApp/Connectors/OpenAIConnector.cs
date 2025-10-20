using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for OpenAI.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class OpenAIConnector(AppSettings settings) : LanguageModelConnector(settings.OpenAI)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not OpenAISettings settings)
        {
            throw new InvalidOperationException("Missing configuration: OpenAI.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: OpenAI:ApiKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: OpenAI:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as OpenAISettings;

        var model = settings!.Model!.Trim() ?? throw new InvalidOperationException("Missing configuration: OpenAI:Model.");
        var apiKey = settings!.ApiKey!.Trim() ?? throw new InvalidOperationException("Missing configuration: OpenAI:ApiKey.");

        var credential = new ApiKeyCredential(apiKey);

        var client = new OpenAIClient(credential);
        var chatClient = client.GetChatClient(model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}