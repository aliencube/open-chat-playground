using System.ClientModel;
using System.Text.RegularExpressions;

using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.AI;

using OpenAI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Foundry Local.
/// </summary>
/// <param name="settings"><see cref="AppSettings"/> instance.</param>
public class FoundryLocalConnector(AppSettings settings) : LanguageModelConnector(settings.FoundryLocal)
{
    private const string ApiKey = "OPENAI_API_KEY";

    private static readonly Regex modelIdSuffix = new(@"\:[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not FoundryLocalSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal.");
        }

        if (settings.DisableFoundryLocalManager == true &&
            string.IsNullOrWhiteSpace(settings.BaseUrl!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal:BaseUrl is required when DisableFoundryLocalManager is enabled.");
        }

        if (string.IsNullOrWhiteSpace(settings.AliasOrModel!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: FoundryLocal:AliasOrModel.");
        }

        if (settings.DisableFoundryLocalManager == true &&
            modelIdSuffix.IsMatch(settings.AliasOrModel!.Trim()!) == false)
        {
            throw new InvalidOperationException("When DisableFoundryLocalManager is enabled, FoundryLocal:AliasOrModel must be the exact model name with version suffix.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as FoundryLocalSettings;

        (Uri? endpoint, string? modelId) = settings!.DisableFoundryLocalManager == true
            ? ParseFromModelId(settings)
            : await ParseFromManagerAsync(settings).ConfigureAwait(false);

        var credential = new ApiKeyCredential(ApiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = endpoint,
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(modelId)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {modelId}");

        return chatClient;
    }

    private static (Uri? endpoint, string? modelId) ParseFromModelId(FoundryLocalSettings settings)
    {
        var baseUrl = settings.BaseUrl!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:BaseUrl.");
        if (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute) == false)
        {
            throw new UriFormatException($"Invalid URI: The Foundry Local base URL '{baseUrl}' is not a valid URI.");
        }

        var endpoint = new Uri($"{baseUrl.TrimEnd('/')}/v1");
        var modelId = settings.AliasOrModel!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:AliasOrModel.");
        if (modelIdSuffix.IsMatch(modelId) == false)
        {
            throw new InvalidOperationException("When DisableFoundryLocalManager is enabled, FoundryLocal:AliasOrModel must be the exact model name with version suffix.");
        }

        return (endpoint, modelId);
   }

    private static async Task<(Uri? endpoint, string? modelId)> ParseFromManagerAsync(FoundryLocalSettings settings)
    {
        var alias = settings!.AliasOrModel!.Trim() ?? throw new InvalidOperationException("Missing configuration: FoundryLocal:AliasOrModel.");
        var manager = await FoundryLocalManager.StartModelAsync(aliasOrModelId: alias).ConfigureAwait(false);
        var model = await manager.GetModelInfoAsync(aliasOrModelId: alias).ConfigureAwait(false);

        var endpoint = manager.Endpoint;
        var modelId = model!.ModelId;

        return (endpoint, modelId);
    }
}