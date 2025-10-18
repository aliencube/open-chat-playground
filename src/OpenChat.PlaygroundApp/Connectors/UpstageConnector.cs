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

        var credential = new ApiKeyCredential(settings?.ApiKey ?? 
            throw new InvalidOperationException("Missing configuration: Upstage:ApiKey."));

        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(settings.BaseUrl ?? 
                throw new InvalidOperationException("Missing configuration: Upstage:BaseUrl."))
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(settings.Model)
                               .AsIChatClient();

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
