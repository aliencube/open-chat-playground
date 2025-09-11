using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for OpenAI.
/// </summary>
public class OpenAIConnector(AppSettings settings) : LanguageModelConnector(settings.OpenAI)
{
    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as OpenAISettings;
        if (settings is null)
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

        var credential = new ApiKeyCredential(settings?.ApiKey ?? throw new InvalidOperationException("Missing configuration: OpenAI:ApiKey."));

        var client = new OpenAIClient(credential);
        var chatClient = client.GetChatClient(settings.Model)
                               .AsIChatClient();

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}