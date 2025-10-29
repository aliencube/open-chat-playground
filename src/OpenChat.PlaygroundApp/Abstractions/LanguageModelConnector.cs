using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Abstractions;

/// <summary>
/// This represents the base language model connector entity for all language model connectors to inherit.
/// </summary>
public abstract class LanguageModelConnector(LanguageModelSettings? settings)
{
    /// <summary>
    /// Gets the <see cref="LanguageModelSettings"/> instance.
    /// </summary>
    protected LanguageModelSettings? Settings { get; } = settings;

    /// <summary>
    /// Ensures that the language model settings are valid or not.
    /// </summary>
    /// <returns>Returns <c>True</c> if the settings are valid; otherwise, throws <see cref="InvalidOperationException"/>.</returns>
    public abstract bool EnsureLanguageModelSettingsValid();

    /// <summary>
    /// Gets an <see cref="IChatClient"/> instance.
    /// </summary>
    /// <returns>Returns <see cref="IChatClient"/> instance.</returns>
    public abstract Task<IChatClient> GetChatClientAsync();

    /// <summary>
    /// Gets an <see cref="IChatClient"/> instance based on the app settings provided.
    /// </summary>
    /// <param name="settings"><see cref="AppSettings"/> instance.</param>
    /// <returns>Returns <see cref="IChatClient"/> instance.</returns>
    public static async Task<IChatClient> CreateChatClientAsync(AppSettings settings)
    {
        LanguageModelConnector connector = settings.ConnectorType switch
        {
            ConnectorType.AmazonBedrock => new AmazonBedrockConnector(settings),
            ConnectorType.AzureAIFoundry => new AzureAIFoundryConnector(settings),
            ConnectorType.GitHubModels => new GitHubModelsConnector(settings),
            ConnectorType.GoogleVertexAI => new GoogleVertexAIConnector(settings),
            ConnectorType.DockerModelRunner => new DockerModelRunnerConnector(settings),
            ConnectorType.FoundryLocal => new FoundryLocalConnector(settings),
            ConnectorType.HuggingFace => new HuggingFaceConnector(settings),
            ConnectorType.Ollama => new OllamaConnector(settings),
            ConnectorType.Anthropic => new AnthropicConnector(settings),
            ConnectorType.LG => new LGConnector(settings),
            ConnectorType.OpenAI => new OpenAIConnector(settings),
            ConnectorType.Upstage => new UpstageConnector(settings),
            _ => throw new NotSupportedException($"Connector type '{settings.ConnectorType}' is not supported.")
        };

        connector.EnsureLanguageModelSettingsValid();

        return await connector.GetChatClientAsync().ConfigureAwait(false);
    }
}
