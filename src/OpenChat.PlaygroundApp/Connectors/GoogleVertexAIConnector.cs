using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Google Vertex AI.
/// </summary>
public class GoogleVertexAIConnector(AppSettings settings) : LanguageModelConnector(settings.GoogleVertexAI)
{
    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as GoogleVertexAISettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:ApiKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as GoogleVertexAISettings;

        // TODO: Replace with actual Google Vertex AI client implementation
        // var credential = new ApiKeyCredential(settings?.ApiKey ?? throw new InvalidOperationException("Missing configuration: GoogleVertexAI:ApiKey."));
        // var client = new GoogleVertexAIClient(credential);
        // var chatClient = client.GetChatClient(settings.Model).AsIChatClient();
        // return await Task.FromResult(chatClient).ConfigureAwait(false);

        throw new NotImplementedException("Google Vertex AI client integration is not implemented yet.");
    }
}
