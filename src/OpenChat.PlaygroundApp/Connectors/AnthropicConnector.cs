using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Anthropic Claude.
/// </summary>
public class AnthropicConnector(AppSettings settings) : LanguageModelConnector(settings.Anthropic)
{
    /// <inheritdoc/>
    public override Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as AnthropicSettings;

        // Validate required settings before attempting to create client
        if (string.IsNullOrWhiteSpace(settings?.ApiKey))
        {
            throw new InvalidOperationException("Missing configuration: Anthropic:ApiKey.");
        }

        // Model is optional, will use default value from AnthropicSettings if not specified
        var model = string.IsNullOrWhiteSpace(settings.Model) ? "claude-3-5-sonnet-latest" : settings.Model;

        // For now, we'll throw a NotImplementedException as we need to add the Anthropic SDK
        // This will be implemented once the appropriate NuGet package is added
        throw new NotImplementedException("Anthropic connector implementation is pending the addition of Anthropic SDK package.");
    }
}
