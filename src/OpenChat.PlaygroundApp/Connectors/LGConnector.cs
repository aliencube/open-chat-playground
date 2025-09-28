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

        // Validate LG model format: should be HuggingFace format, LG model, or GGUF format
        var model = settings.Model.Trim();
        var isHuggingFaceFormat = model.StartsWith("hf.co/") || model.Contains("/");
        var isLGModel = model.ToLowerInvariant().Contains("exaone") || model.ToLowerInvariant().Contains("lg");
        var isGGUFFormat = model.ToLowerInvariant().EndsWith(".gguf") || model.ToLowerInvariant().Contains("gguf");

        if (!isHuggingFaceFormat && !isLGModel && !isGGUFFormat)
        {
            throw new InvalidOperationException("Invalid LG model format. Model should be HuggingFace format (hf.co/...), LG model (containing 'exaone' or 'lg'), or GGUF format (containing 'gguf').");
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
