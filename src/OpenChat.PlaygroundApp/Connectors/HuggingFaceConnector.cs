using Microsoft.Extensions.AI;

using OllamaSharp;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Hugging Face.
/// </summary>
public class HuggingFaceConnector(AppSettings settings) : LanguageModelConnector(settings.HuggingFace)
{
    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as HuggingFaceSettings;

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(settings?.BaseUrl ?? throw new InvalidOperationException("Missing configuration: HuggingFace:BaseUrl.")),
            Model = settings.Model ?? throw new InvalidOperationException("Missing configuration: HuggingFace:Model."),
        };

        var chatClient = new OllamaApiClient(config);

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}