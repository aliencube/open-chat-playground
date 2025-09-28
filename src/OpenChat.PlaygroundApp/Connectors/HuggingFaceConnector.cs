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
    private const string HuggingFaceHost = "hf.co";
    private const string ModelSuffix = "gguf";

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
        // - hf.co/{org}/{model}gguf e.g hf.co/Qwen/Qwen3-0.6B-GGUF hf.co/Qwen/Qwen3-0.6B_GGUF
        if (IsValidModel(settings.Model) == false)
        {
            throw new InvalidOperationException("Invalid configuration: HuggingFace:Model format. Expected 'hf.co/{org}/{model}gguf' format.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as HuggingFaceSettings;
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

    private static bool IsValidModel(string model)
    {
        var segments = model.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length != 3)
        {
            return false;
        }

        if (segments.First().Equals(HuggingFaceHost, StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        if (segments.Last().EndsWith(ModelSuffix, StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        return true;
    }
}