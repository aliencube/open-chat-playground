using Microsoft.Extensions.AI;

using Anthropic.SDK;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Anthropic.
/// </summary>
public class AnthropicConnector(AppSettings settings) : LanguageModelConnector(settings.Anthropic)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not AnthropicSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: Anthropic.");
        }

        if (string.IsNullOrWhiteSpace(settings.ApiKey?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Anthropic:ApiKey.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model?.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Anthropic:Model.");
        }

        if (settings.MaxTokens is not int maxTokens || maxTokens < 1)
        {
            throw new InvalidOperationException("Invalid configuration: Anthropic:MaxTokens must be an integer >= 1.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as AnthropicSettings;
        var apiKey = settings!.ApiKey!.Trim() ?? throw new InvalidOperationException("Missing configuration: Anthropic:ApiKey.");
        var model = settings.Model!.Trim() ?? throw new InvalidOperationException("Missing configuration: Anthropic:Model.");
        var maxTokens = settings.MaxTokens ?? throw new InvalidOperationException("Missing configuration: Anthropic:MaxTokens.");

        if (maxTokens < 1)
        {
            throw new InvalidOperationException("Invalid configuration: Anthropic:MaxTokens must be >= 1.");
        }

        var client = new AnthropicClient() { Auth = new APIAuthentication(apiKey) };

        var chatClient = client.Messages
                                .AsBuilder()
                                .UseFunctionInvocation()
                                .Use((messages, options, next, cancellationToken) =>
                                {
                                    options!.ModelId = settings!.Model;
                                    options.MaxOutputTokens = settings.MaxTokens;
                                    return next(messages, options, cancellationToken);
                                })
                                .Build();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings!.Model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}