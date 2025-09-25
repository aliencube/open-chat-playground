using Microsoft.Extensions.AI;
using OllamaSharp;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for LG AI EXAONE.
/// </summary>
public class LGConnector(AppSettings settings) : LanguageModelConnector(settings.LG)
{
    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as LGSettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: LG.");
        }

        if (string.IsNullOrWhiteSpace(settings.BaseUrl!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: LG:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: LG:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as LGSettings;
        var baseUrl = settings!.BaseUrl!;
        var model = settings!.Model!;

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(baseUrl),
            Model = model,
        };

        var chatClient = new OllamaApiClient(config);

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
