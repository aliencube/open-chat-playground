using System.ClientModel;

using Microsoft.Extensions.AI;

using Mscc.GenerativeAI.Microsoft;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Google Vertex AI.
/// </summary>
public class GoogleVertexAIConnector(AppSettings settings) : LanguageModelConnector(settings.GoogleVertexAI)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not GoogleVertexAISettings settings)
        {
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:ApiKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: GoogleVertexAI:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as GoogleVertexAISettings;

        var apiKey = settings!.ApiKey!.Trim() ?? throw new InvalidOperationException("Missing configuration: GoogleVertexAI:ApiKey.");
        var model = settings!.Model!.Trim() ?? throw new InvalidOperationException("Missing configuration: GoogleVertexAI:Model.");

        var chatClient = new GeminiChatClient(apiKey, model);

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
