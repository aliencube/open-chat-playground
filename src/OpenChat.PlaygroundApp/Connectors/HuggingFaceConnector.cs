using Microsoft.Extensions.AI;

using OllamaSharp;
using System.Text.RegularExpressions;

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

        if (string.IsNullOrWhiteSpace(settings?.BaseUrl) == true)
        {
            throw new InvalidOperationException("Missing configuration: HuggingFace:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model) == true)
        {
            throw new InvalidOperationException("Missing configuration: HuggingFace:Model.");
        }

        // Accepts formats like:
        // - hf.co/{org}/{model}-gguf
        if (IsValidHuggingFaceModel(settings.Model) == false)
        {
            throw new InvalidOperationException("Invalid configuration: HuggingFace:Model format. Expected 'hf.co/{org}/{model}-gguf' format.");
        }

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(settings.BaseUrl),
            Model = settings.Model,
        };

        var chatClient = new OllamaApiClient(config);

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }

    bool IsValidHuggingFaceModel(string input)
    {
        var segments = input.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length != 3)
        {
            return false;
        }

        if (segments[0].Equals("hf.co", StringComparison.OrdinalIgnoreCase) == false)
        {
            return false;
        }

        if (segments[^1].EndsWith("-gguf", StringComparison.OrdinalIgnoreCase) == false)
        {
            return false;
        }

        return true;
    }
}