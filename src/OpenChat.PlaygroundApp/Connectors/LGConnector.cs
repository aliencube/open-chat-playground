using Microsoft.Extensions.AI;

using OllamaSharp;
using OllamaSharp.Models.Exceptions;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for LG AI EXAONE.
/// </summary>
public class LGConnector(AppSettings settings) : LanguageModelConnector(settings.LG)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not LGSettings settings)
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

        if (IsValidModel(settings.Model.Trim()) == false)
        {
            throw new InvalidOperationException("Invalid configuration: Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as LGSettings;

        var baseUrl = settings!.BaseUrl!.Trim() ?? throw new InvalidOperationException("Missing configuration: LG:BaseUrl.");
        var model = settings!.Model!.Trim() ?? throw new InvalidOperationException("Missing configuration: LG:Model.");

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

    private static bool IsValidModel(string model)
    {
        var segments = model.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length != 3)
        {
            return false;
        }

        if (segments[0].Equals("hf.co", StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        if (segments[1].Equals("LGAI-EXAONE", StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        if (segments[2].StartsWith("EXAONE-", StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        if (segments[2].EndsWith("-GGUF", StringComparison.InvariantCultureIgnoreCase) == false)
        {
            return false;
        }

        return true;
    }
}
