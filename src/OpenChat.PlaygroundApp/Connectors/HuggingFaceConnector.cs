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

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(settings.BaseUrl),
            Model = settings.Model,
        };

        // Validate model format after BaseUrl is confirmed to be a valid URI.
        // Accepts formats like:
        // - hf.co/{org}/{model}-gguf
        // - hf.co/{org}/{model}:Q4_0-gguf (qualifier before -gguf)
        // - hf.co/{org}/{model}-gguf:Q4_0 (qualifier after -gguf)
        // Optional colon-prefixed segment (e.g. :Q4_0) is allowed either before or after the -gguf suffix.
        var modelPattern = new Regex(@"^hf\.co\/[A-Za-z0-9._-]+\/[A-Za-z0-9._-]+(?::[A-Za-z0-9._-]+)?-gguf(?::[A-Za-z0-9._-]+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        if (!modelPattern.IsMatch(settings.Model))
        {
            throw new InvalidOperationException("Invalid configuration: HuggingFace:Model format. Expected 'hf.co/{org}/{model}-gguf' or with optional qualifier before/after the suffix (case-insensitive), e.g. 'hf.co/org/model:Q4_0-gguf' or 'hf.co/org/model-gguf:Q4_0'.");
        }

        var chatClient = new OllamaApiClient(config);

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}