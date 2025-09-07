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
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as HuggingFaceSettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: HuggingFace.");
        }

        if (string.IsNullOrWhiteSpace(settings.BaseUrl!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: HuggingFace:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: HuggingFace:Model.");
        }

        // Accepts formats like:
        // - hf.co/{org}/{model}-gguf
        if (IsValidHuggingFaceModelFormat(settings.Model) == false)
        {
            throw new InvalidOperationException("Invalid configuration: HuggingFace:Model format. Expected 'hf.co/{org}/{model}-gguf' format.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as HuggingFaceSettings;

        var baseUrl = settings?.BaseUrl ?? throw new InvalidOperationException("Missing configuration: HuggingFace:BaseUrl.");
        var model = settings?.Model ?? throw new InvalidOperationException("Missing configuration: HuggingFace:Model.");

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(baseUrl),
            Model = model,
        };

        var chatClient = new OllamaApiClient(config);

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }

    private bool IsValidHuggingFaceModelFormat(string input)
    {
        var segments = input.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length != 3)
        {
            return false;
        }

        if (segments.First().Equals("hf.co", StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        if (segments[^1].EndsWith("-gguf", StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        return true;
    }
}