using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

// Temporary workaround for OllamaSharp IChatClient integration

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for LG models through OllamaSharp.
/// </summary>
public class LGConnector(AppSettings settings) : LanguageModelConnector(settings.LG)
{
    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as LGSettings;

        var baseUrl = settings?.BaseUrl ?? throw new InvalidOperationException("Missing configuration: LG:BaseUrl.");
        var model = settings?.Model ?? throw new InvalidOperationException("Missing configuration: LG:Model.");

        var uri = new Uri(baseUrl);
        
        // Use OllamaSharp official way: OllamaApiClient directly implements IChatClient
        IChatClient chatClient = new OllamaApiClient(uri, model);
        
        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
