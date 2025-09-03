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
        if (IsValidHuggingFaceModel(settings.Model) == false)
        {
            throw new InvalidOperationException("Invalid configuration: HuggingFace:Model format. Expected 'hf.co/{org}/{model}-gguf' or with optional qualifier before/after the suffix (case-insensitive), e.g. 'hf.co/org/model:Q4_0-gguf' or 'hf.co/org/model-gguf:Q4_0'.");
        }

        var chatClient = new OllamaApiClient(config);

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }

    bool IsValidHuggingFaceModel(string input)
    {
        if (input.StartsWith("hf.co/", StringComparison.OrdinalIgnoreCase) == false)
        {
            return false;
        }

        var parts = input.Substring("hf.co/".Length).Split('/');
        if (parts.Length != 2)
        {
            return false;
        }

        var (orgName, modelName) = (parts[0], parts[1]);
        if (orgName.Length == 0 || orgName.All(c => char.IsLetterOrDigit(c) || c is '-') == false)
        {
            return false;
        }

        var modelNameParts = modelName.Split(':');
        if (modelNameParts.Length > 2)
        {
            return false;
        }

        var baseModelName = modelNameParts[0];
        if (baseModelName.Length == 0 || baseModelName.All(c => char.IsLetterOrDigit(c) || c is '.' or '_' or '-') == false)
        {
            return false;
        }
        if (baseModelName.EndsWith("-gguf", StringComparison.OrdinalIgnoreCase) == false)
        {
            return false;
        }

        // has optional qualifier
        if (modelNameParts.Length == 2)
        {
            var qualifier = modelNameParts[1];
            if (qualifier.Length == 0 || qualifier.All(c => char.IsLetterOrDigit(c) || c is '_') == false)
            {
                return false;
            }
        }
        return true;
    }

}