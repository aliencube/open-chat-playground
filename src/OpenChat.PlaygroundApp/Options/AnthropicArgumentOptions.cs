using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Anthropic.
/// </summary>
public class AnthropicArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the API key for Anthropic.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Anthropic.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tokens for Anthropic.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var anthropic = settings.Anthropic;

        this.ApiKey ??= anthropic?.ApiKey;
        this.Model ??= anthropic?.Model;
        this.MaxTokens ??= anthropic?.MaxTokens;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ArgumentOptionConstants.Anthropic.ApiKey:
                    if (i + 1 < args.Length)
                    {
                        this.ApiKey = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.Anthropic.Model:
                    if (i + 1 < args.Length)
                    {
                        this.Model = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.Anthropic.MaxTokens:
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var maxTokens))
                    {
                        this.MaxTokens = maxTokens;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}