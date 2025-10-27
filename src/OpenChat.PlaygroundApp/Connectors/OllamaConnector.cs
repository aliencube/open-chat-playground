using Microsoft.Extensions.AI;

using OllamaSharp;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Ollama.
/// </summary>
public class OllamaConnector(AppSettings settings) : LanguageModelConnector(settings.Ollama)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not OllamaSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: Ollama.");
        }

        if (string.IsNullOrWhiteSpace(settings.BaseUrl!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Ollama:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Ollama:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as OllamaSettings;

        var baseUrl = settings!.BaseUrl!.Trim() ?? throw new InvalidOperationException("Missing configuration: Ollama:BaseUrl.");
        if (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute) == false)
        {
            throw new UriFormatException($"Invalid URI: The Ollama base URL '{baseUrl}' is not a valid URI.");
        }
        var model = settings!.Model!.Trim() ?? throw new InvalidOperationException("Missing configuration: Ollama:Model.");

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(baseUrl),
            Model = model,
        };

        var chatClient = new OllamaApiClient(config);

        var pulls = chatClient.PullModelAsync(model); 
        await foreach (var pull in pulls) 
        { 
            Console.WriteLine($"Pull status: {pull!.Status}"); 
        } 
  
        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {model}"); 

        return chatClient;
    }
}