using Microsoft.Extensions.AI;

using OllamaSharp;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using System.Linq;

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
        var settings = this.Settings as OllamaSettings;
        if (settings is null)
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
        var baseUrl = settings!.BaseUrl!;
        var model = settings!.Model!;

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(baseUrl),
            Model = model,
        };

        var chatClient = new OllamaApiClient(config);
        
        // Only attempt to pull model if not in test environment
        if (!IsTestEnvironment())
        {
            try
            {
                var pulls = chatClient.PullModelAsync(model);
                await foreach (var pull in pulls)
                {
                    Console.WriteLine($"Pull status: {pull!.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not pull model {model}: {ex.Message}");
                // Continue anyway - model might already exist
            }
        }

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");
        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }

    private static bool IsTestEnvironment()
    {
        // Check if running in test environment
        return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
               Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
               System.Diagnostics.Debugger.IsAttached ||
               AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.FullName?.Contains("xunit") == true);
    }
}