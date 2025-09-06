using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for OpenAI.
/// </summary>
public class OpenAIConnector(AppSettings settings) : LanguageModelConnector(ValidateAndGetSettings(settings))
{
    private static OpenAISettings ValidateAndGetSettings(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        
        if (settings.OpenAI is null)
        {
            throw new InvalidOperationException("Missing configuration: OpenAI settings are required.");
        }
        
        if (string.IsNullOrWhiteSpace(settings.OpenAI.ApiKey))
        {
            throw new InvalidOperationException("Missing configuration: OpenAI:ApiKey is required.");
        }
        
        if (string.IsNullOrWhiteSpace(settings.OpenAI.Model))
        {
            throw new InvalidOperationException("Missing configuration: OpenAI:Model is required.");
        }
        
        return settings.OpenAI;
    }
    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = (OpenAISettings)this.Settings!;

        var credential = new ApiKeyCredential(settings.ApiKey!);
        var client = new OpenAIClient(credential);
        var chatClient = client.GetChatClient(settings.Model)
                               .AsIChatClient();

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}